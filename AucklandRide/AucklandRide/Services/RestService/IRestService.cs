using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.RestService
{
    public interface IRestService
    {
        Task<IEnumerable<Models.Version>> GetVersions();
    }
}
