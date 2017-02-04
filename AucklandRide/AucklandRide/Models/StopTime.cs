using AucklandRide.Updater.Services.WebClientService;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class StopTime
    {
        [Key, Column(Order = 0)]
        [StringLength(35)]
        public string TripId { get; set; }

        public TimeSpan ArrivalTime { get; set; }

        public TimeSpan DepartureTime { get; set; }

        [Required]
        [StringLength(20)]
        public string StopId { get; set; }

        [Key, Column(Order =1)]
        public int StopSequence { get; set; }

        [StringLength(100)]
        public string StopHeadsign { get; set; }

        public int? PickupType { get; set; }

        public int? DropOffType { get; set; }

        public int? ShapeDistance { get; set; }
    }

    public sealed class StopTimeMap : CsvClassMap<StopTime>
    {
        public StopTimeMap()
        {
            Map(m => m.TripId).Name("trip_id");
            Map(m => m.ArrivalTime).Name("arrival_time").TypeConverter<TimeSpanConverter>().TypeConverterOption("hh':'mm':'ss");
            Map(m => m.DepartureTime).Name("departure_time").TypeConverter<TimeSpanConverter>().TypeConverterOption("hh':'mm':'ss");
            Map(m => m.StopId).Name("stop_id");
            Map(m => m.StopSequence).Name("stop_sequence");
            Map(m => m.StopHeadsign).Name("stop_headsign");
            Map(m => m.PickupType).Name("pickup_type");
            Map(m => m.DropOffType).Name("drop_off_type");
            Map(m => m.ShapeDistance).Name("shape_dist_traveled");
        }
    }
}
