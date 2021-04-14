using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentAtRisk
{
    public interface IStudentAtRiskService
    {
        Task<StudentAtRiskModel> Get(int studentUsi);
    }

    public class StudentAtRiskService : IStudentAtRiskService
    {
        private readonly IStudentAtRiskQueries _queries;
        public StudentAtRiskService(IStudentAtRiskQueries queries)
        {
            _queries = queries;
        }

        public async Task<StudentAtRiskModel> Get(int studentUsi)
        {
            var entityList = await _queries.Get(studentUsi);

            return MapStudentAtRiskEntityToStudentAtRiskModel(entityList);
        }

        private Persistence.Models.StudentAtRisk MapStudentAtRiskModelToStudentAtRiskEntity(StudentAtRiskModel model)
        {
            return new Persistence.Models.StudentAtRisk
            {
                StudentUsi=model.StudentUsi,
                IsHomeless=model.IsHomeless,
                Section504=model.Section504,
                Ar=model.Ar,
                Ssi=model.Ssi,
                Ell=model.Ell,
                PREPregnant = model.PREPregnant,
                PREParent = model.PREParent,
                AEP = model.AEP,
                Expelled = model.Expelled,
                Dropout = model.Dropout,
                LEP = model.LEP,
                FosterCare = model.FosterCare,
                ResidentialPlacementFacility = model.ResidentialPlacementFacility,
                Incarcerated = model.Incarcerated,
                AdultEd = model.AdultEd,
                PRS=model.PRS,
                NotAdvanced=model.NotAdvanced
            };
        }

        private StudentAtRiskModel MapStudentAtRiskEntityToStudentAtRiskModel(Persistence.Models.StudentAtRisk entity)
        {
            return new StudentAtRiskModel
            {
                StudentUsi = entity.StudentUsi,
                IsHomeless = entity.IsHomeless,
                Section504 = entity.Section504,
                Ar = entity.Ar,
                Ssi = entity.Ssi,
                Ell = entity.Ell,
                PREPregnant = entity.PREPregnant,
                PREParent = entity.PREParent,
                AEP = entity.AEP,
                Expelled = entity.Expelled,
                Dropout = entity.Dropout,
                LEP = entity.LEP,
                FosterCare = entity.FosterCare,
                ResidentialPlacementFacility = entity.ResidentialPlacementFacility,
                Incarcerated = entity.Incarcerated,
                AdultEd = entity.AdultEd,
                PRS=entity.PRS,
                NotAdvanced=entity.NotAdvanced
            };
        }
    }
}
