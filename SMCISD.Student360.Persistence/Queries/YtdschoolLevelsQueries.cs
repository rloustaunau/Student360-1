using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IYtdschoolLevelsQueries
    {
        Task<List<YtdschoolLevels>> Get();  
    }

    public class YtdschoolLevelsQueries : IYtdschoolLevelsQueries

    {
        private readonly Student360Context _db;

        public YtdschoolLevelsQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<YtdschoolLevels>> Get() => await _db.YtdschoolLevels.ToListAsync();
    }
}
