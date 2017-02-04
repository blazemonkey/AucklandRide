using AucklandRide.Updater.Models;
using EntityFramework.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.SqlService
{
    public class SqlService : ISqlService
    {
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
            var db = GetDbContext();
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Agencies");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Calendars");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE CalendarDates");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Routes");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Shapes");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Stops");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE StopTimes");
            await db.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Trips");
            EFBatchOperation.For(db, db.Agencies).InsertAll(agencies);
            EFBatchOperation.For(db, db.Calendars).InsertAll(calendars);
            EFBatchOperation.For(db, db.CalendarDates).InsertAll(calendarDates);
            EFBatchOperation.For(db, db.Routes).InsertAll(routes);
            EFBatchOperation.For(db, db.Shapes).InsertAll(shapes);
            EFBatchOperation.For(db, db.Stops).InsertAll(stops);
            EFBatchOperation.For(db, db.StopTimes).InsertAll(stopTimes);
            EFBatchOperation.For(db, db.Trips).InsertAll(trips);
        }

        //private async Task<DbContext> BatchInsert<T>(IEnumerable<T> collection, DbSet set, int batchSize, DbContext db)
        //{
        //    var count = 0;
        //    for (var i = 0; i < collection.Count(); i = i + batchSize)
        //    {
        //        count++;
        //        var sub = collection.Skip(i).Take(batchSize);
        //        set.AddRange(sub);
        //        await db.SaveChangesAsync();

        //        if (count == 10)
        //        {

        //        }
        //    }
        //}


        public async Task AddVersions(IEnumerable<Models.Version> versions)
        {
            using (var db = GetDbContext())
            {
                await OpenAsync(db);
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
    }
}
