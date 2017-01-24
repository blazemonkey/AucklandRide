using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Shape
    {
        public string Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Sequence { get; set; }
        public int? Distance { get; set; }
    }

    public class ShapeMap : CsvClassMap<Shape>
    {
        public ShapeMap()
        {
            Map(m => m.Id).Name("shape_id");
            Map(m => m.Latitude).Name("shape_pt_lat");
            Map(m => m.Longitude).Name("shape_pt_lon");
            Map(m => m.Sequence).Name("shape_pt_sequence");
            Map(m => m.Distance).Name("shape_dist_traveled");
        }
    }
}
