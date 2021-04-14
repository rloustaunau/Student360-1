using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Grid;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Models;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface ISchoolsQueries : IGridFilter {
        Task<EducationOrganizationInformation> GetSchoolInfo(int edOrgId);
    }

    public class SchoolsQueries : ISchoolsQueries
    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public SchoolsQueries(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }
        public async Task<IEnumerable<object>> GetGridFilter(IPrincipal currentUser)
        {
            var query = _db.Schools;
            return _auth.ApplySecurity(query, query, currentUser).ToList().GroupBy(x => new { x.SchoolId, x.LocalEducationAgencyId, x.NameOfInstitution }).Select(x => new {
                x.Key.SchoolId,
                x.Key.LocalEducationAgencyId,
                x.Key.NameOfInstitution,
                ChildOptions = x.Select(g => new { Id = g.GradeLevel, Value = g.GradeLevel }) // For GradeLevel Filter
            });
        }

        public async Task<EducationOrganizationInformation> GetSchoolInfo(int edOrgId)
        {
            return await _db.EducationOrganizationInformation.FirstOrDefaultAsync(x => x.EducationOrganizationId == edOrgId);
        }



    }
}
