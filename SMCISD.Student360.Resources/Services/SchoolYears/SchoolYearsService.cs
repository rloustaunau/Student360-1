using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.SchoolYears
{
    public interface ISchoolYearsService 
    {
        Task<List<SchoolYearsModel>> Get();
    }

    public class SchoolYearsService : ISchoolYearsService
    {
        private readonly ISchoolYearsQueries _queries;
        public SchoolYearsService(ISchoolYearsQueries queries)
        {
            _queries = queries;
        }

        public async Task<List<SchoolYearsModel>> Get()
        {
            var entityList = await _queries.Get();

            return entityList.Select(x => MapSchoolYearsEntityToSchoolYearsModel(x)).ToList();
        }

        private Persistence.Models.SchoolYears MapSchoolYearsModelToSchoolYearsEntity(SchoolYearsModel model)
        {
            return new Persistence.Models.SchoolYears
            {
                SchoolYear = model.SchoolYear
            };
        }

        private SchoolYearsModel MapSchoolYearsEntityToSchoolYearsModel(Persistence.Models.SchoolYears entity)
        {
            return new SchoolYearsModel
            {
               SchoolYear = entity.SchoolYear
            };
        }

    }
}
