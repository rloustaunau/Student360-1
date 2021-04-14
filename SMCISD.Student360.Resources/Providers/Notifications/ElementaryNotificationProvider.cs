using Microsoft.AspNetCore.Hosting;
using SMCISD.Student360.Resources.Providers.Messaging;
using SMCISD.Student360.Resources.Services.StudentAbsencesForEmail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Providers.Notifications
{
    public class ElementaryNotificationProvider : INotificationProvider
    {
        private readonly IStudentAbsencesForEmailService _service;
        private readonly IMessagingProvider _messagingProvider;
        private readonly IHostingEnvironment _env;

        public ElementaryNotificationProvider(IStudentAbsencesForEmailService service, IMessagingProvider messagingProvider, IHostingEnvironment env)
        {
            _service = service;
            _messagingProvider = messagingProvider;
            _env = env;
        }
        public async Task<int> SendNotifications()
        {
            var data = await _service.GetDataForElementaryDailyEmails();

            foreach (var emailData in data)
            {
                var to = emailData.StaffHomeRoomEmail;
                //var to = "s360.teacher@smcisd.net";
                string template = FillEmailTemplate(emailData);
                await _messagingProvider.SendMessageAsync(to, null, null, "SMCISD Student360: Attendance Notification", template);
            }

            return 0;
        }

        private string FillEmailTemplate(ElementaryEmailModel emailData)
        {
            var template = loadEmailTemplate();

            var filledTemplate = template.Replace("{{StaffFullName}}", $"{emailData.HomeRoomStaffFirstName} {(string.IsNullOrEmpty(emailData.HomeRoomStaffMiddleName) ? "" : emailData.HomeRoomStaffMiddleName + " ")}{emailData.HomeRoomStaffLastSurname}")
                                  .Replace("{{EmailMessage}}", "These following students are assigned to a course you teach, and have been marked absent at least 3 of the past 5 days PRIOR to yesterday.")
                                  .Replace("{{TableContent}}", CalculateTableContent(emailData));
            return filledTemplate;
        }

        private string loadEmailTemplate()
        {
            // Get alert template
            var pathToTemplate = Path.Combine(_env.ContentRootPath, "Templates/StudentAbsenceNotification.txt");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }

        private string CalculateTableContent(ElementaryEmailModel emailData)
        {
            string tablecontent = "<thead style='background: #ffffff;'><tr style='background: #ffffff;'>";

            tablecontent += "<th>First Last (name)</th> <th>Student ID</th> <th>Learn Location</th>";

            foreach(var day in emailData.Students.FirstOrDefault().AbsencesFromLastWeek)
                tablecontent += $"<th>{day.Date.ToString("ddd MMM dd")}</th>";
            tablecontent += "</tr></thead> <tbody style='background: #ffffff;'>";
            foreach(var student in emailData.Students)
            {
                tablecontent += @$"<tr style='background: #ffffff;'>
                    <td>{student.StudentFirstName} {student.StudentLastName}</td>
                    <td>{student.StudentUniqueId}</td>
                    <td>{student.LearnLocation}</td>";

                foreach (var day in student.AbsencesFromLastWeek)
                    tablecontent += $"<td>{(day.Absent ? "A" : "")}</td>";

                tablecontent += "</tr>";
            }
            tablecontent += "</tbody>";

            return tablecontent;
        }
    }
}
