using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Threading.Tasks;
using System.Security.Principal;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.Grid;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentHighestAbsenceCourseCountQueries : IGridData
    {
    }

    public class StudentHighestAbsenceCourseCountQueries : IStudentHighestAbsenceCourseCountQueries { 

        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public StudentHighestAbsenceCourseCountQueries(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }

        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            var query = _db.StudentHighestAbsenceCourseCount;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentHighestAbsenceCourseCount());
            request.AllData = true;
            return await securedQuery.ExecuteGridQuery(gridMetadata,request.AllData);
        }
    }
}
