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
    public class StopsController : Controller
    {
        private ISqlService _sqlService;

        public StopsController(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }

        [HttpGet]
        public async Task<IEnumerable<Stop>> Get()
        {
            var stops = await _sqlService.GetStops();
            return stops;
        }

        [HttpGet("{stopId}")]
        public async Task<Stop> GetStopById(string stopId)
        {
            var stop = await _sqlService.GetStopById(stopId);
            return stop;
        }

    }
}
