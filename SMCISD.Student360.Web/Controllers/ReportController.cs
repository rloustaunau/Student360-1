using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SMCISD.Student360.Resources.Services.Adacampus;
using SMCISD.Student360.Resources.Services.Adadistrict;
using SMCISD.Student360.Resources.Services.DistrictDailyAttendanceRate;
using SMCISD.Student360.Resources.Services.Reports;
using SMCISD.Student360.Resources.Services.YtdgradeLevel;
using SMCISD.Student360.Resources.Services.YtdschoolLevels;
using SMCISD.Student360.Web.Attributes;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IDistrictDailyAttendanceRateService _districtDailyAttendanceRateService;
        private readonly IReportService _reportService;
        private readonly IAdacampusService _adacampusService;
        private readonly IAdadistrictService _adadistrictService;
        private readonly IYtdgradeLevelService _ytdGradeLevelService;
        private readonly IYtdschoolLevelsService _ytdschoolLevelsService;
        private readonly IConfiguration _config;
        public ReportController(IConfiguration config, IDistrictDailyAttendanceRateService districtDailyAttendanceRateService, IReportService reportService, IAdacampusService adacampusService, IAdadistrictService adadistrictService, IYtdgradeLevelService ytdgradeLevelService, IYtdschoolLevelsService ytdschoolLevelsService)
        {
            _districtDailyAttendanceRateService = districtDailyAttendanceRateService;
            _config = config;
            _reportService = reportService;
            _adacampusService = adacampusService;
            _adadistrictService = adadistrictService;
            _ytdGradeLevelService = ytdgradeLevelService;
            _ytdschoolLevelsService = ytdschoolLevelsService;
        }

        [NoAuthFilter]
        [HttpGet("adarate")]
        public async Task<ActionResult<List<DistrictDailyAttendanceRateModel>>> GetSchoolSemesters()
        {
            return await _districtDailyAttendanceRateService.Get();
        }

        [NoAuthFilter]
        [HttpGet("reports")]
        public async Task<ActionResult<List<ReportModel>>> GetReports()
        {
            var levelId = int.Parse(User.FindFirst(x => x.Type.Contains("level_id")).Value);
            return await _reportService.Get(levelId);
        }

        [NoAuthFilter]
        [HttpGet("ytdrate")]
        public async Task<ActionResult<List<AdacampusModel>>> GetAdaCampus()
        {
            return await _adacampusService.GetAdaYtdRates();
        }

        [NoAuthFilter]
        [HttpGet("ytdschoollevel")]
        public async Task<ActionResult<List<YtdschoolLevelsModel>>> GetYtdSchoolLevel()
        {
            return await _ytdschoolLevelsService.Get();
        }

        [NoAuthFilter]
        [HttpGet("ytdgradelevel")]
        public async Task<ActionResult<List<YtdgradeLevelModel>>> GetYtdGradeLevel()
        {
            return await _ytdGradeLevelService.Get();
        }
    }
}
