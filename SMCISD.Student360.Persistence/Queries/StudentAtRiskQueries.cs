using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentAtRiskQueries
    {
        Task<StudentAtRisk> Get(int studentUsi);
    }

    public class StudentAtRiskQueries : IStudentAtRiskQueries

    {
        private readonly Student360Context _db;

        public StudentAtRiskQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<StudentAtRisk> Get(int studentUsi) => await _db.StudentAtRisk.FirstOrDefaultAsync(x=> x.StudentUsi==studentUsi);
    }
}
