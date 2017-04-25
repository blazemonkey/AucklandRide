using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models
{
    public class Trip
    {
        public string Id { get; set; }
        public string RouteId { get; set; }
        public string ServiceId { get; set; }
        public string Headsign { get; set; }
        public byte DirectionId { get; set; }
        public string BlockId { get; set; }
        public string ShapeId { get; set; }
        public TimeSpan? FirstArrivalTime { get; set; }
        public TimeSpan? LastDepartureTime { get; set; }
        public List<StopTime> StopTimes { get; set; }
    }
}
