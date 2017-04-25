using AucklandRide.Updater.Models;
using EntityFramework.Utilities;
using FastMember;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.SqlService
{
    public class SqlService : ISqlService
    {
        private string _connString = ConfigurationManager.ConnectionStrings["AucklandRideContext"].ConnectionString;

        public virtual AucklandRideContext GetDbContext()
        {
            return new AucklandRideContext();
        }

        public async virtual Task OpenAsync(DbContext db)
        {
            await db.Database.Connection.OpenAsync();
        }

        public async Task DeleteAndReplaceAll(IEnumerable<Agency> agencies, IEnumerable<Calendar> calendars, IEnumerable<CalendarDate> calendarDates,
            IEnumerable<Route> routes, IEnumerable<Shape> shapes, IEnumerable<Stop> stops, IEnumerable<StopTime> stopTimes, IEnumerable<Trip> trips)
        {
            using (var conn = new SqlConnection(_connString))
            {
                await conn.OpenAsync();
                var trans = conn.BeginTransaction();

                try
                {
                    var cmd = new SqlCommand()
                    {
                        CommandTimeout = 60 * 10,
                        Connection = conn,
                        Transaction = trans
                    };

                    await DeleteAndInsert(cmd, "Agencies", agencies);
                    await DeleteAndInsert(cmd, "Calendars", calendars);
                    await DeleteAndInsert(cmd, "CalendarDates", calendarDates);
                    await DeleteAndInsert(cmd, "Routes", routes);
                    await DeleteAndInsert(cmd, "Shapes", shapes);
                    await DeleteAndInsert(cmd, "Stops", stops);
                    await DeleteAndInsert(cmd, "StopTimes", stopTimes);
                    await DeleteAndInsert(cmd, "Trips", trips);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    await AddLogging(new Logging("DeleteAndReplaceAll", LoggingState.Exception, ex.Message));
                }
                finally
                {
                    trans.Dispose();
                }
            }
        }

        private async Task DeleteAndInsert<T>(SqlCommand cmd, string tableName, IEnumerable<T> collection)
        {
            if (collection == null)
                return;

            await AddLogging(new Logging("DeleteAndInsert" + tableName, LoggingState.Started, "Rows: " + collection.Count()));
            var columns = typeof(T).GetProperties().Select(x => x.Name).ToArray();
            var sql = string.Format("TRUNCATE TABLE {0}", tableName);
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();

            using (var bcp = new SqlBulkCopy(cmd.Connection, SqlBulkCopyOptions.TableLock, cmd.Transaction))
            using (var reader = ObjectReader.Create(collection, columns))
            {
                bcp.BulkCopyTimeout = 60 * 120;
                bcp.BatchSize = 100000;
                bcp.DestinationTableName = tableName;
                bcp.WriteToServer(reader);
            }
            await AddLogging(new Logging("DeleteAndInsert" + tableName, LoggingState.Completed));
        }

        public async Task UpdateTripsTime()
        {
            using (var conn = new SqlConnection(_connString))
            {
                await conn.OpenAsync();
                var trans = conn.BeginTransaction();

                try
                {                                        
                    var sql = "UPDATE t " + 
                        "SET t.FirstArrivalTime = st.ArrivalTime " + 
                        "FROM Trips AS t " + 
                        "JOIN StopTimes AS st " + 
                        "ON t.Id = st.TripId " + 
                        "WHERE StopSequence = 1";
                    var cmd = new SqlCommand(sql, conn, trans)
                    {
                        CommandTimeout = 60 * 10
                    };
                    await cmd.ExecuteNonQueryAsync();

                    sql = "UPDATE t " +
                        "SET t.LastDepartureTime = sta.DepartureTime " +
                        "FROM StopTimes sta " + 
                        "LEFT OUTER JOIN StopTimes stb " +
                        "ON sta.TripId = stb.TripId AND sta.StopSequence < stb.StopSequence " +
                        "JOIN Trips AS t " +
                        "ON sta.TripId = t.Id " +
                        "WHERE stb.TripId IS NULL; ";
                    cmd = new SqlCommand(sql, conn, trans)
                    {
                        CommandTimeout = 60 * 10
                    };
                    await cmd.ExecuteNonQueryAsync();

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    await AddLogging(new Logging("UpdateTripsTime", LoggingState.Exception, ex.Message));
                }
                finally
                {
                    trans.Dispose();
                }
            }
        }

        public async Task AddStopRegions(IEnumerable<StopRegion> stopRegions)
        {
            using (var db = GetDbContext())
            {
                await OpenAsync(db);
                db.StopRegions.AddRange(stopRegions);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StopRegion>> GetStopRegions()
        {
            using (var db = GetDbContext())
            {
                await OpenAsync(db);
                return await db.StopRegions.ToListAsync();
            }
        }

        public async Task AddVersions(IEnumerable<Models.Version> versions)
        {
            using (var db = GetDbContext())
            {
                await OpenAsync(db);
                db.Versions.RemoveRange(db.Versions);
                db.Versions.AddRange(versions);
                await db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Models.Version>> GetVersions()
        {
            using (var db = GetDbContext())
            {
                await OpenAsync(db);
                return await db.Versions.ToListAsync();
            }
        }        

        public async Task AddLogging(Logging logging)
        {
            using (var db = GetDbContext())
            {
                await OpenAsync(db);
                db.Loggings.Add(logging);
                await db.SaveChangesAsync();
            }
        }
    }
}
