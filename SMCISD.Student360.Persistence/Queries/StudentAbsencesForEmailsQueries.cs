using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentAbsencesForEmailsQueries
    {
        Task<List<StudentAbsencesForEmails>> Get();
        Task<List<StudentAbsencesForEmails>> GetAbsencesForElementaryTeacher();
        Task<List<StudentAbsencesForEmails>> GetAbsencesForSecondaryTeacher();
    }

    public class StudentAbsencesForEmailsQueries : IStudentAbsencesForEmailsQueries

    {
        private readonly Student360Context _db;

        public StudentAbsencesForEmailsQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<StudentAbsencesForEmails>> Get() => await _db.StudentAbsencesForEmails.ToListAsync();

        public async Task<List<StudentAbsencesForEmails>> GetAbsencesForElementaryTeacher()
        {
            var elementaryGradeLevels = new List<string> { "01", "02", "03", "04" , "EE", "KG", "PK", "IT" };
            var absences = await _db.StudentAbsencesForEmails
                .Where(x => elementaryGradeLevels.Contains(x.GradeLevel) && x.Period == "2"
                && x.StaffHomeRoomEmail != null).ToListAsync();
            
            return absences;
        }

        public async Task<List<StudentAbsencesForEmails>> GetAbsencesForSecondaryTeacher()
        {
            var secondaryGradeLevels = new List<string> { "05", "06", "07", "08", "09","10", "11", "12" };
            var absences = await _db.StudentAbsencesForEmails
                .Where(x => secondaryGradeLevels.Contains(x.GradeLevel))
                .ToListAsync();
            
            return absences;
        }
    }
}

