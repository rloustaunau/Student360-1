using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SMCISD.Student360.Resources.Services.Cohort;
using SMCISD.Student360.Resources.Services.Grade;
using SMCISD.Student360.Resources.Services.Schools;
using SMCISD.Student360.Resources.Services.SchoolYears;
using SMCISD.Student360.Resources.Services.Semesters;
using SMCISD.Student360.Web.Attributes;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolController : ControllerBase
    {
        private readonly ISchoolsService _schoolsService;
        private readonly ISchoolYearsService _schoolYearsService;
        private readonly ISemestersService _semestersService;
        private readonly ICohortService _cohortService;
        private readonly IGradeService _gradeService;
        private readonly IConfiguration _config;
        public SchoolController(ISchoolsService schoolsService, ISchoolYearsService schoolYearsService, ISemestersService semestersService, IConfiguration config, ICohortService cohortService, IGradeService gradeService)
        {
            _schoolsService = schoolsService;
            _schoolYearsService = schoolYearsService;
            _semestersService = semestersService;
            _gradeService = gradeService;
            _cohortService = cohortService;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>>  GetSchools()
        {
            return (await _schoolsService.GetGridFilter(User)).ToList();
        }

        [NoAuthFilter]
        [HttpGet("years")]
        public async Task<ActionResult<List<SchoolYearsModel>>> GetSchoolYears()
        {
            return await _schoolYearsService.Get();
        }

        [NoAuthFilter]
        [HttpGet("semesters")]
        public async Task<ActionResult<List<SemestersModel>>> GetSchoolSemesters()
        {
            return await _semestersService.Get();
        }

        [NoAuthFilter]
        [HttpGet("absenceCountList")]
        public async Task<ActionResult<int[]>> GetAbsenceCountList()
        {
            var list = _config.GetSection("AbsenceCountList").Get<int[]>();

            return list;
        }

        [NoAuthFilter]
        [HttpGet("absencePercentList")]
        public async Task<ActionResult<int[]>> GetAbsencePercentList()
        {
            var list = _config.GetSection("AbsencePercentList").Get<int[]>();

            return list;
        }

        [NoAuthFilter]
        [HttpGet("grades")]
        public async Task<ActionResult<List<GradeModel>>> GetGrades()
        {
            var list = await _gradeService.Get();

            return list;
        }

        [NoAuthFilter]
        [HttpGet("cohorts")]
        public async Task<ActionResult<List<CohortModel>>> GetCohorts()
        {
            var list =  await _cohortService.Get();

            return list;
        }
    }
}
