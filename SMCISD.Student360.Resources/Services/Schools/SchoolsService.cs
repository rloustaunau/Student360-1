using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Schools
{
    public interface ISchoolsService : IGridFilter { }

    public class SchoolsService : ISchoolsService
    {
        private readonly ISchoolsQueries _queries;
        public SchoolsService(ISchoolsQueries queries)
        {
            _queries = queries;
        }

        public async Task<IEnumerable<object>> GetGridFilter(IPrincipal currentUser) => await _queries.GetGridFilter(currentUser);
        private Persistence.Models.Schools MapSchoolsModelToSchoolsEntity(SchoolsModel model)
        {
            return new Persistence.Models.Schools
            {
               SchoolId = model.SchoolId,
               NameOfInstitution = model.NameOfInstitution,
               LocalEducationAgencyId = model.LocalEducationAgencyId
            };
        }

        private SchoolsModel MapSchoolsEntityToSchoolsModel(Persistence.Models.Schools entity)
        {
            return new SchoolsModel
            {
                SchoolId = entity.SchoolId,
                NameOfInstitution = entity.NameOfInstitution,
                LocalEducationAgencyId = entity.LocalEducationAgencyId
            };
        }
    }
}
