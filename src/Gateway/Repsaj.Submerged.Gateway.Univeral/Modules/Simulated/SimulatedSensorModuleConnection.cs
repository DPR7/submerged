﻿using Newtonsoft.Json.Linq;
using Repsaj.Submerged.GatewayApp.Universal.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Universal.Modules.Simulated
{
    public class SimulatedSensorModuleConnection : SimulatedModuleConnectionBase, ISensorModule
    {
        private Sensor[] _sensors;

        public IEnumerable<Sensor> Sensors
        {
            get { return _sensors; }
        }

        double basePh = 7.0;
        double baseTemp1 = 24;
        double baseTemp2 = 20;

        public SimulatedSensorModuleConnection(string moduleName, Sensor[] sensors) : base(moduleName)
        {
            this._sensors = sensors;
        }

        public override string ModuleType
        {
            get
            {
                return ModuleTypeDisplayNames.SENSORS;
            }
        }

        public Task<IEnumerable<SensorTelemetryModel>> RequestSensorData()
        {
            Random rand = new Random();

            double currentTemp1 = baseTemp1 + rand.NextDouble() * 5 - 2.5;
            double currentTemp2 = baseTemp2 + rand.NextDouble() * 5 - 2.5;
            double currentPH = basePh + rand.NextDouble() * 1.2 - 0.6;

            List<SensorTelemetryModel> sensorData = new List<SensorTelemetryModel>();
            sensorData.Add(new SensorTelemetryModel("temperature1", currentTemp1));
            sensorData.Add(new SensorTelemetryModel("temperature2", currentTemp2));
            sensorData.Add(new SensorTelemetryModel("pH", currentPH));

            IEnumerable<SensorTelemetryModel> result = sensorData;
            return Task.FromResult(result);
        }
    }
}
