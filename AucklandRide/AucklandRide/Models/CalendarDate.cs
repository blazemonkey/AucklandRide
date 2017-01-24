using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class CalendarDate
    {
        public string ServiceId { get; set; }
        public string Date { get; set; }
        public byte ExceptionType { get; set; }
    }

    public sealed class CalendarDateMap : CsvClassMap<CalendarDate>
    {
        public CalendarDateMap()
        {
            Map(m => m.ServiceId).Name("service_id");
            Map(m => m.Date).Name("date");
            Map(m => m.ExceptionType).Name("exception_type");
        }
    }
}
