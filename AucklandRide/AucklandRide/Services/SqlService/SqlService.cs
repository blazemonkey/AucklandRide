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
