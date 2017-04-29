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
    public class CalendarsController : Controller
    {
        private ISqlService _sqlService;

        public CalendarsController(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }

        [HttpGet("{serviceId}")]
        public async Task<Calendar> GetCalendarByServiceId(string serviceId)
        {
            var calendar = await _sqlService.GetCalendarByServiceId(serviceId);
            return calendar;
        }

    }
}
