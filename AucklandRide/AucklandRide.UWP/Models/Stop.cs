using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models
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
        public int ParentStation { get; set; }
        public string RegionName { get; set; }
    }
}
