using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesLocation
{
    public interface IStudentAbsencesLocationService : IGridData {}

    public class StudentAbsencesLocationService : IStudentAbsencesLocationService
    {
        private readonly IStudentAbsencesLocationQueries _queries;
        public StudentAbsencesLocationService(IStudentAbsencesLocationQueries queries)
        {
            _queries = queries;
        }

        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
           return await _queries.GetGridData(request, currentUser);
        }

        private Persistence.Models.StudentAbsencesLocation MapStudentAbsencesLocationModelToStudentAbsencesLocationEntity(StudentAbsencesLocationModel model)
        {
            return new Persistence.Models.StudentAbsencesLocation
            {
                HighestCourseCount = model.HighestCourseCount,
                StudentUsi = model.StudentUsi,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastSurname = model.LastSurname,
                GradeLevel = model.GradeLevel,
                GraduationSchoolYear = model.GraduationSchoolYear,
                SchoolId = model.SchoolId,
                LocalEducationAgencyId = model.LocalEducationAgencyId,
                SchoolYear = model.SchoolYear,
                StudentUniqueId = model.StudentUniqueId,
                AdaAbsences = model.AdaAbsences,
                DaysFromLastAbsence = model.DaysFromLastAbsence
            };
        }

        private StudentAbsencesLocationModel MapStudentAbsencesLocationEntityToStudentAbsencesLocationModel(Persistence.Models.StudentAbsencesLocation entity)
        {
            return new StudentAbsencesLocationModel
            {
                HighestCourseCount = entity.HighestCourseCount,
                StudentUsi = entity.StudentUsi,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                FirstName = entity.FirstName,
                MiddleName = entity.MiddleName,
                LastSurname = entity.LastSurname,
                GradeLevel = entity.GradeLevel,
                GraduationSchoolYear = entity.GraduationSchoolYear,
                SchoolId = entity.SchoolId,
                LocalEducationAgencyId = entity.LocalEducationAgencyId,
                SchoolYear = entity.SchoolYear,
                StudentUniqueId = entity.StudentUniqueId,
                AdaAbsences = entity.AdaAbsences,
                DaysFromLastAbsence = entity.DaysFromLastAbsence
            };
        }
    }
}
