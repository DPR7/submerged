﻿using Repsaj.Submerged.Common.Configurations;
using Repsaj.Submerged.Common.Helpers;
using Repsaj.Submerged.Common.Models;
using Repsaj.Submerged.Infrastructure.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.Infrastructure.Repository
{
    public class DeviceRulesRepository : IDeviceRulesRepository
    {
        private readonly IConfigurationProvider _configurationProvider;

        private readonly string _storageAccountConnectionString;
        private readonly string _deviceRulesBlobStoreContainerName;
        private readonly string _deviceRulesNormalizedTableName;
        private readonly string _blobName;

        private DateTimeFormatInfo _formatInfo;

        public DeviceRulesRepository(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;

            _storageAccountConnectionString = configurationProvider.GetConfigurationSettingValue("device.StorageConnectionString");
            _deviceRulesBlobStoreContainerName = configurationProvider.GetConfigurationSettingValue("DeviceRulesStoreContainerName");
            _deviceRulesNormalizedTableName = configurationProvider.GetConfigurationSettingValue("DeviceRulesTableName");
            _blobName = configurationProvider.GetConfigurationSettingValue("AsaRefDataRulesBlobName");

            // note: InvariantCulture is read-only, so use en-US and hardcode all relevant aspects
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            _formatInfo = culture.DateTimeFormat;
            _formatInfo.ShortDatePattern = @"yyyy-MM-dd";
            _formatInfo.ShortTimePattern = @"HH-mm";
        }

        public async Task OverrideRulesForDevice(string deviceId, bool enabled)
        {
            List<DeviceRuleBlobEntity> blobList = await BuildBlobEntityListFromTableRows();
            DeviceRuleBlobEntity rule = blobList.SingleOrDefault(i => i.DeviceId == deviceId);

            // when all rules need to be disabled; set all values to null instead of the value
            // that was saved in table storage
            if (enabled == true)
            {
                foreach (var sensorRule in rule.SensorRules)
                {
                    sensorRule.Threshold = null;
                }
            }

            await PersistRulesToBlobStorageAsync(blobList);
        }

        /// <summary>
        /// Save a Device Rule to the server. This may be either a new rule or an update to an existing rule. 
        /// </summary>
        /// <param name="updateContainer"></param>
        /// <returns></returns>
        public async Task<TableStorageResponse<DeviceRule>> SaveDeviceRuleAsync(DeviceRule updatedRule)
        {
            DeviceRuleTableEntity incomingEntity = BuildTableEntityFromRule(updatedRule);

            TableStorageResponse<DeviceRule> result =
                await AzureTableStorageHelper.DoTableInsertOrReplaceAsync<DeviceRule, DeviceRuleTableEntity>(incomingEntity, BuildRuleFromTableEntity,
                _storageAccountConnectionString, _deviceRulesNormalizedTableName);

            if (result.Status == TableStorageResponseStatus.Successful)
            {
                // Build up a new blob to push up for ASA job ref data
                List<DeviceRuleBlobEntity> blobList = await BuildBlobEntityListFromTableRows();
                await PersistRulesToBlobStorageAsync(blobList);
            }

            return result;
        }

        private DeviceRule BuildRuleFromTableEntity(DeviceRuleTableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                return null;
            }

            var updatedRule = new DeviceRule(tableEntity.RuleId)
            {
                DeviceID = tableEntity.DeviceId,
                DataField = tableEntity.DataField,
                DataType = tableEntity.DataType,
                Threshold = tableEntity.Threshold,
                EnabledState = tableEntity.Enabled,
                Operator = tableEntity.Operator,
                RuleOutput = tableEntity.RuleOutput,
                Etag = tableEntity.ETag
            };

            return updatedRule;
        }

        private DeviceRuleTableEntity BuildTableEntityFromRule(DeviceRule incomingRule)
        {
            DeviceRuleTableEntity tableEntity =
                new DeviceRuleTableEntity(incomingRule.DeviceID, incomingRule.RuleId)
                {
                    DataField = incomingRule.DataField,
                    Threshold = incomingRule.Threshold,
                    Operator = incomingRule.Operator,
                    Enabled = incomingRule.EnabledState,
                    RuleOutput = incomingRule.RuleOutput,
                    DataType = incomingRule.DataType
                };

            if (!string.IsNullOrWhiteSpace(incomingRule.Etag))
            {
                tableEntity.ETag = incomingRule.Etag;
            }

            return tableEntity;
        }

        /// <summary>
        /// Compile all rows from the table storage into the format used in the blob storage for
        /// ASA job reference data.
        /// </summary>
        /// <returns></returns>
        private async Task<List<DeviceRuleBlobEntity>> BuildBlobEntityListFromTableRows()
        {
            IEnumerable<DeviceRuleTableEntity> queryResults = await GetAllRulesFromTable();
            Dictionary<string, DeviceRuleBlobEntity> blobEntityDictionary = new Dictionary<string, DeviceRuleBlobEntity>();

            foreach (DeviceRuleTableEntity rule in queryResults)
            {
                DeviceRuleBlobEntity entity = null;
                if (!blobEntityDictionary.ContainsKey(rule.PartitionKey))
                {
                    entity = new DeviceRuleBlobEntity(rule.PartitionKey);
                    blobEntityDictionary.Add(rule.PartitionKey, entity);
                }
                else
                {
                    entity = blobEntityDictionary[rule.PartitionKey];
                }

                object threshold = rule.Enabled ? (object)rule.Threshold : null;
                SensorRuleEntity sensorRule = new SensorRuleEntity(rule.DataField, threshold, rule.Operator);
                entity.SensorRules.Add(sensorRule);
            }

            return blobEntityDictionary.Values.ToList();
        }

        /// <summary>
        /// Get all Device Rules from AzureTableStorage. If none are found it will return an empty list.
        /// </summary>
        /// <returns>All DeviceRules or an empty list</returns>
        public async Task<List<DeviceRule>> GetAllRulesAsync()
        {
            List<DeviceRule> result = new List<DeviceRule>();

            IEnumerable<DeviceRuleTableEntity> queryResults = await GetAllRulesFromTable();
            foreach (DeviceRuleTableEntity rule in queryResults)
            {
                var deviceRule = BuildRuleFromTableEntity(rule);
                result.Add(deviceRule);
            }

            return result;
        }

        /// <summary>
        /// Retrieve all rules from the database that have been defined for a single device.
        /// If none exist an empty list will be returned. This method guarantees a non-null
        /// result.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public async Task<List<DeviceRule>> GetAllRulesForDeviceAsync(string deviceId)
        {
            var deviceRulesTable = await AzureTableStorageHelper.GetTableAsync(_storageAccountConnectionString, _deviceRulesNormalizedTableName);
            TableQuery<DeviceRuleTableEntity> query = new TableQuery<DeviceRuleTableEntity>().
                Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, deviceId));

            List<DeviceRule> result = new List<DeviceRule>();
            foreach (DeviceRuleTableEntity entity in deviceRulesTable.ExecuteQuery(query))
            {
                result.Add(BuildRuleFromTableEntity(entity));
            }
            return result;
        }

        private async Task<IEnumerable<DeviceRuleTableEntity>> GetAllRulesFromTable()
        {
            var deviceRulesTable = await AzureTableStorageHelper.GetTableAsync(_storageAccountConnectionString, _deviceRulesNormalizedTableName);
            TableQuery<DeviceRuleTableEntity> query = new TableQuery<DeviceRuleTableEntity>();

            return await Task.Run(() =>
                deviceRulesTable.ExecuteQuery(query)
            );
        }

        //When we save data to the blob storage for use as ref data on an ASA job, ASA picks that
        //data up based on the current time, and the data must be finished uploading before that time.
        //
        //From the Azure Team: "What this means is your blob in the path 
        //<...>/devicerules/2015-09-23/15-24/devicerules.json needs to be uploaded before the clock 
        //strikes 2015-09-23 15:25:00 UTC, preferably before 2015-09-23 15:24:00 UTC to be used when 
        //the clock strikes 2015-09-23 15:24:00 UTC"
        //
        //If we have many devices, an upload could take a measurable amount of time.
        //
        //Also, it is possible that the ASA clock is not precisely in sync with the
        //server clock. We want to store our update on a path slightly ahead of the current time so
        //that by the time ASA reads it we will no longer be making any updates to that blob -- i.e.
        //all current changes go into a future blob. We will choose two minutes into the future. In the
        //worst case, if we make a change at 12:03:59 and our write is delayed by ten seconds (until 
        //12:04:09) it will still be saved on the path {date}\12-05 and will be waiting for ASA to 
        //find in one minute.
        private const int blobSaveMinutesInTheFuture = 2;
        private async Task PersistRulesToBlobStorageAsync(List<DeviceRuleBlobEntity> blobList)
        {
            CloudBlobContainer container = await BlobStorageHelper.BuildBlobContainerAsync(_storageAccountConnectionString, _deviceRulesBlobStoreContainerName);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            //settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            string updatedJson = JsonConvert.SerializeObject(blobList, settings);
            DateTime saveDate = DateTime.UtcNow.AddMinutes(blobSaveMinutesInTheFuture);
            string dateString = saveDate.ToString("d", _formatInfo);
            string timeString = saveDate.ToString("t", _formatInfo);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(string.Format(@"{0}\{1}\{2}", dateString, timeString, _blobName));
            await blockBlob.UploadTextAsync(updatedJson);
        }

        /// <summary>
        /// Retrieve a single rule from AzureTableStorage or default if none exists. 
        /// A distinct rule is defined by the combination key deviceID/ruleId
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public async Task<DeviceRule> GetDeviceRuleAsync(string deviceId, string ruleId)
        {
            var deviceRulesTable = await AzureTableStorageHelper.GetTableAsync(_storageAccountConnectionString, _deviceRulesNormalizedTableName);
            TableOperation query = TableOperation.Retrieve<DeviceRuleTableEntity>(deviceId, ruleId);

            TableResult response = await Task.Run(() =>
                deviceRulesTable.Execute(query)
            );

            DeviceRule result = BuildRuleFromTableEntity((DeviceRuleTableEntity)response.Result);
            return result;
        }

        public async Task<TableStorageResponse<DeviceRule>> DeleteDeviceRuleAsync(DeviceRule ruleToDelete)
        {
            DeviceRuleTableEntity incomingEntity = BuildTableEntityFromRule(ruleToDelete);

            TableStorageResponse<DeviceRule> result =
                await AzureTableStorageHelper.DoDeleteAsync<DeviceRule, DeviceRuleTableEntity>(incomingEntity, BuildRuleFromTableEntity,
                _storageAccountConnectionString, _deviceRulesNormalizedTableName);

            if (result.Status == TableStorageResponseStatus.Successful)
            {
                // Build up a new blob to push up for ASA job ref data
                List<DeviceRuleBlobEntity> blobList = await BuildBlobEntityListFromTableRows();
                await PersistRulesToBlobStorageAsync(blobList);
            }

            return result;
        }
    }
}
