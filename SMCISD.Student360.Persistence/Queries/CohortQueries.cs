using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface ICohortQueries 
    {
        Task<List<Cohort>> Get();  
    }

    public class CohortQueries : ICohortQueries

    {
        private readonly Student360Context _db;

        public CohortQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<Cohort>> Get() => await _db.Cohort.OrderBy(x => x.GraduationSchoolYear).ToListAsync();
    }
}
