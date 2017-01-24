using AucklandRide.Updater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Services.WebClientService
{
    public interface IWebClientService
    {
        Task<IEnumerable<Route>> GetRoutes();
    }
}
