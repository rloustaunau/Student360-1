using Microsoft.Extensions.Configuration;
using SMCISD.Student360.Persistence;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.People
{
    public interface IPeopleService 
    {
        Task<PeopleModel> Get(string email);
        Task<List<PeopleModel>> GetStaffByName(string name);
    }

    public class PeopleService : IPeopleService
    {
        private readonly IPeopleQueries _queries;
        private readonly IConfiguration _config;
        public PeopleService(IPeopleQueries queries, IConfiguration config)
        {
            _queries = queries;
            _config = config;
        }

        public async Task<PeopleModel> Get(string email)
        {
            var entityList = await _queries.Get(email);

            if (entityList.Count() == 0)
                throw new UnauthorizedAccessException("The user credentials are invalid. If this message persists please verify that you have access to the app.");


            return MapPeopleEntityListToPeopleModel(entityList);
        }

        public async Task<List<PeopleModel>> GetStaffByName(string name)
        {
            var entityList = await _queries.GetStaffByName(name);

            if (entityList.Count() == 0)
                throw new UnauthorizedAccessException("No data");

            List<PeopleModel> models = new List<PeopleModel>();

            foreach (var entity in entityList) {
                var model = MapPeopleEntityToPeopleModel(entity);
                models.Add(model);
            }

            return models;
        }

        private PeopleModel MapPeopleEntityListToPeopleModel(List<Persistence.Models.People> entityList)
        {

            return (from list in entityList
                group list by list.ElectronicMailAddress into g
                select new PeopleModel {
                    Usi = g.FirstOrDefault().Usi,
                    UniqueId= g.FirstOrDefault().UniqueId,
                    FirstName = g.FirstOrDefault().FirstName,
                    LastSurname = g.FirstOrDefault().LastSurname,
                    Role = g.OrderBy(x => x.LevelId).FirstOrDefault().PositionTitle,
                    AccessLevel = g.OrderBy(x => x.LevelId).FirstOrDefault().AccessLevel,
                    LevelId = g.OrderBy(x => x.LevelId).FirstOrDefault().LevelId,
                    ElectronicMailAddress = g.Key,
                    EdOrgAssociations = g.GroupBy(x => new { x.PersonType, x.SchoolId, x.LocalEducationAgencyId, x.PositionTitle }).Select(p => new EdOrgAssociation // removing possible duplicates
                    {
                        EducationOrganizationId = p.Key.SchoolId.HasValue ? p.Key.SchoolId.Value : p.Key.LocalEducationAgencyId.Value,
                        EdOrgType = p.Key.LocalEducationAgencyId.HasValue ? "LEA" : "School",
                        Role = p.Key.PositionTitle
                    }).ToList()
                }).FirstOrDefault();
        }

        private Persistence.Models.People MapPeopleModelToPeopleEntity(PeopleModel model)
        {
            return new Persistence.Models.People
            {
                UniqueId = model.UniqueId,
                Usi = model.Usi,
                ElectronicMailAddress = model.ElectronicMailAddress,
                FirstName = model.FirstName,
                LastSurname = model.LastSurname,
                PositionTitle = model.Role,
                SchoolId = model.SchoolId,
                LocalEducationAgencyId = model.LocalEducationAgencyId,
                AccessLevel=model.AccessLevel,
                LevelId=model.LevelId
            };
        }

        private PeopleModel MapPeopleEntityToPeopleModel(Persistence.Models.People entity)
        {
            return new PeopleModel
            {
                UniqueId = entity.UniqueId,
                Usi = entity.Usi,
                ElectronicMailAddress = entity.ElectronicMailAddress,
                FirstName = entity.FirstName,
                LastSurname = entity.LastSurname,
                Role = entity.PositionTitle,
                SchoolId = entity.SchoolId,
                LocalEducationAgencyId = entity.LocalEducationAgencyId,
                AccessLevel=entity.AccessLevel,
                LevelId=entity.LevelId
            };
        }

    }
}
