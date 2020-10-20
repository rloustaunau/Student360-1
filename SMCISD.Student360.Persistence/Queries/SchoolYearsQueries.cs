using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface ISchoolYearsQueries 
    {
        Task<List<SchoolYears>> Get();  
    }

    public class SchoolYearsQueries : ISchoolYearsQueries

    {
        private readonly Student360Context _db;

        public SchoolYearsQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<SchoolYears>> Get() => await _db.SchoolYears.ToListAsync();
    }
}
