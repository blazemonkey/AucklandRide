using AucklandRide.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AucklandRide.Api.Services.SqlService
{
    public interface ISqlService
    {
        Task<List<Agency>> GetAgencies();
        Task<List<Stop>> GetStops();
        Task<Stop> GetStopById(string stopId);
    }
}
