using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class Logging
    {
        public int Id { get; set; }
        
        public DateTime DateLogged { get; set; }
        public byte State { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }        

        public Logging(string method, LoggingState state, string message = "")
        {
            DateLogged = DateTime.UtcNow;
            Method = method;
            State = (byte)state;
            Message = message;
        }
    }

    public enum LoggingState
    {
        Started = 0,
        Completed = 1,
        Exception = 2
    }
}
