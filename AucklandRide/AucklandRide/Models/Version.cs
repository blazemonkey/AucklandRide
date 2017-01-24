using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Version
    {
        [JsonProperty(PropertyName = "version")]
        public string Ver { get; set; }
        [JsonProperty(PropertyName = "startdate")]
        public DateTime StartDate { get; set; }
        [JsonProperty(PropertyName = "enddate")]
        public DateTime EndDate { get; set; }
    }
}
