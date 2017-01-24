using AucklandRide.Services.WebClientService;
using AucklandRide.Updater.Services.RestService;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater
{
    public class Updater
    {
        [Dependency]
        public IRestService RestService { get; set; }
        [Dependency]
        public IWebClientService WebClientService { get; set; }

        public async Task Run()
        {
            await WebClientService.GetRoutes();
            await RestService.GetVersion();
        }
    }
}
