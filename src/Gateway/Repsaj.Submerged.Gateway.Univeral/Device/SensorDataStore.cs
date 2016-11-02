﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repsaj.Submerged.GatewayApp.Universal.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using Repsaj.Submerged.GatewayApp.Universal.Helpers;

namespace Repsaj.Submerged.GatewayApp.Universal.Modules
{
    public class SensorDatastore : ISensorDataStore
    {
        ConcurrentDictionary<Tuple<string, string>, FixedSizeQueue<SensorTelemetryModel>> _datastore;

        public SensorDatastore()
        {
            this._datastore = new ConcurrentDictionary<Tuple<string, string>, FixedSizeQueue<SensorTelemetryModel>>();
        }

        /// <summary>
        /// Stores all incoming sensor data in queues per sensor to keep 
        /// track of averages
        /// </summary>
        /// <param name="sensorTelemetry"></param>
        public void ProcessData(ISensorModule module, IEnumerable<SensorTelemetryModel> sensorTelemetry)
        {
            if (sensorTelemetry != null)
            {
                foreach (var telemetryRecord in sensorTelemetry)
                {
                    SaveSensorData(module, telemetryRecord);
                }
            }

            // check for sensors which have a queue but were not present in the collection
            var missingSensorQueues = this._datastore.Where(d => d.Key.Item1 == module.ModuleName && (sensorTelemetry?.Any(s => d.Key.Item2 == s.SensorName) == false))
                                                     .Select(d => new
                                                     {
                                                         SensorName = d.Key.Item2,
                                                         SensorQueue = d.Value
                                                     });

            // store a null for each queue that was not present
            foreach (var missing in missingSensorQueues)
                missing.SensorQueue.Enqueue(new SensorTelemetryModel(missing.SensorName, null));                                                
        }

        private void SaveSensorData(ISensorModule module, SensorTelemetryModel telemetryRecord)
        {
            string sensorName = telemetryRecord.SensorName;

            // ensure there will be a queue for every sensor in the colletion
            if (! _datastore.Keys.Any(k => k.Item1 == module.ModuleName && k.Item2 == sensorName))
            {
                Sensor sensorDefinition = module.Sensors.SingleOrDefault(s => s.Name == sensorName);

                if (sensorDefinition == null)
                    throw new ArgumentException($"Module {module.ModuleName} is not configured for sensor {sensorName}");

                // create a new queue and set the capacity based on the predefined queue capacity
                int queueCapacity = GetQueueCapacityForSensor(sensorDefinition);
                _datastore.TryAdd(new Tuple<string, string>(module.ModuleName, sensorName), new FixedSizeQueue<SensorTelemetryModel>(queueCapacity));
            }
            
            // fetch the queue, store the item and pop off any excess items
            var queue = _datastore.Single(i => i.Key.Item1 == module.ModuleName && i.Key.Item2 == sensorName);
            queue.Value.Enqueue(telemetryRecord);
        }

        private int GetQueueCapacityForSensor(Sensor sensor)
        {
            // for sensors returning numeric values, use a capacity of 6 for a running average
            if (sensor.SensorType == SensorTypes.FLOW ||
                sensor.SensorType == SensorTypes.PH ||
                sensor.SensorType == SensorTypes.TEMPERATURE)
                return 6;

            // for sensors returning booleans or other non-numeric values, use 1 so the datastore will just return the last recorded value
            if (sensor.SensorType == SensorTypes.MOISTURE ||
                sensor.SensorType == SensorTypes.STOCKFLOAT)
                return 1;

            throw new ArgumentException($"The sensor type {sensor.SensorType} was not configured to return a queue capacity.");
        }

        private TelemetryTrendIndication CalculateTrend(IEnumerable<SensorTelemetryModel> telemetry)
        {
            int hasGoneUp = 0;
            int hasGoneDown = 0;
            int hasRemainedEqual = 0;
            SensorTelemetryModel previous = null;

            foreach (var item in telemetry)
            {
                if (previous == null)
                {
                    previous = item;
                    continue;
                }

                // skip when the value inserted is not a double value
                if (!(item.Value is double))
                    continue;

                // increase the correct instance based on comparing current value with previous
                hasGoneUp += (double)item.Value > (double)previous.Value ? 1 : 0;
                hasGoneDown += (double)item.Value < (double)previous.Value ? 1 : 0;
                hasRemainedEqual += (double)item.Value == (double)previous.Value ? 1 : 0;

                previous = item;
            }

            // Exactly one of hasGoneUp and hasGoneDown is true by this point
            double criticalCount = telemetry.Count() * 0.75;
            if (hasGoneUp > criticalCount)
                return TelemetryTrendIndication.Increasing;
            else if (hasGoneDown > criticalCount)
                return TelemetryTrendIndication.Decreasing;
            else if (hasRemainedEqual > criticalCount)
                return TelemetryTrendIndication.Equal;
            else
                return TelemetryTrendIndication.Unknown;
        }

        /// <summary>
        /// Returns the average of the sensordata stored in the datastore. Stored null values will not weigh in for the average,
        /// but sensors which only have null values will not be returned.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SensorTelemetryModel> GetData()
        {
            List<SensorTelemetryModel> result = new List<SensorTelemetryModel>();

            try
            {
                foreach (var sensorQueue in _datastore)
                {
                    // if all of the queue values are null, the sensor is skipped
                    if (sensorQueue.Value.All(v => v.Value == null))
                    {
                        result.Add(new SensorTelemetryModel(sensorQueue.Key.Item2, null));
                    }
                    // if the queue length is just one, simply return the value
                    else if (sensorQueue.Value.Count == 1)
                    {
                        result.Add(sensorQueue.Value.First());
                    }
                    // if the queue length is larger then one the sensor values should be numeric and we'll calculate
                    // the running average
                    else
                    {
                        var values = sensorQueue.Value.Where(v => v.Value != null);

                        double sensorSum = values.Sum(v => Convert.ToDouble(v.Value));
                        double sensorAverage = sensorSum / (double)values.Count();

                        TelemetryTrendIndication trend = CalculateTrend(values);

                        result.Add(new SensorTelemetryModel(sensorQueue.Key.Item2, sensorAverage, trend));
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Cannot calculate stored sensor values due to: {ex}.");
            }

            return result;
        }
    }
}
