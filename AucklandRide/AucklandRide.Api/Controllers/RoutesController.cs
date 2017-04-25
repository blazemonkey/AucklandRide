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
    public class RoutesController : Controller
    {
        private ISqlService _sqlService;

        public RoutesController(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }

        [HttpGet]
        public async Task<IEnumerable<Route>> Get()
        {
            var routes = await _sqlService.GetRoutes();
            return routes;
        }

        [HttpGet("{routeId}")]
        public async Task<Route> GetRouteById(string routeId)
        {
            var route = await _sqlService.GetRouteById(routeId);
            return route;
        }
    }
}
