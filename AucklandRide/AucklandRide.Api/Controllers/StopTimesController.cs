using AucklandRide.Api.Models;
using AucklandRide.Api.Services.SqlService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AucklandRide.Api.Controllers
{
    [Route("api/[controller]")]
    public class StopTimesController : Controller
    {
        private ISqlService _sqlService;

        public StopTimesController(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }


        [HttpGet("{tripId}")]
        public async Task<List<StopTime>> GetStopTimesByTripId(string tripId)
        {
            var stopTimes = await _sqlService.GetStopTimesByTripId(tripId);
            return stopTimes;
        }

    }
}
