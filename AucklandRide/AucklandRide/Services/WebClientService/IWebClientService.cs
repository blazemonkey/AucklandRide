using AucklandRide.Updater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Services.WebClientService
{
    public interface IWebClientService
    {
        Task<IEnumerable<Agency>> GetAgencies();
        Task<IEnumerable<Calendar>> GetCalendars();
        Task<IEnumerable<Route>> GetRoutes();
        Task<IEnumerable<Shape>> GetShapes();
        Task<IEnumerable<Stop>> GetStops();
        Task<IEnumerable<StopTime>> GetStopTimes();
        Task<IEnumerable<Trip>> GetTrips();
    }
}
