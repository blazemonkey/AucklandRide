using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models
{
    public class StopTime
    {
        public string TripId { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string StopId { get; set; }
        public int StopSequence { get; set; }
        public string StopHeadsign { get; set; }
        public int? PickupType { get; set; }
        public int? DropOffType { get; set; }
        public int? ShapeDistance { get; set; }
        public string StopName { get; set; }
        public decimal StopLatitude { get; set; }
        public decimal StopLongitude { get; set; }
        public string StopRegionName { get; set; }
    }
}
