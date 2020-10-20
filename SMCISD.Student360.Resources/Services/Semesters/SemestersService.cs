using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Semesters
{
    public interface ISemestersService 
    {
        Task<List<SemestersModel>> Get();
    }

    public class SemestersService : ISemestersService
    {
        private readonly ISemestersQueries _queries;
        public SemestersService(ISemestersQueries queries)
        {
            _queries = queries;
        }

        public async Task<List<SemestersModel>> Get()
        {
            var entityList = await _queries.Get();

            return entityList.Select(x => MapSemestersEntityToSemestersModel(x)).ToList();
        }
        private Persistence.Models.Semesters MapSemestersModelToSemestersEntity(SemestersModel model)
        {
            return new Persistence.Models.Semesters
            {
              SessionName = model.SessionName
            };
        }

        private SemestersModel MapSemestersEntityToSemestersModel(Persistence.Models.Semesters entity)
        {
            return new SemestersModel
            {
                SessionName = entity.SessionName
            };
        }

    }
}
