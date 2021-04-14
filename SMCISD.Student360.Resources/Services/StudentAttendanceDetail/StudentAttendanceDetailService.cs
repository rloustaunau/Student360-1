using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentAttendanceDetail
{
    public interface IStudentAttendanceDetailService : IGridData
    {
    }

    public class StudentAttendanceDetailService : IStudentAttendanceDetailService { 

        private readonly IStudentAttendanceDetailQueries _queries;
        public StudentAttendanceDetailService(IStudentAttendanceDetailQueries queries)
        {
            _queries = queries;
        }


        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser) => await _queries.GetGridData(request, currentUser);

        private Persistence.Models.StudentAttendanceDetail MapStudentAttendanceDetailModelToStudentAttendanceDetailEntity(StudentAttendanceDetailModel model)
        {
            return new Persistence.Models.StudentAttendanceDetail
            {
                Course = model.Course,
                Semester = model.Semester,
                Code = model.Code,
                SchoolId = model.SchoolId,
                LocalEducationAgencyId = model.LocalEducationAgencyId,
                StudentUsi = model.StudentUsi,
                Date = model.Date,
                Period = model.Period,
                Local=model.Local
            };
        }

        private StudentAttendanceDetailModel MapStudentAttendanceDetailEntityToStudentAttendanceDetailModel(Persistence.Models.StudentAttendanceDetail entity)
        {
            return new StudentAttendanceDetailModel
            {
                Course = entity.Course,
                Semester = entity.Semester,
                Code = entity.Code,
                SchoolId = entity.SchoolId,
                LocalEducationAgencyId = entity.LocalEducationAgencyId,
                StudentUsi = entity.StudentUsi,
                Date = entity.Date,
                Period = entity.Period,
                Description=entity.Description,
                Local = entity.Local
            };
        }
    }
}
