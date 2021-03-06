﻿using Newtonsoft.Json;
using Repsaj.Submerged.GatewayApp.Universal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Universal.Models
{
    public enum TelemetryTrendIndication
    {
        Unknown,
        Equal,
        Increasing,
        Decreasing
    }

    public class DeviceTelemetryModel
    {
        public string ObjectType { get { return DeviceMessageObjectTypes.TELEMETRY; } }
        public string DeviceId { get; set; }
        public IEnumerable<SensorTelemetryCloudModel> SensorData { get; set; }
    }

    public class SensorTelemetryCloudModel
    {
        public SensorTelemetryCloudModel(string sensorName, object value)
        {
            this.SensorName = sensorName;
            this.Value = value;
        }

        public string SensorName { get; set; }
        public object Value { get; set; }
    }

    public class SensorTelemetryModel
    {
        public SensorTelemetryModel(string sensorName, object value)
        {
            this.SensorName = sensorName;
            this.Value = value;
        }

        public SensorTelemetryModel(string sensorName, object value, TelemetryTrendIndication trend)
        {
            this.SensorName = sensorName;
            this.Value = value;
            this.TrendIndication = trend;
        }

        public string ModuleName { get; set; }
        public string SensorName { get; set; }
        public object Value { get; set; }
        public TelemetryTrendIndication TrendIndication { get; set; }
    }
}
