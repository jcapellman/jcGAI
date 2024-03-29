﻿using jcGAI.WebAPI.Controllers.Base;
using jcGAI.WebAPI.Managers;
using jcGAI.WebAPI.Objects.NonRelational;
using jcGAI.WebAPI.Services;

using Microsoft.AspNetCore.Mvc;

namespace jcGAI.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/activities")]
    public class ActivitiesController : BaseController<InsightsManager>
    {
        protected ActivitiesController(ILogger<ActivitiesController> logger, MongoDbService mongo) : base(logger, mongo)
        {
        }

        [HttpGet]
        public async Task<IEnumerable<Activities>> GetAsync(DateTime? startTime = null, DateTime? endTime = null) 
            => await _manager.GetActivitiesAsync(UserId, startTime, endTime);
    }
}