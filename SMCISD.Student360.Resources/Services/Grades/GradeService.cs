using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Grade
{
    public interface IGradeService 
    {
        Task<List<GradeModel>> Get();
    }

    public class GradeService : IGradeService
    {
        private readonly IGradeQueries _queries;
        public GradeService(IGradeQueries queries)
        {
            _queries = queries;
        }

        public async Task<List<GradeModel>> Get()
        {
            var entityList = await _queries.Get();

            return entityList.Select(x => MapGradeEntityToGradeModel(x)).ToList();
        }

        private Persistence.Models.Grade MapGradeModelToGradeEntity(GradeModel model)
        {
            return new Persistence.Models.Grade
            {
                CodeValue = model.CodeValue
            };
        }

        private GradeModel MapGradeEntityToGradeModel(Persistence.Models.Grade entity)
        {
            return new GradeModel
            {
                CodeValue = entity.CodeValue
            };
        }

    }
}
