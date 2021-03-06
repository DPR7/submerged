﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repsaj.Submerged.Infrastructure.BusinessLogic;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Repsaj.Submerged.API.Tests.Helpers;
using System.Collections.Generic;
using Repsaj.Submerged.Common.DeviceSchema;
using Repsaj.Submerged.Infrastructure.Repository;
using Repsaj.Submerged.Common.Models.Commands;
using Repsaj.Submerged.Common.SubscriptionSchema;

namespace Repsaj.Submerged.API.Tests.UnitTests
{
    [TestClass]
    public class DeviceLogicTests
    {
        [TestMethod]
        public void Send_Command_Success()
        {
            using (var autoMock = AutoMock.GetLoose())
            {
                DeviceModel device = DeviceModel.BuildDevice(TestConfigHelper.DeviceId, true);
                autoMock.Mock<ISubscriptionRepository>()
                        .Setup(x => x.GetDeviceAsync(TestConfigHelper.DeviceId))
                        .Returns(() => Task.FromResult<DeviceModel>(device));

                Dictionary<string, object> commandParams = new Dictionary<string, object>();
                commandParams.Add("RelayNumber", 1);
                commandParams.Add("RelayState", true);

                var deviceLogic = autoMock.Create<DeviceLogic>();
                deviceLogic.SendCommandAsync(TestConfigHelper.DeviceId, DeviceCommandTypes.SWITCH_RELAY, commandParams).Wait();
            }
        }

        //[TestMethod]
        //public async Task Update_Device_Success()
        //{
        //    using (var autoMock = AutoMock.GetLoose())
        //    {
        //        var deviceLogic = autoMock.Create<DeviceLogic>();

        //        var device = await deviceLogic.GetDeviceAsync(TestConfigHelper.DeviceId);
        //        await deviceLogic.UpdateDeviceAsync(device);
        //    }
        //}

        //[TestMethod]
        //public async Task CanExtractDevicePropertyValuesModels()
        //{
        //    using (var autoMock = AutoMock.GetLoose())
        //    {
        //        var deviceLogic = autoMock.Create<DeviceLogic>();
        //        var device = await deviceLogic.GetDeviceAsync(TestConfigHelper.DeviceId);
        //        deviceLogic.ExtractDevicePropertyValuesModels(device);
        //    }
        //}

        //[TestMethod]
        //public async Task CanApplyDevicePropertyValueModels()
        //{
        //    using (var autoMock = AutoMock.GetLoose())
        //    {
        //        var deviceLogic = autoMock.Create<DeviceLogic>();
        //        var device = await deviceLogic.GetDeviceAsync(TestConfigHelper.DeviceId);
        //        var propertyValueModel = deviceLogic.ExtractDevicePropertyValuesModels(device);
        //        deviceLogic.ApplyDevicePropertyValueModels(device, propertyValueModel);
        //    }
        //}
    }
}
