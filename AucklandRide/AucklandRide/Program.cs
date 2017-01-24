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
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                using (var container = new UnityContainer())
                {
                    container.RegisterType<IRestService, RestService>();
                    container.RegisterType<IWebClientService, WebClientService>();
                    var updater = container.Resolve<Updater>();
                    await updater.Run();
                }
            }).GetAwaiter().GetResult();
        }
    }
}
