using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Reasons
{
    public interface IReasonsService 
    {
        Task<List<ReasonsModel>> Get();
    }

    public class ReasonsService : IReasonsService
    {
        private readonly IReasonQueries _queries;
        public ReasonsService(IReasonQueries queries)
        {
            _queries = queries;
        }

        public async Task<List<ReasonsModel>> Get()
        {
            var entityList = await _queries.Get();

            return entityList.Select(x => MapReasonsEntityToReasonsModel(x)).ToList();
        }
        private Persistence.Models.Reasons MapReasonsModelToReasonsEntity(ReasonsModel model)
        {
            return new Persistence.Models.Reasons
            {
              Description = model.Description,
              CreateDate = model.CreateDate,
              Value = model.Value,
              HasHours = model.HasHours,
              ReasonId = model.ReasonId
            };
        }

        private ReasonsModel MapReasonsEntityToReasonsModel(Persistence.Models.Reasons entity)
        {
            return new ReasonsModel
            {
                Description = entity.Description,
                CreateDate = entity.CreateDate,
                Value = entity.Value,
                HasHours = entity.HasHours,
                ReasonId = entity.ReasonId
            };
        }

    }
}
