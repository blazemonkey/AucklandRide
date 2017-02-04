using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Agency
    {
        [StringLength(20)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Url { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeZone { get; set; }

        [Required]
        [StringLength(50)]
        public string Lang { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }
    }

    public class AgencyMap : CsvClassMap<Agency>
    {
        public AgencyMap()
        {
            Map(m => m.Phone).Name("agency_phone");
            Map(m => m.Url).Name("agency_url");
            Map(m => m.Id).Name("agency_id");
            Map(m => m.Name).Name("agency_name");
            Map(m => m.TimeZone).Name("agency_timezone");
            Map(m => m.Lang).Name("agency_lang");
        }
    }
}
