using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentCourseTranscript
{
    public interface IStudentCourseTranscriptService : IGridData
    {
    }

    public class StudentCourseTranscriptService : IStudentCourseTranscriptService { 
        private readonly IStudentCourseTranscriptQueries _queries;
        public StudentCourseTranscriptService(IStudentCourseTranscriptQueries queries)
        {
            _queries = queries;
        }


        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser) => await _queries.GetGridData(request, currentUser);

        private Persistence.Models.StudentCourseTranscript MapStudentCourseTranscriptModelToStudentCourseTranscriptEntity(StudentCourseTranscriptModel model)
        {
            return new Persistence.Models.StudentCourseTranscript
            {
                StudentUsi = model.StudentUsi,
                SchoolId = model.SchoolId,
                SchoolYear = model.SchoolYear,
                LocalEducationAgencyId  = model.LocalEducationAgencyId,
                AttemptedCredits = model.AttemptedCredits,
                CourseCode = model.CourseCode,
                CourseTitle = model.CourseTitle,
                EarnedCredits = model.EarnedCredits,
                FinalNumericGradeEarned = model.FinalNumericGradeEarned
            };
        }

        private StudentCourseTranscriptModel MapStudentCourseTranscriptEntityToStudentCourseTranscriptModel(Persistence.Models.StudentCourseTranscript entity)
        {
            return new StudentCourseTranscriptModel
            {
                StudentUsi = entity.StudentUsi,
                SchoolId = entity.SchoolId,
                SchoolYear = entity.SchoolYear,
                LocalEducationAgencyId = entity.LocalEducationAgencyId,
                AttemptedCredits = entity.AttemptedCredits,
                CourseCode = entity.CourseCode,
                CourseTitle = entity.CourseTitle,
                EarnedCredits = entity.EarnedCredits,
                FinalNumericGradeEarned = entity.FinalNumericGradeEarned
            };
        }
    }
}
