using AucklandRide.Api.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AucklandRide.Api.Services.SqlService
{
    public class SqlService : ISqlService
    {
        private string _connString = ConfigurationManager.ConnectionStrings["AucklandRideContext"].ConnectionString;

        public async Task<List<Agency>> GetAgencies()
        {
            Action<List<Agency>, SqlDataReader> addAction = (agencies, reader) =>
            {
                var agency = new Agency()
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Url = reader.GetString(2),
                    TimeZone = reader.GetString(3),
                    Lang = reader.GetString(4),
                    Phone = reader.GetString(5)
                };
                agencies.Add(agency);
            };

            return await GetAll("SELECT * FROM Agencies", addAction);
        }

        public async Task<Calendar> GetCalendarByServiceId(string serviceId)
        {
            Func<Calendar, SqlDataReader, Calendar> addCalendarAction = (a, reader) =>
            {
                a = new Calendar()
                {
                    ServiceId = reader.GetString(0),
                    StartDate = reader.GetDateTime(1),
                    EndDate = reader.GetDateTime(2),
                    Monday = reader.GetBoolean(3),
                    Tuesday = reader.GetBoolean(4),
                    Wednesday = reader.GetBoolean(5),
                    Thursday = reader.GetBoolean(6),
                    Friday = reader.GetBoolean(7),
                    Saturday = reader.GetBoolean(8),
                    Sunday = reader.GetBoolean(9)
                };

                return a;
            };

            Action<List<CalendarDate>, SqlDataReader> addDatesAction = (cds, reader) =>
            {
                var cd = new CalendarDate()
                {
                    ServiceId = reader.GetString(0),
                    Date = reader.GetDateTime(1),
                    ExceptionType = reader.GetByte(2),
                };
                cds.Add(cd);
            };

            var calendar = await GetById("SELECT * FROM Calendars", "ServiceId", serviceId, addCalendarAction);
            var dates = await GetAllById("SELECT * FROM CalendarDates", "ServiceId", serviceId, addDatesAction);
            calendar.CalendarDates = dates;

            return calendar;
        }

        public async Task<List<Route>> GetRoutes()
        {
            Action<List<Route>, SqlDataReader> addAction = (routes, reader) =>
            {
                var route = new Route()
                {
                    Id = reader.GetString(0),
                    AgencyId = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    LongName = reader.GetString(3),
                    Type = reader.GetByte(4),
                    Color = SafeGetString(reader, 5),
                    TextColor = SafeGetString(reader, 6),
                    AgencyName = reader.GetString(7)
                };
                routes.Add(route);
            };

            return await GetAll("SELECT r.Id, r.AgencyId, r.ShortName, r.LongName, r.Type, r.Color, r.TextColor, a.Name " +
                "FROM Routes AS r JOIN Versions AS v ON r.Version = v.Version JOIN Agencies AS a ON r.AgencyId = a.Id WHERE r.Version = v.Version ORDER BY ShortName", addAction);
        }

        public async Task<Route> GetRouteById(string routeId)
        {
            Func<Route, SqlDataReader, Route> addAction = (s, reader) =>
            {
                s = new Route()
                {
                    Id = reader.GetString(0),
                    AgencyId = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    LongName = reader.GetString(3),
                    Type = reader.GetByte(4),
                    Color = SafeGetString(reader, 5),
                    TextColor = SafeGetString(reader, 6),
                    AgencyName = reader.GetString(7)
                };

                return s;
            };

            Action<List<Trip>, SqlDataReader> addTripsAction = (ts, reader) =>
            {
                var t = new Trip()
                {
                    Id = reader.GetString(0),
                    RouteId = reader.GetString(1),
                    ServiceId = reader.GetString(2),
                    Headsign = reader.GetString(3),
                    DirectionId = reader.GetByte(4),
                    BlockId = reader.GetString(5),
                    ShapeId = reader.GetString(6),
                    FirstArrivalTime = reader.GetTimeSpan(7),
                    LastDepartureTime = reader.GetTimeSpan(8)
                };
                ts.Add(t);
            };

            var route = await GetById("SELECT r.Id, r.AgencyId, r.ShortName, r.LongName, r.Type, r.Color, r.TextColor, a.Name " +
                "FROM Routes AS r JOIN Agencies AS a ON r.AgencyId = a.Id", "r.Id", routeId, addAction);

            var trips = await GetAllById("SELECT * FROM Trips", "RouteId", routeId, addTripsAction, "ORDER BY FirstArrivalTime, LastDepartureTime");
            route.Trips = trips;

            return route;
        }

        public async Task<List<Shape>> GetShapesById(string shapeId)
        {
            Action<List<Shape>, SqlDataReader> addAction = (ss, reader) =>
            {
                var s = new Shape()
                {
                    Id = reader.GetString(0),
                    Latitude = reader.GetDecimal(1),
                    Longitude = reader.GetDecimal(2),
                    Sequence = reader.GetInt32(3),
                    Distance = SafeGetInt32(reader, 4)
                };
                ss.Add(s);
            };

            var shapes = await GetAllById("SELECT * FROM Shapes", "Id", shapeId, addAction);
            return shapes;
        }

        public async Task<List<Stop>> GetStops()
        {
            Action<List<Stop>, SqlDataReader> addAction = (stops, reader) =>
            {
                var stop = new Stop()
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Code = reader.GetInt32(2),
                    RegionName = reader.GetString(3)
                };
                stops.Add(stop);
            };

            return await GetAll("SELECT s.Id, s.Name, s.Code, sr.Name AS RegionName FROM Stops AS s JOIN StopRegions AS sr ON s.Id = sr.StopId ORDER BY s.Code", addAction);
        }

        public async Task<Stop> GetStopById(string stopId)
        {
            Func<Stop, SqlDataReader, Stop> addAction = (s, reader) =>
            {
                s = new Stop()
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Latitude = reader.GetDecimal(3),
                    Longitude = reader.GetDecimal(4),
                    ZoneId = SafeGetString(reader, 5),
                    Code = reader.GetInt32(6),
                    LocationType = reader.GetByte(7),
                    ParentStation = SafeGetInt32(reader, 8),
                    RegionName = reader.GetString(9)
                };

                return s;
            };

            var stop = await GetById("SELECT s.*, sr.Name As RegionName FROM Stops AS s JOIN StopRegions AS sr ON s.Id = sr.StopId", "s.Id", stopId, addAction);

            Action<List<Route>, SqlDataReader> addRoutesAction = (rs, reader) =>
            {
                var route = new Route()
                {
                    Id = reader.GetString(0),
                    AgencyId = reader.GetString(1),
                    ShortName = reader.GetString(2),
                    LongName = reader.GetString(3),
                    Type = reader.GetByte(4),
                    Color = SafeGetString(reader, 5),
                    TextColor = SafeGetString(reader, 6)
                };
                rs.Add(route);
            };

            var routes = await GetAllById("SELECT DISTINCT r.* FROM StopTimes AS st JOIN Trips AS t ON st.TripId = t.Id JOIN Routes AS r ON t.RouteId = r.Id", "st.StopId", stop.Id, addRoutesAction);
            stop.RoutesWithStop = routes;

            return stop;
        }

        public async Task<List<StopTime>> GetStopTimesByTripId(string tripId)
        {
            Action<List<StopTime>, SqlDataReader> addAction = (sts, reader) =>
            {
                var st = new StopTime()
                {
                    TripId = reader.GetString(0),
                    ArrivalTime = reader.GetTimeSpan(1),
                    DepartureTime = reader.GetTimeSpan(2),
                    StopId = reader.GetString(3),
                    StopSequence = reader.GetInt32(4),
                    StopHeadsign = SafeGetString(reader, 5),
                    PickupType = SafeGetInt32(reader, 6),
                    DropOffType = SafeGetInt32(reader, 7),
                    ShapeDistance = SafeGetInt32(reader, 8),
                    StopName = reader.GetString(9),
                    StopLatitude = reader.GetDecimal(10),
                    StopLongitude = reader.GetDecimal(11),
                    StopRegionName = reader.GetString(12)
                };
                sts.Add(st);
            };

            var stopTimes = await GetAllById("SELECT st.*, s.Name, s.Latitude, s.Longitude, sr.Name FROM StopTimes AS st JOIN Stops AS s ON st.StopId = s.Id JOIN StopRegions AS sr ON s.Id = sr.StopId", 
                "TripId", tripId, addAction, "ORDER BY StopSequence");
            return stopTimes;
        }

        private async Task<List<T>> GetAll<T>(string sql, Action<List<T>, SqlDataReader> addAction)
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    var collection = new List<T>();
                    await conn.OpenAsync();

                    var command = new SqlCommand(sql, conn);
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        addAction.Invoke(collection, reader);
                    }

                    conn.Close();

                    return collection;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        private async Task<List<T>> GetAllById<T>(string sql, string key, string id, Action<List<T>, SqlDataReader> addAction, string orderBy = "")
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    var collection = new List<T>();
                    await conn.OpenAsync();

                    var command = new SqlCommand(string.Format("{0} WHERE {1} = '{2}' {3}", sql, key, id, orderBy), conn);
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        addAction.Invoke(collection, reader);
                    }

                    conn.Close();

                    return collection;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<T> GetById<T>(string sql, string key, string id, Func<T, SqlDataReader, T> addAction) where T : new()
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    var model = new T();
                    await conn.OpenAsync();

                    var command = new SqlCommand(string.Format("{0} WHERE {1} = '{2}'", sql, key, id), conn);
                    var reader = await command.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        model = addAction.Invoke(model, reader);
                    }

                    conn.Close();

                    return model;
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private static string SafeGetString(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            return string.Empty;
        }

        private static int SafeGetInt32(SqlDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetInt32(colIndex);
            return 0;
        }
    }
}
