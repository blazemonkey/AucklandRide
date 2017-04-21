using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AucklandRide.Api.Services.SqlService;
using AucklandRide.Api.Models;

namespace AucklandRide.Api.Controllers
{
    [Route("api/[controller]")]
    public class AgenciesController : Controller
    {
        private ISqlService _sqlService;

        public AgenciesController(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }

        [HttpGet]
        public async Task<IEnumerable<Agency>> Get()
        {
            var agencies = await _sqlService.GetAgencies();
            return agencies;
        }
    }
}
