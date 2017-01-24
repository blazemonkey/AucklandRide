using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Calendar
    {
        public string ServiceId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }

    public sealed class CalendarMap : CsvClassMap<Calendar>
    {
        public CalendarMap()
        {
            Map(m => m.ServiceId).Name("service_id");
            Map(m => m.StartDate).Name("start_date");
            Map(m => m.EndDate).Name("end_date");
            Map(m => m.Monday).Name("monday");
            Map(m => m.Tuesday).Name("tuesday");
            Map(m => m.Wednesday).Name("wednesday");
            Map(m => m.Thursday).Name("thursday");
            Map(m => m.Friday).Name("friday");
            Map(m => m.Saturday).Name("saturday");
            Map(m => m.Sunday).Name("sunday");
        }
    }
}
