using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Trip
    {
        public string Id { get; set; }
        public string RouteId { get; set; }
        public string ServiceId { get; set; }
        public string Headsign { get; set; }
        public int DirectionId { get; set; }
        public string BlockId { get; set; }
        public string ShapeId { get; set; }
        public string FirstArrivalTime { get; set; }
        public string LastDepartureTime { get; set; }
    }

    public sealed class TripMap : CsvClassMap<Trip>
    {
        public TripMap()
        {
            Map(m => m.BlockId).Name("block_id");
            Map(m => m.RouteId).Name("route_id");
            Map(m => m.DirectionId).Name("direction_id");
            Map(m => m.Headsign).Name("trip_headsign");
            Map(m => m.ShapeId).Name("shape_id");
            Map(m => m.ServiceId).Name("service_id");
            Map(m => m.Id).Name("trip_id");
        }
    }
}
