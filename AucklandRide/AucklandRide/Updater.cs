using AucklandRide.Services.WebClientService;
using AucklandRide.Updater.Services.RestService;
using AucklandRide.Updater.Services.SqlService;
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
        public ISqlService SqlService { get; set; }
        [Dependency]
        public IWebClientService WebClientService { get; set; }

        public async Task Run()
        {
            var latestVers = await RestService.GetVersions();
            var dbVers = await SqlService.GetVersions();

            var updateReq = IsNewVersionDetected(dbVers, latestVers);
        }

        public bool IsNewVersionDetected(IEnumerable<Models.Version> dbVers, IEnumerable<Models.Version> latestVers)
        {
            if (latestVers == null)
                throw new Exception("Cannot retrieve Versions from Auckland Transport API");
            
            if (dbVers.Any())
            {
                var isEqual = latestVers.SequenceEqual(dbVers);
                return isEqual;
            }
            else
                return true;            
        }
    }
}
