using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Grid;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Linq;
namespace SMCISD.Student360.Persistence.Queries
{
    public interface ISchoolsQueries : IGridFilter { }

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
    }
}
