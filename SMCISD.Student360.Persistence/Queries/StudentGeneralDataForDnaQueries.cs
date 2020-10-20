using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentGeneralDataForDnaQueries 
    {
        Task<StudentGeneralDataForDna> GetById(int studentUsi);  
    }

    public class StudentGeneralDataForDnaQueries : IStudentGeneralDataForDnaQueries

    {
        private readonly Student360Context _db;

        public StudentGeneralDataForDnaQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<StudentGeneralDataForDna> GetById(int studentUsi) => await _db.StudentGeneralDataForDna.FirstOrDefaultAsync(x => x.StudentUsi == studentUsi);
    }
}
