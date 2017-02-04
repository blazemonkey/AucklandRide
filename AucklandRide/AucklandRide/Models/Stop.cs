using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Stop
    {
        [StringLength(20)]
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public int? ZoneId { get; set; }

        public int Code { get; set; }

        public byte LocationType { get; set; }

        public int? ParentStation { get; set; }
    }

    public sealed class StopMap : CsvClassMap<Stop>
    {
        public StopMap()
        {
            Map(m => m.Latitude).Name("stop_lat");
            Map(m => m.ZoneId).Name("zone_id");
            Map(m => m.Longitude).Name("stop_lon");
            Map(m => m.Id).Name("stop_id");
            Map(m => m.ParentStation).Name("parent_station");
            Map(m => m.Description).Name("stop_desc");
            Map(m => m.Name).Name("stop_name");
            Map(m => m.LocationType).Name("location_type");
            Map(m => m.Code).Name("stop_code");
        }
    }
}
