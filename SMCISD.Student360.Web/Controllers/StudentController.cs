using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Resources.Services.StudentAbsencesByCourse;
using SMCISD.Student360.Resources.Services.StudentAbsencesCodesByPeriod;
using SMCISD.Student360.Resources.Services.StudentAbsencesLocation;
using SMCISD.Student360.Resources.Services.StudentAtRisk;
using SMCISD.Student360.Resources.Services.StudentAttendanceDetail;
using SMCISD.Student360.Resources.Services.StudentCourseTranscript;
using SMCISD.Student360.Resources.Services.StudentExtraHours;
using SMCISD.Student360.Resources.Services.StudentHighestAbsenceCourseCount;
using SMCISD.Student360.Web.Attributes;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentHighestAbsenceCourseCountService _studentHighestAbsenceCourseCountService;
        private readonly IStudentAbsencesByCourseService _studentAbsencesByCourseService;
        private readonly IStudentAbsencesLocationService _studentAbsencesLocationService;
        private readonly IStudentAbsencesCodesByPeriodService _studentAbsencesCodesByPeriod;
        private readonly IStudentAttendanceDetailService _studentAttendanceDetailService;
        private readonly IStudentAtRiskService _studentAtRiskService;
        private readonly IStudentCourseTranscriptService _studentCourseTranscriptService;
        public StudentController(IStudentHighestAbsenceCourseCountService studentHighestAbsenceCourseCountService,
            IStudentAbsencesByCourseService studentAbsencesByCourseService,
            IStudentAbsencesLocationService studentAbsencesLocationService,
            IStudentAbsencesCodesByPeriodService studentAbsencesCodesByPeriod,
            IStudentAttendanceDetailService studentAttendanceDetailService,
            IStudentAtRiskService studentAtRiskService,
            IStudentCourseTranscriptService studentCourseTranscriptService
            )
        {
            _studentHighestAbsenceCourseCountService = studentHighestAbsenceCourseCountService;
            _studentAbsencesByCourseService = studentAbsencesByCourseService;
            _studentAbsencesLocationService = studentAbsencesLocationService;
            _studentAbsencesCodesByPeriod = studentAbsencesCodesByPeriod;
            _studentAtRiskService = studentAtRiskService;
            _studentAttendanceDetailService = studentAttendanceDetailService;
            _studentCourseTranscriptService = studentCourseTranscriptService;
        }

        [HttpPost("grid")]
        public async Task<ActionResult<GridResponse>> GetGridData([FromBody]GridRequest request)
        {
            return await _studentHighestAbsenceCourseCountService.GetGridData(request, User);
        }

        [HttpPost("profile")]
        public async Task<ActionResult<StudentProfileModel>> GetStudentProfile([FromBody]GridRequest request)
        {
            var model = await _studentAbsencesByCourseService.Get(request, User);

            if (model == null)
                return NoContent();

            return model;
        }

        [HttpPost("dna")]
        public async Task<ActionResult<GridResponse>> GetDnaChart([FromBody]GridRequest request)
        {
            return await _studentAbsencesLocationService.GetGridData(request, User);
        }

        [NoAuthFilter]
        [HttpGet("absencescodes/{studentUsi}")]
        public async Task<ActionResult<GeneralStudentDnaDataModel>> GetStudentAbsencesCodesByPeriod(int studentUsi)
        {
            return await _studentAbsencesCodesByPeriod.Get(studentUsi);
        }

        [HttpGet("atRisk/{studentUsi}")]
        public async Task<ActionResult<StudentAtRiskModel>> GetStudentAtRisk(int studentUsi)
        {
            return await _studentAtRiskService.Get(studentUsi);
        }

        // Pending Move student exrta hour calls to another controller.
        [HttpPost("attendance")]
        public async Task<ActionResult<GridResponse>> GetStudentAttendance([FromBody]GridRequest request)
        {
            return await _studentAttendanceDetailService.GetGridData(request, User);
        }

        [HttpPost("courseTranscript")]
        public async Task<ActionResult<GridResponse>> GetStudentCourseTranscript([FromBody]GridRequest request)
        {
            return await _studentCourseTranscriptService.GetGridData(request, User);
        }

    }
}
