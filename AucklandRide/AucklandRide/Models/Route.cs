using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Route
    {
        public string Id { get; set; }
        public string AgencyId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public byte Type { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
    }

    public class RouteMap : CsvClassMap<Route>
    {
        public RouteMap()
        {
            Map(x => x.Id).Name("route_id");
            Map(x => x.AgencyId).Name("agency_id");
            Map(x => x.ShortName).Name("route_short_name");
            Map(x => x.LongName).Name("route_long_name");
            Map(x => x.Type).Name("route_type");
            Map(x => x.Color).Name("route_color");
            Map(x => x.TextColor).Name("route_text_color");
        }
    }
}
