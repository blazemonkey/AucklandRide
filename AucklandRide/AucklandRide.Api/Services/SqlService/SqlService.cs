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

        public async Task<List<Stop>> GetStops()
        {
            Action<List<Stop>, SqlDataReader> addAction = (stops, reader) =>
            {
                var stop = new Stop()
                {
                    Id = reader.GetString(0),
                    Name = reader.GetString(1),
                    RegionName = reader.GetString(2)
                };
                stops.Add(stop);
            };

            return await GetAll("SELECT s.Id, s.Name, sr.Name AS RegionName FROM Stops AS s JOIN StopRegions AS sr ON s.Id = sr.StopId", addAction);
        }

        public async Task<Stop> GetStopById(string stopId)
        {
            Func<Stop, SqlDataReader, Stop> addAction = (stop, reader) =>
            {
                stop = new Stop()
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

                return stop;
            };

            return await GetById("SELECT s.*, sr.Name As RegionName FROM Stops AS s JOIN StopRegions AS sr ON s.Id = sr.StopId", "s.Id", stopId, addAction);
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

        private async Task<T> GetById<T>(string sql, string key, string id, Func<T, SqlDataReader, T> addAction) where T : new()
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    var model = new T();
                    await conn.OpenAsync();

                    var command = new SqlCommand(string.Format("{0} WHERE {1} = {2}", sql, key, id), conn);
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
