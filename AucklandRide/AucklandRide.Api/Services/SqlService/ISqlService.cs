﻿using AucklandRide.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AucklandRide.Api.Services.SqlService
{
    public interface ISqlService
    {
        Task<List<Agency>> GetAgencies();
        Task<Calendar> GetCalendarByServiceId(string serviceId);
        Task<List<Route>> GetRoutes();
        Task<Route> GetRouteById(string routeId);
        Task<List<Shape>> GetShapesById(string shapeId);
        Task<List<Stop>> GetStops();
        Task<Stop> GetStopById(string stopId);
        Task<List<StopTime>> GetStopTimesByTripId(string tripId);
    }
}
