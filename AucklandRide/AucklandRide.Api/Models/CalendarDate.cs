using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AucklandRide.Api.Models
{
    public class CalendarDate
    {
        public string ServiceId { get; set; }
        public DateTime Date { get; set; }
        public byte ExceptionType { get; set; }
    }
}
