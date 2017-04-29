using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AucklandRide.Api.Models
{
    public class Calendar
    {
        public string ServiceId { get; set; }
        public DateTime StartDate { get; set; }       
        public DateTime EndDate { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public List<CalendarDate> CalendarDates { get; set; }
    }
}
