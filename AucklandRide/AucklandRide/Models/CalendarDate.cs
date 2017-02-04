using AucklandRide.Updater.Services.WebClientService;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class CalendarDate
    {
        [Key, Column(Order = 0)]
        [StringLength(35)]
        public string ServiceId { get; set; }

        [Key, Column(Order = 1, TypeName = "date")]
        public DateTime Date { get; set; }

        public byte ExceptionType { get; set; }
    }

    public sealed class CalendarDateMap : CsvClassMap<CalendarDate>
    {
        public CalendarDateMap()
        {
            Map(m => m.ServiceId).Name("service_id");
            Map(m => m.Date).Name("date").TypeConverter<DateTimeConverter>().TypeConverterOption("yyyyMMdd");
            Map(m => m.ExceptionType).Name("exception_type");
        }
    }
}
