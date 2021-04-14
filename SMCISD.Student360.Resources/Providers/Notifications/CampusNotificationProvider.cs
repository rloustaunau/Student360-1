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
    public class CampusNotificationProvider : INotificationProvider
    {
        private readonly IStudentAbsencesForEmailService _service;
        private readonly IMessagingProvider _messagingProvider;
        private readonly IHostingEnvironment _env;

        public CampusNotificationProvider(IStudentAbsencesForEmailService service, IMessagingProvider messagingProvider, IHostingEnvironment env)
        {
            _service = service;
            _messagingProvider = messagingProvider;
            _env = env;
        }
        public async Task<int> SendNotifications()
        {
            var data = await _service.GetDataForCampusDailyEmails();

            /*var to = "oscar@student1.org";
            string template = FillEmailTemplate(data[0].Data);
            await _messagingProvider.SendMessageAsync(to, null, null, "SMCISD Student360: Attendance Notification", template);*/

           foreach(var school in data)
            {
                var to = $"{school.ShortSchoolName.ToLower().Trim()}-Attendance.Admins@smcisd.net";
                string template = FillEmailTemplate(school.Data);
                await _messagingProvider.SendMessageAsync(to, null, null, "SMCISD Student360: Attendance Notification", template);
            }


            return 0;
        }

        private string FillEmailTemplate(List<CampusEmailSchoolModel> emailData)
        {
            var template = loadEmailTemplate();

            var filledTemplate = template.Replace("{{StaffFullName}}", "Campus Attendance Administrator")
                                  .Replace("{{EmailMessage}}", @"The following teachers were notified today of students that were absent 3 of the last 5 days.
                                                                <ul>
                                                                    <li>Elementary students are calculated by counting 3 or more absences in the 5 school days PRIOR to yesterday to compensate for the delay in recording Remote Asynchronous student absences.</li>
                                                                    <li>Secondary students are calculated by counting 3 or more absences in a single period over the last 5 times that period met for class.  This looks back approximately 8 school calendar days to accommodate A & B day scheduling.</li>
                                                                </ul>")
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

        private string CalculateTableContent(List<CampusEmailSchoolModel> emailData)
        {
            emailData = emailData.OrderByDescending(m=> m.AbsentPercentage).ToList();

            string tablecontent = @"<thead style='background: #ffffff;'><tr style='background: #ffffff;'>
                                    <th>Teacher Name</th>
                                    <th>Students with 3+ absences in last 5 days</th>
                                    <th>Total Qty of Student Assigned</th>
                                    </tr></thead><tbody style='background: #ffffff;'>";
          
            foreach (var row in emailData)   
            {
                
                tablecontent += @$"<tr style='background: #ffffff;'>
                        <td style='background: #ffffff;'>{row.StaffFirstName} {row.StaffLastname} ({Math.Round(row.AbsentPercentage, 2)}%)</td>
                        <td style='background: #ffffff;'>{row.TotalAbsenceStudents}</td>
                        <td style='background: #ffffff;'>{row.TotalStudents}</td>
                    </tr>";
            }
            tablecontent += "</tbody>";

            return tablecontent;
        }
    }
}
