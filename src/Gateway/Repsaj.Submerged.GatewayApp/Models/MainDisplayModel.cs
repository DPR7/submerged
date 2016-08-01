﻿using Repsaj.Submerged.GatewayApp.Universal.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.GatewayApp.Models
{
    public class MainDisplayModel : NotificationBase
    {
        ObservableCollection<ModuleDisplayModel> _modules = new ObservableCollection<ModuleDisplayModel>();
        public ObservableCollection<ModuleDisplayModel> Modules
        {
            get { return _modules; }
            set { SetProperty(ref _modules, value); }
        }

        ObservableCollection<SensorDisplayModel> _sensors = new ObservableCollection<SensorDisplayModel>();
        public ObservableCollection<SensorDisplayModel> Sensors
        {
            get { return _sensors; }
            set { SetProperty(ref _sensors, value); }
        }

        ObservableCollection<RelayDisplayModel> _relays = new ObservableCollection<RelayDisplayModel>();
        public ObservableCollection<RelayDisplayModel> Relays
        {
            get { return _relays; }
            set { SetProperty(ref _relays, value); }
        }

        public void ProcessSensorData(IEnumerable<Sensor> sensorData)
        {
            List<Sensor> sensorList = sensorData.ToList();

            // process all moisture sensors and remove the entries
            var moistureSensors = sensorList.Where(s => s.SensorType == SensorTypes.MOISTURE);
            ProcessLeakage(moistureSensors);
            sensorList.RemoveAll(s => s.SensorType == SensorTypes.MOISTURE);

            // process all other types of sensors
            foreach (var sensor in sensorList)
            {
                ProcessSensor(sensor);
            }
        }

        private void ProcessLeakage(IEnumerable<Sensor> sensor)
        {
            // all moisture sensors are grouped into one value that aggregates all of the values
            bool value = sensor.Any(s => (bool?)s.Reading == true);
            var sensorModel = this.Sensors.SingleOrDefault(s => s.Name == "Leakage");

            if (sensorModel == null)
            {
                SensorDisplayModel newSensor = new SensorDisplayModel()
                {
                    Name = "Leakage",
                    DisplayName = "Leak detection",
                    Reading = value,
                    SensorType = SensorTypes.MOISTURE
                };

                this.Sensors.Add(newSensor);
            }
            else
            {
                sensorModel.Reading = value;
            }
        }

        private void ProcessSensor(Sensor sensor)
        {
            var displayModel = this.Sensors.SingleOrDefault(s => s.Name == sensor.Name);

            if (displayModel == null && sensor.Reading != null)
            {
                displayModel = new SensorDisplayModel(sensor);                
                this.Sensors.Add(displayModel);
            }
            else if (displayModel != null && sensor.Reading == null)
            {
                this.Sensors.Remove(displayModel);
            }
            else if (displayModel != null && sensor.Reading != null)
            {
                displayModel.Reading = sensor.Reading;
            }
        }

        public void ProcessRelayData(IEnumerable<Relay> relayData)
        {
            foreach (var relay in relayData)
            {
                ProcessRelay(relay);
            }
        }

        private void ProcessRelay(Relay relay)
        {
            var displayModel = this.Relays.SingleOrDefault(s => s.Name == relay.Name);

            if (displayModel == null)
            {
                displayModel = new RelayDisplayModel(relay);
                this.Relays.Add(displayModel);
            }
            else
            {
                displayModel.State = relay.State;
            }
        }

        public void ProcessModuleData(IEnumerable<Module> moduleData)
        {
            foreach (var module in moduleData)
            {
                ProcessModule(module);
            }
        }

        private void ProcessModule(Module module)
        {
            var model = this.Modules.SingleOrDefault(s => s.Name == module.Name);

            if (model == null)
            {
                ModuleDisplayModel newModule = new ModuleDisplayModel(module);
                this.Modules.Add(newModule);
            }
            else
            {
                Debug.WriteLine($"Updated module {module.Name} to status {module.Status}");
                model.Status = module.Status;
            }
        }
    }
}