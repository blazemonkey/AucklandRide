using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace AucklandRide.Updater.Models
{
    public class Version
    {
        [Key]
        [Column("Version", Order = 0)]
        [StringLength(25)]
        [JsonProperty(PropertyName = "version")]
        public string Ver { get; set; }
        [Key]
        [Column(Order = 1)]
        [JsonProperty(PropertyName = "startdate")]
        public DateTime StartDate { get; set; }
        [Key]
        [Column(Order = 2)]
        [JsonProperty(PropertyName = "enddate")]
        public DateTime EndDate { get; set; }
    }
}
