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
    public class Trip
    {
        [StringLength(35)]
        public string Id { get; set; }

        [Required]
        [StringLength(35)]
        public string RouteId { get; set; }

        [Required]
        [StringLength(35)]
        public string ServiceId { get; set; }

        [Required]
        [StringLength(100)]
        public string Headsign { get; set; }

        public byte DirectionId { get; set; }

        [Required]
        [StringLength(35)]
        public string BlockId { get; set; }

        [Required]
        [StringLength(35)]
        public string ShapeId { get; set; }

        public TimeSpan? FirstArrivalTime { get; set; }

        public TimeSpan? LastDepartureTime { get; set; }
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
