using AucklandRide.Updater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.SqlService
{
    public interface ISqlService
    {
        Task DeleteAndReplaceAll(IEnumerable<Agency> agencies, IEnumerable<Calendar> calendars, IEnumerable<CalendarDate> calendarDates,
            IEnumerable<Route> routes, IEnumerable<Shape> shapes, IEnumerable<Stop> stops, IEnumerable<StopTime> stopTimes, IEnumerable<Trip> trips);
        Task AddVersions(IEnumerable<Models.Version> versions);
        Task<IEnumerable<Models.Version>> GetVersions();
    }
}
