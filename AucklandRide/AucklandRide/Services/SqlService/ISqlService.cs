using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Services.SqlService
{
    public interface ISqlService
    {
        Task AddVersions(IEnumerable<Models.Version> versions);
        Task<IEnumerable<Models.Version>> GetVersions();
    }
}
