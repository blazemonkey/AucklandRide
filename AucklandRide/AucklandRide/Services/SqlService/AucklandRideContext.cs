namespace AucklandRide.Updater.Services.SqlService
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AucklandRideContext : DbContext
    {
        public AucklandRideContext()
            : base("name=AucklandRideContext")
        {
        }

        public virtual DbSet<Models.Version> Versions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Version>()
                .Property(e => e.Ver)
                .IsUnicode(false);
        }
    }
}
