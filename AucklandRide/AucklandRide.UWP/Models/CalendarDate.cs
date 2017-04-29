using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.UWP.Models
{
    public class CalendarDate
    {
        public string ServiceId { get; set; }
        public DateTime Date { get; set; }
        public byte ExceptionType { get; set; }
    }
}
