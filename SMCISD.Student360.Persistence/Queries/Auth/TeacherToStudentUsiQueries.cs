using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries.Auth
{
    public interface ITeacherToStudentUsiQueries 
    {
        IQueryable<TeacherToStudentUsi> GetTeacherStudents(int staffUsi);
        Task<List<TeacherToStudentUsi>> GetTeacherStudents(int[] staffUsis);
    }

    public class TeacherToStudentUsiQueries : ITeacherToStudentUsiQueries

    {
        private readonly Student360Context _db;

        public TeacherToStudentUsiQueries(Student360Context db)
        {
            _db = db;
        }
        public IQueryable<TeacherToStudentUsi> GetTeacherStudents(int staffUsi) =>  _db.TeacherToStudentUsi.Where(x => x.StaffUsi == staffUsi);

        public async Task<List<TeacherToStudentUsi>> GetTeacherStudents(int[] staffUsis) => await _db.TeacherToStudentUsi.Where(x => staffUsis.Contains(x.StaffUsi)).ToListAsync();
    }
}
