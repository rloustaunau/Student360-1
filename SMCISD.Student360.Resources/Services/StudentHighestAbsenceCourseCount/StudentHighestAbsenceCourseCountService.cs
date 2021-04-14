using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentHighestAbsenceCourseCount
{
    public interface IStudentHighestAbsenceCourseCountService : IGridData
    {
    }

    public class StudentHighestAbsenceCourseCountService : IStudentHighestAbsenceCourseCountService { 
        private readonly IStudentHighestAbsenceCourseCountQueries _queries;
        public StudentHighestAbsenceCourseCountService(IStudentHighestAbsenceCourseCountQueries queries)
        {
            _queries = queries;
        }


        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser) => await _queries.GetGridData(request, currentUser);

        private Persistence.Models.StudentHighestAbsenceCourseCount MapStudentHighestAbsenceCourseCountModelToStudentHighestAbsenceCourseCountEntity(StudentHighestAbsenceCourseCountModel model)
        {
            return new Persistence.Models.StudentHighestAbsenceCourseCount
            {
                GradeLevel = model.GradeLevel,
                GraduationSchoolYear = model.GraduationSchoolYear,
                HighestCourseCount = model.HighestCourseCount,
                NameOfInstitution = model.NameOfInstitution,
                SchoolId = model.SchoolId,
                SchoolYear = model.SchoolYear,
                StudentUniqueId  = model.StudentUniqueId,
                StudentUsi = model.StudentUsi,
                FirstName = model.FirstName,
                LastSurname = model.LastSurname,
                GradeDescription=model.GradeDescription
            };
        }

        private StudentHighestAbsenceCourseCountModel MapStudentHighestAbsenceCourseCountEntityToStudentHighestAbsenceCourseCountModel(Persistence.Models.StudentHighestAbsenceCourseCount entity)
        {
            return new StudentHighestAbsenceCourseCountModel
            {
                GradeLevel = entity.GradeLevel,
                GraduationSchoolYear = entity.GraduationSchoolYear,
                HighestCourseCount = entity.HighestCourseCount,
                NameOfInstitution = entity.NameOfInstitution,
                SchoolId = entity.SchoolId,
                SchoolYear = entity.SchoolYear.Value,
                StudentUniqueId = entity.StudentUniqueId,
                StudentUsi = entity.StudentUsi,
                FirstName = entity.FirstName,
                LastSurname = entity.LastSurname,
                GradeDescription=entity.GradeDescription
            };
        }
    }
}
