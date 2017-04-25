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
    public class ShapesController : Controller
    {
        private ISqlService _sqlService;

        public ShapesController(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }

        [HttpGet("{shapeId}")]
        public async Task<List<Shape>> GetShapesById(string shapeId)
        {
            var shapes = await _sqlService.GetShapesById(shapeId);
            return shapes;
        }

    }
}
