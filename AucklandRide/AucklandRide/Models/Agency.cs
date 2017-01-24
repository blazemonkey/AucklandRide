using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Agency
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string TimeZone { get; set; }
        public string Lang { get; set; }
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
