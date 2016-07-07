﻿using Autofac.Extras.Moq;
using Repsaj.Submerged.Common.Models;
using Repsaj.Submerged.Common.SubscriptionSchema;
using Repsaj.Submerged.Infrastructure.BusinessLogic;
using Repsaj.Submerged.Infrastructure.Models;
using Repsaj.Submerged.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repsaj.Submerged.API.Tests.Helpers
{
    public static class TestInjectors
    {
        public static void InjectMockedSecurityKey(AutoMock autoMock)
        {
            autoMock.Mock<ISecurityKeyGenerator>()
                .Setup(x => x.CreateRandomKeys())
                .Returns(new SecurityKeys("key1", "key2"));
        }

        public static void InjectMockedSubscription(AutoMock autoMock, bool addDevice = false)
        {
            DeviceModel device = null;

            // if a device should be added; generate one 
            if (addDevice)
                device = DeviceModel.BuildDevice(TestConfigHelper.DeviceId, true);

            InjectMockedSubscription(autoMock, device);
        }

        public static void InjectMockedSubscription(AutoMock autoMock, TankModel tank)
        {
            autoMock.Mock<ISubscriptionRepository>()
                .Setup(x => x.GetSubscriptionAsync(TestConfigHelper.SubscriptionUser))
                .Returns(() => { return ReturnSubscriptionAsync(null, tank); });
        }

        public static void InjectMockedSubscription(AutoMock autoMock, DeviceModel device)
        {
            var subscription = SubscriptionModel.BuildSubscription(Guid.NewGuid(), TestConfigHelper.SubscriptionName, "Test Subscription", TestConfigHelper.SubscriptionUser);

            // add a preconfigured device to the subscription when required

            autoMock.Mock<ISubscriptionRepository>()
                .Setup(x => x.GetSubscriptionAsync(TestConfigHelper.SubscriptionUser))
                .Returns(() => { return ReturnSubscriptionAsync(device); });
            autoMock.Mock<ISubscriptionRepository>()
                .Setup(x => x.GetSubscriptionByDeviceId(TestConfigHelper.DeviceId, TestConfigHelper.SubscriptionUser, false))
                .Returns(() => { return ReturnSubscriptionAsync(device); });
        }

        private static async Task<SubscriptionModel> ReturnSubscriptionAsync(DeviceModel device = null, TankModel tank = null)
        { 
            var result = SubscriptionModel.BuildSubscription(Guid.NewGuid(), TestConfigHelper.SubscriptionName, "Test Subscription", TestConfigHelper.SubscriptionUser);

            if (device != null)
                result.Devices.Add(device);

            if (tank != null)
                result.Tanks.Add(tank);

            return result;
        }

        internal static void InjectDeviceRules(AutoMock autoMock, DeviceRule rule)
        {
            if (rule == null)
            {
                rule = DeviceRule.BuildRule(TestConfigHelper.DeviceId);
            }

            autoMock.Mock<IDeviceRulesLogic>()
                    .Setup(x => x.GetNewRuleAsync(TestConfigHelper.DeviceId))
                    .Returns(() => Task.FromResult(rule));

            autoMock.Mock<IDeviceRulesLogic>()
                    .Setup(x => x.GetDeviceRulesAsync(TestConfigHelper.DeviceId))
                    .Returns(() => Task.FromResult(new List<DeviceRule>(new DeviceRule[] { rule })));

            autoMock.Mock<IDeviceRulesRepository>()
                    .Setup(x => x.GetAllRulesForDeviceAsync(TestConfigHelper.DeviceId))
                    .Returns(() => Task.FromResult(new List<DeviceRule>(new DeviceRule[] { rule })));
        }

        static string mockedModuleName = "Mocked Module";
        static string mockedSensorName = "Mocked Sensor";
        static string mockedModuleConnectionString = "ConnectionString";
        static string mockedSensorDescription = "Fake Sensor";

        public static ModuleModel InjectMockedModule(DeviceModel device)
        {
            var module = ModuleModel.BuildModule(mockedModuleName, mockedModuleConnectionString, ModuleTypes.CABINET);
            device.Modules.Add(module);

            return module;
        }

        public static SensorModel InjectSensorModel(DeviceModel device)
        {
            var sensor = SensorModel.BuildSensor(mockedSensorName, mockedSensorDescription, SensorTypes.PH, mockedModuleName);
            device.Sensors.Add(sensor);

            return sensor;
        }
    }
}
