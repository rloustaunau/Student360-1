using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Providers.Image;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesByCourse
{
    public interface IStudentAbsencesByCourseService {
        public Task<StudentProfileModel> Get(GridRequest request, IPrincipal currentUser);
    }

    public class StudentAbsencesByCourseService : IStudentAbsencesByCourseService
    {
        private readonly IStudentAbsencesByCourseQueries _queries;
        private readonly IImageProvider _imgProvider;
        public StudentAbsencesByCourseService(IStudentAbsencesByCourseQueries queries, IImageProvider imgProvider)
        {
            _queries = queries;
            _imgProvider = imgProvider;
        }

        public async Task<StudentProfileModel> Get(GridRequest request, IPrincipal currentUser)
        {
            request.AllData = true;
            var studentUsi = request.Filters.FirstOrDefault(x => x.Column == "StudentUsi");
            var studentUniqueId = request.Filters.FirstOrDefault(x => x.Column == "StudentUniqueId");

            if (studentUsi == null || studentUniqueId == null)
                return null;

            var gridData = await _queries.GetGridData(request, currentUser);
            string image = "";
            try
            {
                image = await _imgProvider.GetStudentImageUrlAsync(studentUniqueId.Value.ToString());
            }
            catch (Exception) {
                image = "";
            }
            

            return new StudentProfileModel
            {
                Grid = gridData,
                ImageUrl = image
            };


        }

        private Persistence.Models.StudentAbsencesByCourse MapStudentAbsencesByCourseModelToStudentAbsencesByCourseEntity(StudentAbsencesByCourseModel model)
        {
            return new Persistence.Models.StudentAbsencesByCourse
            {
               AbsencesCount = model.AbsencesCount,
               ClassPeriodName = model.ClassPeriodName,
               Room = model.Room,
               GradeLevel = model.GradeLevel,
               GraduationSchoolYear = model.GraduationSchoolYear,
               LocalCourseCode = model.LocalCourseCode,
               LocalCourseTitle = model.LocalCourseTitle,
               Mark9w1 = model.Mark9w1,
               Mark9w2 = model.Mark9w2,
               Mark9w3 = model.Mark9w3,
               Mark9w4 = model.Mark9w4,
               NameOfInstitution = model.NameOfInstitution,
               SchoolId = model.SchoolId,
               SchoolYear = model.SchoolYear,
               SectionIdentifier = model.SectionIdentifier,
               SessionName = model.SessionName,
               StudentFirstName = model.StudentFirstName,
               StudentLastSurname = model.StudentLastSurname,
               StudentUniqueId = model.StudentUniqueId,
               StudentUsi = model.StudentUsi,
               TeacherLastSurname = model.TeacherLastSurname,
               Credits = model.Credits,
               LocalEducationAgencyId = model.LocalEducationAgencyId,
               Fs1 = model.Fs1,
               Fs2 = model.Fs2,
               S1abs = model.S1abs,
               S2abs = model.S2abs,
               Yfinal = model.Yfinal
            };
        }

        private StudentAbsencesByCourseModel MapStudentAbsencesByCourseEntityToStudentAbsencesByCourseModel(Persistence.Models.StudentAbsencesByCourse entity)
        {
            return new StudentAbsencesByCourseModel
            {
                AbsencesCount = entity.AbsencesCount,
                ClassPeriodName = entity.ClassPeriodName,
                Room = entity.Room,
                GradeLevel = entity.GradeLevel,
                GraduationSchoolYear = entity.GraduationSchoolYear,
                LocalCourseCode = entity.LocalCourseCode,
                LocalCourseTitle = entity.LocalCourseTitle,
                Mark9w1 = entity.Mark9w1,
                Mark9w2 = entity.Mark9w2,
                Mark9w3 = entity.Mark9w3,
                Mark9w4 = entity.Mark9w4,
                NameOfInstitution = entity.NameOfInstitution,
                SchoolId = entity.SchoolId,
                SchoolYear = entity.SchoolYear,
                SectionIdentifier = entity.SectionIdentifier,
                SessionName = entity.SessionName,
                StudentFirstName = entity.StudentFirstName,
                StudentLastSurname = entity.StudentLastSurname,
                StudentUniqueId = entity.StudentUniqueId,
                StudentUsi = entity.StudentUsi,
                TeacherLastSurname = entity.TeacherLastSurname,
                Credits = entity.Credits,
                LocalEducationAgencyId = entity.LocalEducationAgencyId,
                Fs1 = entity.Fs1,
                Fs2 = entity.Fs2,
                S1abs = entity.S1abs,
                S2abs = entity.S2abs,
                Yfinal = entity.Yfinal
            };
        }
    }
}
