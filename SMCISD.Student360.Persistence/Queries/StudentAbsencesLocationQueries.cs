using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentAbsencesLocationQueries : IGridData {}

    public class StudentAbsencesLocationQueries : IStudentAbsencesLocationQueries

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public StudentAbsencesLocationQueries(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }
       
        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            var query = _db.StudentAbsencesLocation;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser); 
            var gridMetadata = request.ProcessMetadata(new StudentAbsencesLocation());

            return await securedQuery.ExecuteGridQuery(gridMetadata, true);
        }
    }
}
