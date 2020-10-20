using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Grid
{
    public interface IGridData
    {
        public Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser);
    }
}
