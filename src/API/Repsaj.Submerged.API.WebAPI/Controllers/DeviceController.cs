﻿using Repsaj.Submerged.API.Helpers;
using Repsaj.Submerged.Infrastructure.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Repsaj.Submerged.API.Controllers
{
    [AllowAnonymous]
    //[Authorize] TODO: switch back to authenticated calls
    [RoutePrefix("api/device")]
    [HostAuthentication("OAuth2Bearer")]
    [System.Web.Mvc.OutputCache(CacheProfile = "NoCacheProfile")]
    public class DeviceController : ApiController
    {
        private readonly ISubscriptionLogic _subscriptionLogic;

        public DeviceController(ISubscriptionLogic subscriptionLogic)
        {
            _subscriptionLogic = subscriptionLogic;
        }

        [Route("updatedeviceinfo")]
        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateDeviceInfo([FromBody]dynamic deviceInfo)
        {
            await _subscriptionLogic.UpdateDeviceFromDeviceInfoPacketAsync(deviceInfo);
            return Ok();
        }

        [Route("updaterequest")]
        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateRequest([FromBody]dynamic updateRequest)
        {
            if (updateRequest == null || updateRequest.DeviceID == null)
                return BadRequest("The request didn't contain a valid DeviceID");

            await _subscriptionLogic.SendDeviceConfigurationMessage((string)updateRequest.DeviceID);
            return Ok();
        }
    }
}