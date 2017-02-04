namespace AucklandRide.Updater.Services.SqlService
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Models;

    public partial class AucklandRideContext : DbContext
    {
        public AucklandRideContext()
            : base("name=AucklandRideContext")
        {
        }

        public virtual DbSet<Agency> Agencies { get; set; }
        public virtual DbSet<CalendarDate> CalendarDates { get; set; }
        public virtual DbSet<Calendar> Calendars { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Shape> Shapes { get; set; }
        public virtual DbSet<Stop> Stops { get; set; }
        public virtual DbSet<StopTime> StopTimes { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<Models.Version> Versions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agency>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Agency>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Agency>()
                .Property(e => e.Url)
                .IsUnicode(false);

            modelBuilder.Entity<Agency>()
                .Property(e => e.TimeZone)
                .IsUnicode(false);

            modelBuilder.Entity<Agency>()
                .Property(e => e.Lang)
                .IsUnicode(false);

            modelBuilder.Entity<Agency>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<CalendarDate>()
                .Property(e => e.ServiceId)
                .IsUnicode(false);

            modelBuilder.Entity<Calendar>()
                .Property(e => e.ServiceId)
                .IsUnicode(false);

            modelBuilder.Entity<Route>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Route>()
                .Property(e => e.AgencyId)
                .IsUnicode(false);

            modelBuilder.Entity<Route>()
                .Property(e => e.ShortName)
                .IsUnicode(false);

            modelBuilder.Entity<Route>()
                .Property(e => e.LongName)
                .IsUnicode(false);

            modelBuilder.Entity<Route>()
                .Property(e => e.Color)
                .IsUnicode(false);

            modelBuilder.Entity<Route>()
                .Property(e => e.TextColor)
                .IsUnicode(false);

            modelBuilder.Entity<Shape>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Shape>()
                .Property(e => e.Latitude)
                .HasPrecision(10, 5);

            modelBuilder.Entity<Shape>()
                .Property(e => e.Longitude)
                .HasPrecision(10, 5);

            modelBuilder.Entity<Stop>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Stop>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Stop>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Stop>()
                .Property(e => e.Latitude)
                .HasPrecision(10, 5);

            modelBuilder.Entity<Stop>()
                .Property(e => e.Longitude)
                .HasPrecision(10, 5);

            modelBuilder.Entity<StopTime>()
                .Property(e => e.TripId)
                .IsUnicode(false);

            modelBuilder.Entity<StopTime>()
                .Property(e => e.StopId)
                .IsUnicode(false);

            modelBuilder.Entity<StopTime>()
                .Property(e => e.StopHeadsign)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.RouteId)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.ServiceId)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.Headsign)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.BlockId)
                .IsUnicode(false);

            modelBuilder.Entity<Trip>()
                .Property(e => e.ShapeId)
                .IsUnicode(false);

            modelBuilder.Entity<Models.Version>()
                .Property(e => e.Ver)
                .IsUnicode(false);
        }
    }
}
