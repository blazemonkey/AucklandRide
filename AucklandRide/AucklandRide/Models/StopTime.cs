using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class StopTime
    {
        public string TripId { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureTime { get; set; }
        public string StopId { get; set; }
        public int StopSequence { get; set; }
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
            Map(m => m.ArrivalTime).Name("arrival_time");
            Map(m => m.DepartureTime).Name("departure_time");
            Map(m => m.StopId).Name("stop_id");
            Map(m => m.StopSequence).Name("stop_sequence");
            Map(m => m.StopHeadsign).Name("stop_headsign");
            Map(m => m.PickupType).Name("pickup_type");
            Map(m => m.DropOffType).Name("drop_off_type");
            Map(m => m.ShapeDistance).Name("shape_dist_traveled");
        }
    }
}
