using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Schools
{
    public interface ISchoolsService : IGridFilter {
        Task<EducationOrganiztionInformationModel> GetSchoolInfo(int edOrgId);
    }

    public class SchoolsService : ISchoolsService
    {
        private readonly ISchoolsQueries _queries;
        public SchoolsService(ISchoolsQueries queries)
        {
            _queries = queries;
        }

        public async Task<IEnumerable<object>> GetGridFilter(IPrincipal currentUser) => await _queries.GetGridFilter(currentUser);

        public async Task<EducationOrganiztionInformationModel> GetSchoolInfo(int edOrgId)
        {
            var entity = await _queries.GetSchoolInfo(edOrgId);

            return MapEdOrgInfoEntityToEdOrgInfoModel(entity);
        }
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


        private Persistence.Models.EducationOrganizationInformation MapEdOrgInfoModelToEdOrgInfoEntity(EducationOrganiztionInformationModel model)
        {
            return new Persistence.Models.EducationOrganizationInformation
            {
                StreetNumberName = model.StreetNumberName,
                City = model.City,
                State = model.State,
                EducationOrganizationId = model.EducationOrganizationId,
                Phone = model.Phone,
                PostalCode = model.PostalCode,
                PrincipalFirstName = model.PrincipalFirstName,
                PrincipalLastSurname = model.PrincipalLastSurname,
                PrincipalMiddleName = model.PrincipalMiddleName,
                NameOfInstitution = model.NameOfInstitution,
                ShortNameOfInstitution = model.ShortNameOfInstitution

            };
        }

        private EducationOrganiztionInformationModel MapEdOrgInfoEntityToEdOrgInfoModel(Persistence.Models.EducationOrganizationInformation entity)
        {
            return new EducationOrganiztionInformationModel
            {
                StreetNumberName = entity.StreetNumberName,
                City = entity.City,
                State = entity.State,
                EducationOrganizationId = entity.EducationOrganizationId,
                Phone = entity.Phone,
                PostalCode = entity.PostalCode,
                PrincipalFirstName = entity.PrincipalFirstName,
                PrincipalLastSurname = entity.PrincipalLastSurname,
                PrincipalMiddleName = entity.PrincipalMiddleName,
                NameOfInstitution = entity.NameOfInstitution,
                ShortNameOfInstitution = entity.ShortNameOfInstitution
            };
        }
    }
}
