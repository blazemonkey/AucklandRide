using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class RestWrapper<T>
    {
        public string Status { get; set; }
        public List<T> Response { get; set; }
        public string Error { get; set; }
    }
}
