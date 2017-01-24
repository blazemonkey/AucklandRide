using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Stop
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string ZoneId { get; set; }
        public int Code { get; set; }
        public byte LocationType { get; set; }
        public string ParentStation { get; set; }
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
