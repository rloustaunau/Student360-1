using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries.Auth
{
    public interface IParentUsitoSchoolIdQueries
    {
        IQueryable<ParentUsitoSchoolId> GetParentSchools(int parentUsi);  
    }

    public class ParentUsitoSchoolIdQueries : IParentUsitoSchoolIdQueries

    {
        private readonly Student360Context _db;

        public ParentUsitoSchoolIdQueries(Student360Context db)
        {
            _db = db;
        }
        public IQueryable<ParentUsitoSchoolId> GetParentSchools(int parentUsi) => _db.ParentUsitoSchoolId.Where(x => x.ParentUsi == parentUsi);
    }
}
