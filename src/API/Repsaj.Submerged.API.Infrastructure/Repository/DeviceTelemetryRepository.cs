﻿using Repsaj.Submerged.Common.Configurations;
using Repsaj.Submerged.Common.Helpers;
using Repsaj.Submerged.Infrastructure.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.Infrastructure.Repository
{
    using Microsoft.WindowsAzure.Storage;
    using System.Globalization;
    using System.Net;
    using StrDict = IDictionary<string, string>;

    public class DeviceTelemetryRepository : IDeviceTelemetryRepository
    {
        private readonly string _telemetryContainerName;
        private readonly string _telemetryDataPrefix;
        private readonly string _telemetryStoreConnectionString;
        private readonly string _telemetrySummaryPrefix;

        /// <summary>
        /// Initializes a new instance of the DeviceTelemetryRepository class.
        /// </summary>
        /// <param name="configProvider">
        /// The IConfigurationProvider implementation with which to initialize 
        /// the new instance.
        /// </param>
        public DeviceTelemetryRepository(IConfigurationProvider configProvider)
        {
            if (configProvider == null)
            {
                throw new ArgumentNullException("configProvider");
            }

            _telemetryContainerName = configProvider.GetConfigurationSettingValue("TelemetryStoreContainerName");
            _telemetryDataPrefix = configProvider.GetConfigurationSettingValue("TelemetryDataPrefix");
            _telemetryStoreConnectionString = configProvider.GetConfigurationSettingValue("device.StorageConnectionString");
            _telemetrySummaryPrefix = configProvider.GetConfigurationSettingValue("TelemetrySummaryPrefix");
        }

        /// <summary>
        /// Loads the most recent Device telemetry.
        /// </summary>
        /// <param name="deviceId">
        /// The ID of the Device for which telemetry should be returned.
        /// </param>
        /// <param name="minTime">
        /// The minimum time of record of the telemetry that should be returned.
        /// </param>
        /// <returns>
        /// Telemetry for the Device specified by deviceId, inclusively since 
        /// minTime.
        /// </returns>
        public async Task<DeviceTelemetryModel> LoadLatestDeviceTelemetryAsync(
            string deviceId)
        {
            DeviceTelemetryModel result = null;

            CloudBlobContainer container =
                await BlobStorageHelper.BuildBlobContainerAsync(this._telemetryStoreConnectionString, _telemetryContainerName);

            IEnumerable<IListBlobItem> blobs =
                await BlobStorageHelper.LoadBlobItemsAsync(
                    async (token) =>
                    {
                        return await container.ListBlobsSegmentedAsync(
                            _telemetryDataPrefix,
                            true,
                            BlobListingDetails.None,
                            null,
                            token,
                            null,
                            null);
                    });


            // TODO: instead of doing it like this, all blobmodels should be loaded from files that have the correct 
            // datetime pattern in the folder structure. Then we should do a compare to find out what the latest 
            // record was. This is a temporary fix.
            blobs = blobs.OrderByDescending(t => BlobStorageHelper.ExtractBlobItemDate(t))
                         .OrderByDescending(t => BlobStorageHelper.ExtractBlobItemPropertyDate(t));

            CloudBlockBlob blockBlob;
            IEnumerable<DeviceTelemetryModel> blobModels;
            foreach (IListBlobItem blob in blobs)
            {
                if ((blockBlob = blob as CloudBlockBlob) == null)
                {
                    continue;
                }

                try
                {
                    blobModels = await LoadBlobTelemetryModelsAsync(blockBlob);
                }
                catch
                {
                    continue;
                }

                if (blobModels == null)
                {
                    break;
                }

                int preFilterCount = blobModels.Count();

                blobModels =
                    blobModels.Where(
                        t => (t != null) &&
                             t.EventEnqueuedUTCTime.HasValue)
                              .OrderByDescending(
                        t => t.EventEnqueuedUTCTime.Value);

                if (preFilterCount == 0)
                {
                    break;
                }

                if (!string.IsNullOrEmpty(deviceId))
                {
                    blobModels = blobModels.Where(t => t.DeviceId == deviceId);
                }

                result = blobModels.FirstOrDefault();
                if (result != null)
                    break;
            }

            return result;
        }

        /// <summary>
        /// Loads the most recent Device telemetry.
        /// </summary>
        /// <param name="deviceId">
        /// The ID of the Device for which telemetry should be returned.
        /// </param>
        /// <param name="minTimeUTC">
        /// The minimum time of record of the telemetry that should be returned.
        /// </param>
        /// <returns>
        /// Telemetry for the Device specified by deviceId, inclusively since 
        /// minTime.
        /// </returns>
        public async Task<IEnumerable<DeviceTelemetryModel>> LoadLatestDeviceTelemetryAsync(
            string deviceId,
            DateTimeOffset minTimeUTC)
        {
            IEnumerable<DeviceTelemetryModel> result = new DeviceTelemetryModel[0];

            CloudBlobContainer container =
                await BlobStorageHelper.BuildBlobContainerAsync(this._telemetryStoreConnectionString, _telemetryContainerName);

            IEnumerable<IListBlobItem> blobs =
                await BlobStorageHelper.LoadBlobItemsAsync(
                    async (token) =>
                    {
                        return await container.ListBlobsSegmentedAsync(
                            _telemetryDataPrefix,
                            true,
                            BlobListingDetails.None,
                            null,
                            token,
                            null,
                            null);
                    });

            // select all the blobs, extract the datetime from the name and then filter 
            // based on the provided period
            blobs = blobs
                        .Select(b => new Tuple<DateTime?, IListBlobItem>(BlobStorageHelper.ExtractBlobItemDate(b), b))
                        .Where(b => b.Item1.HasValue && b.Item1.Value >= minTimeUTC.Date)
                        .OrderByDescending(b => b.Item1)
                        .Select(b => b.Item2);

            CloudBlockBlob blockBlob;
            IEnumerable<DeviceTelemetryModel> blobModels;
            foreach (IListBlobItem blob in blobs)
            {
                if ((blockBlob = blob as CloudBlockBlob) == null)
                {
                    continue;
                }

                try
                {
                    blobModels = await LoadBlobTelemetryModelsAsync(blockBlob);
                }
                catch
                {
                    continue;
                }

                if (blobModels == null)
                {
                    break;
                }

                int preFilterCount = blobModels.Count();

                blobModels =
                    blobModels.Where(
                        t =>
                            (t != null) &&
                            t.EventEnqueuedUTCTime.HasValue &&
                            t.EventEnqueuedUTCTime.Value >= minTimeUTC);

                if (preFilterCount == 0)
                {
                    break;
                }

                result = result.Concat(blobModels);

                if (preFilterCount != blobModels.Count())
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(deviceId))
            {
                result = result.Where(t => t.DeviceId == deviceId);
            }

            return result;
        }

        /// <summary>
        /// Loads the most recent Device telemetry.
        /// </summary>
        /// <param name="deviceId">
        /// The ID of the Device for which telemetry should be returned.
        /// </param>
        /// <param name="minTimeUTC">
        /// The minimum time of record of the telemetry that should be returned.
        /// </param>
        /// <returns>
        /// Telemetry for the Device specified by deviceId, inclusively since 
        /// minTime.
        /// </returns>
        public async Task<IEnumerable<DeviceTelemetryModel>> LoadDeviceTelemetryAsync(
            string deviceId,
            DateTimeOffset minTimeUTC,
            DateTimeOffset maxTimeUTC)
        {
            IEnumerable<DeviceTelemetryModel> result = new DeviceTelemetryModel[0];

            CloudBlobContainer container =
                await BlobStorageHelper.BuildBlobContainerAsync(this._telemetryStoreConnectionString, _telemetryContainerName);

            IEnumerable<IListBlobItem> blobs =
                await BlobStorageHelper.LoadBlobItemsAsync(
                    async (token) =>
                    {
                        return await container.ListBlobsSegmentedAsync(
                            _telemetryDataPrefix,
                            true,
                            BlobListingDetails.None,
                            null,
                            token,
                            null,
                            null);
                    });

            // select all the blobs, extract the datetime from the name and then filter 
            // based on the provided period
            blobs = blobs
                        .Select(b => new Tuple<DateTime?, IListBlobItem>(BlobStorageHelper.ExtractBlobItemDate(b), b))
                        .Where(b => b.Item1.HasValue && b.Item1.Value <= maxTimeUTC.Date && b.Item1.Value >= minTimeUTC.Date)
                        .OrderByDescending(b => b.Item1)
                        .Select(b => b.Item2);

            CloudBlockBlob blockBlob;
            IEnumerable<DeviceTelemetryModel> blobModels;
            foreach (IListBlobItem blob in blobs)
            {
                if ((blockBlob = blob as CloudBlockBlob) == null)
                {
                    continue;
                }

                try
                {
                    blobModels = await LoadBlobTelemetryModelsAsync(blockBlob);
                }
                catch
                {
                    continue;
                }

                if (blobModels == null)
                {
                    break;
                }

                int preFilterCount = blobModels.Count();

                blobModels =
                    blobModels.Where(
                        t =>
                            (t != null) &&
                            t.EventEnqueuedUTCTime.HasValue &&
                            t.EventEnqueuedUTCTime.Value >= minTimeUTC && 
                            t.EventEnqueuedUTCTime.Value < maxTimeUTC);

                if (preFilterCount == 0)
                {
                    break;
                }

                result = result.Concat(blobModels);

                if (preFilterCount != blobModels.Count())
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(deviceId))
            {
                result = result.Where(t => t.DeviceId == deviceId);
            }

            return result;
        }

        private async static Task<List<DeviceTelemetryModel>> LoadBlobTelemetryModelsAsync(CloudBlockBlob blob)
        {
            Debug.Assert(blob != null, "blob is a null reference.");

            List<DeviceTelemetryModel> models = new List<DeviceTelemetryModel>();

            TextReader reader = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                OperationContext context = new OperationContext();
                BlobRequestOptions options = new BlobRequestOptions()
                {
                     
                };
                await blob.DownloadToStreamAsync(stream, null, options, context);
                stream.Position = 0;
                reader = new StreamReader(stream);

                models = ParsingHelper.ParseJson<DeviceTelemetryModel>(reader).ToList();               
            }
            finally
            {
                IDisposable disp;

                if ((disp = stream) != null)
                {
                    disp.Dispose();
                }

                if ((disp = reader) != null)
                {
                    disp.Dispose();
                }
            }

            return models;
        }

        /// <summary>
        /// Loads the most recent DeviceTelemetrySummaryModel for a specified Device.
        /// </summary>
        /// <param name="deviceId">
        /// The ID of the Device for which a telemetry summary model should be 
        /// returned.
        /// </param>
        /// <param name="minTimeUTC">
        /// If provided the the minimum time stamp of the summary data that should 
        /// be loaded.
        /// </param>
        /// <returns>
        /// The most recent DeviceTelemetrySummaryModel for the Device, 
        /// specified by deviceId.
        /// </returns>
        public async Task<DeviceTelemetrySummaryModel> LoadDeviceTelemetrySummaryAsync(
            string deviceId,
            DateTimeOffset minTimeUTC)
        {
            DeviceTelemetrySummaryModel summaryModel = null;

            CloudBlobContainer container =
                await BlobStorageHelper.BuildBlobContainerAsync(
                    this._telemetryStoreConnectionString,
                    _telemetryContainerName);

            IEnumerable<IListBlobItem> blobs =
                await BlobStorageHelper.LoadBlobItemsAsync(
                    async (token) =>
                    {
                        return await container.ListBlobsSegmentedAsync(
                            _telemetrySummaryPrefix,
                            true,
                            BlobListingDetails.None,
                            null,
                            token,
                            null,
                            null);
                    });

            // select all the blobs, extract the datetime from the name and then filter 
            // based on the provided period
            blobs = blobs
                        .Select(b => new Tuple<DateTime?, IListBlobItem>(BlobStorageHelper.ExtractBlobItemDate(b), b))
                        .Where(b => b.Item1.HasValue && b.Item1.Value >= minTimeUTC.Date)
                        .OrderByDescending(b => b.Item1)
                        .Select(b => b.Item2);

            IEnumerable<DeviceTelemetrySummaryModel> blobModels;
            CloudBlockBlob blockBlob;

            foreach (IListBlobItem blob in blobs)
            {
                if ((blockBlob = blob as CloudBlockBlob) == null)
                {
                    continue;
                }

                try
                {
                    blobModels = await LoadBlobTelemetrySummaryModelsAsync(blockBlob);
                }
                catch
                {
                    continue;
                }

                if (blobModels == null)
                {
                    break;
                }

                blobModels = blobModels.Where(t => t != null);

                if (!string.IsNullOrEmpty(deviceId))
                {
                    blobModels = blobModels.Where(t => t.DeviceId == deviceId);
                }

                summaryModel = blobModels.LastOrDefault();
                if (summaryModel != null)
                {
                    break;
                }
            }

            return summaryModel;
        }

        /// <summary>
        /// Loads the most recent DeviceTelemetrySummaryModel for a specified Device.
        /// </summary>
        /// <param name="deviceId">
        /// The ID of the Device for which a telemetry summary model should be 
        /// returned.
        /// </param>
        /// <param name="minTimeUTC">
        /// If provided the the minimum time stamp of the summary data that should 
        /// be loaded.
        /// </param>
        /// <returns>
        /// The most recent DeviceTelemetrySummaryModel for the Device, 
        /// specified by deviceId.
        /// </returns>
        public async Task<IEnumerable<DeviceTelemetrySummaryModel>> LoadDeviceTelemetrySummaryAsync(
            string deviceId,
            DateTimeOffset minTimeUTC,
            DateTimeOffset maxTimeUTC)
        {
            CloudBlobContainer container =
                await BlobStorageHelper.BuildBlobContainerAsync(
                    this._telemetryStoreConnectionString,
                    _telemetryContainerName);

            IEnumerable<IListBlobItem> blobs =
                await BlobStorageHelper.LoadBlobItemsAsync(
                    async (token) =>
                    {
                        return await container.ListBlobsSegmentedAsync(
                            _telemetrySummaryPrefix,
                            true,
                            BlobListingDetails.None,
                            null,
                            token,
                            null,
                            null);
                    });

            // select all the blobs, extract the datetime from the name and then filter 
            // based on the provided period
            blobs = blobs
                        .Select(b => new Tuple<DateTime?, IListBlobItem>(BlobStorageHelper.ExtractBlobItemDate(b), b))
                        .Where(b => b.Item1.HasValue && b.Item1.Value <= maxTimeUTC.Date && b.Item1.Value >= minTimeUTC.Date)
                        .OrderByDescending(b => b.Item1)
                        .Select(b => b.Item2);

            IEnumerable<DeviceTelemetrySummaryModel> blobModels;
            List<DeviceTelemetrySummaryModel> result = new List<DeviceTelemetrySummaryModel>();
            CloudBlockBlob blockBlob;

            foreach (IListBlobItem blob in blobs)
            {
                if ((blockBlob = blob as CloudBlockBlob) == null)
                {
                    continue;
                }

                try
                {
                    blobModels = await LoadBlobTelemetrySummaryModelsAsync(blockBlob);
                }
                catch
                {
                    continue;
                }

                if (blobModels == null)
                {
                    break;
                }

                // filter the models based on the given datetime range
                blobModels = blobModels.Where(t => t != null &&
                                                   t.OutTime != null &&
                                                   t.OutTime >= minTimeUTC && t.OutTime < maxTimeUTC);

                if (!string.IsNullOrEmpty(deviceId))
                {
                    blobModels = blobModels.Where(t => t.DeviceId == deviceId);
                }

                result.AddRange(blobModels);
            }

            result = result.OrderBy(r => r.OutTime).ToList();

            return result;
        }

        private async static Task<List<DeviceTelemetrySummaryModel>> LoadBlobTelemetrySummaryModelsAsync(
            CloudBlockBlob blob)
        {
            Debug.Assert(blob != null, "blob is a null reference.");

            var models = new List<DeviceTelemetrySummaryModel>();

            TextReader reader = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                await blob.DownloadToStreamAsync(stream);
                stream.Position = 0;
                reader = new StreamReader(stream);

                models = ParsingHelper.ParseJson<DeviceTelemetrySummaryModel>(reader).ToList();

                foreach (var model in models)
                { 
                    // Translate LastModified to local time zone.  DateTimeOffsets 
                    // don't do this automatically.  This is for equivalent behavior 
                    // with parsed DateTimes.
                    if ((blob.Properties != null) &&
                        blob.Properties.LastModified.HasValue)
                    {
                        model.Timestamp = blob.Properties.LastModified.Value.DateTime;
                    }
                }
            }
            finally
            {
                IDisposable disp;
                if ((disp = stream) != null)
                {
                    disp.Dispose();
                }

                if ((disp = reader) != null)
                {
                    disp.Dispose();
                }
            }

            return models;
        }
    }
}
