using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMCISD.Student360.Resources.Services.AttendanceLetters;
using SMCISD.Student360.Resources.Services.Notification;
using SMCISD.Student360.Web.Attributes;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _service;
        private readonly IAttendanceLetterService _letterService;
        public NotificationController(INotificationService service, IAttendanceLetterService letterService)
        {
            _service = service;
            _letterService = letterService;
        }

        [NoAuthFilter]
        [AllowAnonymous]
        [HttpGet]
        public async Task SendNotifications()
        {
            // Send all emails
             await _service.SendNotifications();
        }


        [NoAuthFilter]
        [AllowAnonymous]
        [HttpGet("letter")]
        public async Task TestAttendanceLetters()
        {
            // Generate Letters
            await _letterService.GenerateLetters();
        }
    }
}
