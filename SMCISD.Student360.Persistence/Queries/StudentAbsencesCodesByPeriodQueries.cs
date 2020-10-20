using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentAbsencesCodesByPeriodQueries 
    {
        Task<List<StudentAbsencesCodesByPeriod>> Get(int studentUsi);  
    }

    public class StudentAbsencesCodesByPeriodQueries : IStudentAbsencesCodesByPeriodQueries

    {
        private readonly Student360Context _db;

        public StudentAbsencesCodesByPeriodQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<StudentAbsencesCodesByPeriod>> Get(int studentUsi) => await _db.StudentAbsencesCodesByPeriod.Where(x => x.StudentUsi == studentUsi).ToListAsync();
    }
}
