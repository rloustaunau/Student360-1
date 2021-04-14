using Microsoft.AspNetCore.Hosting;
using SMCISD.Student360.Resources.Providers.Messaging;
using SMCISD.Student360.Resources.Services.StudentAbsencesForEmail;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers.Notifications
{
    public class SecondaryNotificationProvider : INotificationProvider
    {
        private readonly IStudentAbsencesForEmailService _service;
        private readonly IMessagingProvider _messagingProvider;
        private readonly IHostingEnvironment _env;

        public SecondaryNotificationProvider(IStudentAbsencesForEmailService service, IMessagingProvider messagingProvider, IHostingEnvironment env)
        {
            _service = service;
            _messagingProvider = messagingProvider;
            _env = env;
        }
        public async Task<int> SendNotifications()
        {
            var data = await _service.GetDataForSecondaryDailyEmails();

            foreach (var emailData in data)
            {
                var to = emailData.StaffEmail;
               // var to = "s360.teacher@smcisd.net";
                string template = FillEmailTemplate(emailData);
                await _messagingProvider.SendMessageAsync(to, null, null, "SMCISD Student360: Attendance Notification", template);
            }

            return 0;
        }

        private string FillEmailTemplate(SecondaryEmailModel emailData)
        {
            var template = loadEmailTemplate();

            var filledTemplate = template.Replace("{{StaffFullName}}", $"{emailData.StaffFirstName} {(string.IsNullOrEmpty(emailData.StaffMiddleName) ? "" : emailData.StaffMiddleName + " ")}{emailData.StaffLastname}")
                                  .Replace("{{EmailMessage}}", "These following students are assigned to a course you teach, and have been marked absent at least 3 of the last 5 attendance periods.")
                                  .Replace("{{TableContent}}", CalculateTableContent(emailData));
            return filledTemplate;
        }

        private string loadEmailTemplate()
        {
            // Get alert template
            var pathToTemplate = 
                Path.Combine(_env.ContentRootPath, "Templates/StudentAbsenceNotification.txt");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }

        private string CalculateTableContent(SecondaryEmailModel emailData)
        {
            string tablecontent = "<tbody>";
            string days = "";
            foreach (var day in emailData.CoursePeriods.FirstOrDefault().Students.FirstOrDefault().AbsencesFromLastWeek)
                days += $"<td>{day.Date.ToString("ddd MMM dd")}</td>";
            foreach (var period in emailData.CoursePeriods)
            {
                tablecontent += $@"<tr>
                                        <td background='background: #eeeeee !important;'>
                                            <table style='width:100%;margin:auto;background: #ffffff;padding: 25px;margin-bottom: 10px;'>
                                                <tbody>
                                                    <tr>
                                                        <td colspan='4'>Period {period.Period}</td>
                                                        <td colspan='4'>{period.CourseTitle}</td>
                                                        <td colspan='4'>Course #: {period.CourseCode}</td>
                                                    </tr>
                                                    <tr style='border-bottom:2px solid #000'>
                                                        <td>First Last (name)</td>
                                                        <td>Student ID</td>
                                                        <td>Learn Loc</td>
                                                        {days}
                                                    </tr>";
                foreach(var student in period.Students)
                {
                    tablecontent += @$"<tr>
                    <td>{student.StudentFirstName} {student.StudentLastName}</td>
                    <td>{student.StudentUniqueId}</td>
                    <td>{student.LearnLocation}</td>";

                    foreach (var day in student.AbsencesFromLastWeek)
                        tablecontent += $"<td>{(day.Absent ? "A" : "")}</td>";

                    tablecontent += "</tr>";
                }

                tablecontent += "</tbody></table></td></tr>";
            }
           
           
            tablecontent += "</tbody>";

            return tablecontent;
        }
    }
}
