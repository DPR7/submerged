﻿using Microsoft.Azure.Mobile.Server.Config;
using Repsaj.Submerged.API.Helpers;
using Repsaj.Submerged.Common.SubscriptionSchema;
using Repsaj.Submerged.Infrastructure.BusinessLogic;
using Repsaj.Submerged.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Repsaj.Submerged.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/tank/log")]
    [MobileAppController]
    [System.Web.Mvc.OutputCache(CacheProfile = "NoCacheProfile")]
    public class TankLogController : ApiController
    {
        private readonly ITankLogLogic _tankLogLogic;

        public TankLogController(ITankLogLogic tankLogLogic)
        {
            _tankLogLogic = tankLogLogic;
        }

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid tankId)
        {
            try
            {
                var result = await _tankLogLogic.GetTankLogAsync(tankId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> Post(TankLog log)
        {
            try
            {
                await _tankLogLogic.SaveTankLogAsync(log);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }


        [Route("")]
        [HttpDelete]
        public async Task<IHttpActionResult> Delete(Guid tankId, Guid logId)
        {
            try
            {
                await _tankLogLogic.DeleteTankLogAsync(tankId, logId);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}
