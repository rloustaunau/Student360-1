using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Grid 
{
    public interface IGridFilter
    {
        public Task<IEnumerable<object>> GetGridFilter(IPrincipal currentUser);
    }
}
