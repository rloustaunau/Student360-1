using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Resources.Services.Reasons;
using SMCISD.Student360.Resources.Services.StudentExtraHours;
using SMCISD.Student360.Web.Attributes;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentExtraHourController : ControllerBase
    {
        private readonly IStudentExtraHoursService _studentExtraHoursService;
        private readonly IReasonsService _reasonsService;
        public StudentExtraHourController(IStudentExtraHoursService studentExtraHoursService, IReasonsService reasonsService)
        {
            _studentExtraHoursService = studentExtraHoursService;
            _reasonsService = reasonsService;
        }

        [NoAuthFilter]
        [HttpGet("reasons")]
        public async Task<ActionResult<List<ReasonsModel>>> GetReasons()
        {
            return await _reasonsService.Get();
        }

        [NoAuthFilter]
        [HttpPost("import")]
        public async Task<ActionResult<List<StudentExtraHoursModel>>> ImportStudentExtraHours([FromBody]List<StudentExtraHoursModel> studentExtraHours)
        {

            if (Int32.Parse(User.FindFirst(x => x.Type.Contains("level_id")).Value) > 0)
                return Forbid();

            return await _studentExtraHoursService.ImportStudentExtraHours(studentExtraHours, User);
        }

        [HttpPost("grid")]
        public async Task<ActionResult<GridResponse>> GetStudentExtraHoursGrid([FromBody]GridRequest request)
        {
            return await _studentExtraHoursService.GetGridData(request, User);
        }


        [HttpPost("current")]
        public async Task<ActionResult<GridResponse>> GetCurrentStudentExtraHours([FromBody] GridRequest request)
        {
            return await _studentExtraHoursService.GetCurrentStudentExtraHours(request, User);
        }

        [HttpPost("history")]
        public async Task<ActionResult<GridResponse>> GetHistoryHoursById([FromBody]GridRequest request)
        {
            return await _studentExtraHoursService.GetHistoryDataById(request, User);
        }

        [NoAuthFilter]
        [HttpPost("create")]
        public async Task<ActionResult<StudentExtraHoursModel>> CreateStudentExtraHours([FromBody]StudentExtraHoursModel model)
        {
            if (Int32.Parse(User.FindFirst(x => x.Type.Contains("level_id")).Value) > 3)
                return Forbid();

            return await _studentExtraHoursService.CreateStudentExtraHours(model, User);
        }

        [NoAuthFilter]
        [HttpPut()]
        public async Task<ActionResult<StudentExtraHoursModel>> UpdateStudentExtraHours([FromBody] StudentExtraHourGridModel model)
        {
            if (Int32.Parse(User.FindFirst(x => x.Type.Contains("level_id")).Value) > 3)
                return Forbid();

            return await _studentExtraHoursService.UpdateStudentExtraHours(model, User);
        }

        [NoAuthFilter]
        [HttpPut("bulk")]
        public async Task<ActionResult<List<StudentExtraHoursModel>>> UpdateBulkStudentExtraHours([FromBody] List<StudentExtraHourGridModel> model)
        {
            if (Int32.Parse(User.FindFirst(x => x.Type.Contains("level_id")).Value) > 3)
                return Forbid();

            return await _studentExtraHoursService.UpdateBulkStudentExtraHours(model, User);
        }
    }
}
