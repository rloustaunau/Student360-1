using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IHomeroomToStudentUsiQueries 
    {
        Task<List<HomeroomToStudentUsi>> GetStaffHomeroomStudents(int staffUsi);  
    }

    public class HomeroomToStudentUsiQueries : IHomeroomToStudentUsiQueries

    {
        private readonly Student360Context _db;

        public HomeroomToStudentUsiQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<HomeroomToStudentUsi>> GetStaffHomeroomStudents(int staffUsi) => await _db.HomeroomToStudentUsi.Where(x => x.StaffUsi == staffUsi).ToListAsync();
    }
}
