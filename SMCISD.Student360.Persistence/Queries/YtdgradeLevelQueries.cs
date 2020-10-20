using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IYtdgradeLevelQueries 
    {
        Task<List<YtdgradeLevel>> Get();  
    }

    public class YtdgradeLevelQueries : IYtdgradeLevelQueries

    {
        private readonly Student360Context _db;

        public YtdgradeLevelQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<YtdgradeLevel>> Get() => await _db.YtdgradeLevel.OrderBy(x => x.GradeLevel).ToListAsync();
    }
}
