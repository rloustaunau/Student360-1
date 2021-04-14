using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SMCISD.Student360.Persistence.Commands;
using SMCISD.Student360.Persistence.Enum;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Models;
using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Providers.Pdf;
using SMCISD.Student360.Resources.Services.Schools;
using SMCISD.Student360.Resources.Services.SchoolYears;
using SMCISD.Student360.Resources.Services.StudentAbsencesForEmail;
using SMCISD.Student360.Resources.Services.StudentExtraHours;
using SMCISD.Student360.Resources.Services.StudentGeneralDataForDna;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.AttendanceLetters
{
    public interface IAttendanceLetterService : IGridData
    {
        Task GenerateLetters();
        Task<List<AttendanceLetterStatusModel>> GetAttendanceLetterStatus();
        Task<List<AttendanceLetterModel>> UpdateLetterBulk(List<AttendanceLetterModel> letters, IPrincipal currentUser);
        Task<LetterFileModel> SendLetterBulk(List<AttendanceLetterModel> letters, IPrincipal currentUser);
        Task<LetterFileModel> ReprintLetterBulk(List<AttendanceLetterModel> letters, IPrincipal currentUser);
    }
    public class AttendanceLetterService : IAttendanceLetterService
    {
        private readonly IAttendanceLetterCommands _commands;
        private readonly IAttendanceLetterQueries _queries;
        private readonly IStudentAbsencesForEmailService _studentAbsencesForEmailService;
        private readonly IStudentExtraHoursService _studentExtraHoursService;
        private readonly IConfiguration _config;
        private readonly ICalendarMembershipDaysQueries _calendarQueries;
        private readonly IPdfProvider _pdfProvider;
        private readonly ISchoolYearsService _schoolYearService;
        private readonly ISchoolsService _schoolsService;
        private readonly IStudentGeneralDataForDnaService _studentGeneralDataForDnaService;
        private readonly IHostingEnvironment _env;
        private int Day3Range;
        private int Day5Range;
        private int Day10Range;
        public AttendanceLetterService(IAttendanceLetterCommands commands, IAttendanceLetterQueries queries, IStudentAbsencesForEmailService studentAbsencesForEmailService, ICalendarMembershipDaysQueries calendarQueries, IConfiguration config, IPdfProvider pdfProvider, ISchoolYearsService schoolYearService, IStudentExtraHoursService studentExtraHoursService, IHostingEnvironment env, ISchoolsService schoolsService, IStudentGeneralDataForDnaService studentGeneralDataForDnaService)
        {
            _commands = commands;
            _queries = queries;
            _config = config;
            _studentAbsencesForEmailService = studentAbsencesForEmailService;
            _calendarQueries = calendarQueries;
            _pdfProvider = pdfProvider;
            _schoolYearService = schoolYearService;
            _studentExtraHoursService = studentExtraHoursService;
            _env = env;
            _studentGeneralDataForDnaService = studentGeneralDataForDnaService;
            _schoolsService = schoolsService;

            Day3Range = _config.GetSection("Notifications:AttendanceLetter:3DayLetterRange").Get<int>();
            Day5Range = _config.GetSection("Notifications:AttendanceLetter:5DayLetterRange").Get<int>();
            Day10Range = _config.GetSection("Notifications:AttendanceLetter:10DayLetterRange").Get<int>();
        }

        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            var grid =  await _queries.GetGridData(request,currentUser);

            var letters = grid.Data.Cast<AttendanceLetterGrid>();

            var types = await _queries.GetAttendaceLetterTypes();

            var todayDateForLetters = DateTime.Now.Date.AddDays(-2);

            var Groupedletters = letters.GroupBy(x => x.AttendanceLetterTypeId).Select(x => new AttendaceLetterTypeGridDataModel
            {   
                Count = x.Count(),
                TypeId  =  x.Key,
                Type = x.FirstOrDefault().Type,
                MaxLetterAge = x.Max(x => Math.Abs((int)(x.LastAbsence - todayDateForLetters).TotalDays)),
                AverageLetterAge = x.Sum(x => Math.Abs((int)(x.LastAbsence - todayDateForLetters).TotalDays))/x.Count(),
            }).ToList();

            var metadata = new List<object>();
            var Day3Group = Groupedletters.Where(x => x.TypeId == AttendanceLetterTypeEnum.Day3Letter.Value).FirstOrDefault();
            var Day5Group = Groupedletters.Where(x => x.TypeId == AttendanceLetterTypeEnum.Day5Letter.Value).FirstOrDefault();
            var Day10Group = Groupedletters.Where(x => x.TypeId == AttendanceLetterTypeEnum.Day10Letter.Value).FirstOrDefault();

            if (Day3Group == null)
                metadata.Add(new AttendaceLetterTypeGridDataModel
                {
                    Count = 0,
                    TypeId = AttendanceLetterTypeEnum.Day3Letter.Value,
                    Type = types.FirstOrDefault(x => x.AttendanceLetterTypeId == 1).CodeValue,
                    MaxLetterAge = 0,
                    AverageLetterAge = 0
                });
            else metadata.Add(Day3Group);

            if (Day5Group == null)
                metadata.Add(new AttendaceLetterTypeGridDataModel
                {
                    Count = 0,
                    TypeId = AttendanceLetterTypeEnum.Day5Letter.Value,
                    Type = types.FirstOrDefault(x => x.AttendanceLetterTypeId == 2).CodeValue,
                    MaxLetterAge = 0,
                    AverageLetterAge = 0
                });
            else metadata.Add(Day5Group);

            if (Day10Group == null)
                metadata.Add(new AttendaceLetterTypeGridDataModel
                {
                    Count = 0,
                    TypeId = AttendanceLetterTypeEnum.Day10Letter.Value,
                    Type = types.FirstOrDefault(x => x.AttendanceLetterTypeId == 3).CodeValue,
                    MaxLetterAge = 0,
                    AverageLetterAge = 0
                });
            else metadata.Add(Day10Group);

            grid.Metadata = metadata;

            return grid;
        }

        public async Task<List<AttendanceLetterStatusModel>> GetAttendanceLetterStatus()
        {
            var entities = await _queries.GetAttendaceLetterStatus();

            return entities.Select(x => MapAttendanceLetterStatusEntityToModel(x)).ToList();
        }

        public async Task<List<AttendanceLetterModel>> UpdateLetterBulk(List<AttendanceLetterModel> letters, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            var role = claims.First(x => x.Type.Contains("role")).Value;
            var userUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
            var userFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
            var userLastsurname = claims.First(x => x.Type.Contains("lastsurname")).Value;


            foreach (var letter in letters)
            {
                letter.UserRole = role;
                letter.UserCreatedUniqueId = userUniqueId;
                letter.UserFirstName = userFirstName;
                letter.UserLastSurname = userLastsurname;
            }
            var entities = letters.Select(x => MapAttendanceLetterModelToEntity(x)).ToList();

            var resultingEntities = await _commands.UpdateLetterBulk(entities);

            return letters;
        }

        public async Task<LetterFileModel> SendLetterBulk(List<AttendanceLetterModel> letters, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            var role = claims.First(x => x.Type.Contains("role")).Value;
            var userUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
            var userFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
            var userLastsurname = claims.First(x => x.Type.Contains("lastsurname")).Value;

            foreach (var letter in letters)
            {
                letter.UserRole = role;
                letter.UserCreatedUniqueId = userUniqueId;
                letter.UserFirstName = userFirstName;
                letter.UserLastSurname = userLastsurname;
                letter.ResolutionDate = DateTime.Now;
                letter.AttendanceLetterStatusId = AttendanceLetterStatusEnum.Sent.Value; 
            }

            var entities = letters.Select(x => MapAttendanceLetterModelToEntity(x)).ToList();

            var resultingEntities = await _commands.UpdateLetterBulk(entities);

            var attendanceActionsToAdd = entities.Select(x => MapEntityAttendanceLettersEntityToStudentExtraHoursEntity(x)).ToList();

            await _studentExtraHoursService.CreateStudentExtraHourBulk(attendanceActionsToAdd);


            var schoolExtraInfo = await _schoolsService.GetSchoolInfo(letters.FirstOrDefault().SchoolId);
            string htmlString = await GenerateHtmlFromLetters(letters, schoolExtraInfo, false);
            var file =  _pdfProvider.GetPdfFromHtmlString(htmlString, loadPdfGeneralStyles());
            var today = DateTime.Now;
            var fileName = $"{schoolExtraInfo.ShortNameOfInstitution.Trim()}_{today.ToString("yyyy-MM-dd")}_{today.ToString("HH.mm.ss")}";

            File.WriteAllBytes(Path.Combine(_env.ContentRootPath, $"Letters/{fileName}.pdf"), file);

            //Generate ZIP

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/{fileName}.pdf"), fileName+".pdf");
                   
                    List<AttendanceLetterModel> schools = letters.GroupBy(m => m.SchoolId).Select(x => x.First()).ToList();

                    String newLine = "";
                    StreamWriter stream;
                    foreach (var school in schools)
                    {
                        var letterSchool3 = letters.Where(m => m.SchoolId == school.SchoolId && m.AttendanceLetterTypeId == 1);
                        if (letterSchool3.Count() > 0) { 
                            
                            stream = File.CreateText(Path.Combine(_env.ContentRootPath, $"Letters/3day{school.SchoolId}.txt"));

                            newLine = "Student ID, Qualifying date, Type";
                            stream.WriteLine(newLine);
                            foreach (var letter in letterSchool3)
                            {
                                newLine = letter.StudentUsi + "," + letter.CreateDate.ToShortDateString() + "," + "3";
                                stream.WriteLine(newLine);
                            }
                            stream.Close();
                            archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/3day{school.SchoolId}.txt"), $"3day{school.SchoolId}.txt");
                        }

                        var letterSchool5 = letters.Where(m => m.SchoolId == school.SchoolId && m.AttendanceLetterTypeId == 2);
                        if (letterSchool5.Count() > 0)
                        {
                            stream = File.CreateText(Path.Combine(_env.ContentRootPath, $"Letters/5day{school.SchoolId}.txt"));

                            newLine = "Student ID, Qualifying date, Type";
                            stream.WriteLine(newLine);
                            foreach (var letter in letterSchool5)
                            {
                                newLine = letter.StudentUsi + "," + letter.CreateDate.ToShortDateString() + "," + "5";
                                stream.WriteLine(newLine);
                            }
                            stream.Close();
                            archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/5day{school.SchoolId}.txt"), $"5day{school.SchoolId}.txt");
                        }

                        var letterSchool10 = letters.Where(m => m.SchoolId == school.SchoolId && m.AttendanceLetterTypeId == 3);
                        if (letterSchool10.Count() > 0)
                        {
                            stream = File.CreateText(Path.Combine(_env.ContentRootPath, $"Letters/10day{school.SchoolId}.txt"));

                            newLine = "Student ID, Qualifying date, Type";
                            stream.WriteLine(newLine);
                            foreach (var letter in letterSchool5)
                            {
                                newLine = letter.StudentUsi + "," + letter.CreateDate.ToShortDateString() + "," + "10";
                                stream.WriteLine(newLine);
                            }
                            stream.Close();

                            archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/10day{school.SchoolId}.txt"), $"10day{school.SchoolId}.txt");
                        }
                    }
                }

                //File(memoryStream.ToArray(), "application/zip", fileDownloadName: pdf.FileName);
                return new LetterFileModel { FileContent = memoryStream.ToArray(), FileName = fileName };
            }


            
        }
        

        public async Task<LetterFileModel> ReprintLetterBulk(List<AttendanceLetterModel> letters, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            var role = claims.First(x => x.Type.Contains("role")).Value;
            var userUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
            var userFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
            var userLastsurname = claims.First(x => x.Type.Contains("lastsurname")).Value;
            var isReprint = false;
            foreach (var letter in letters)
            {

                // When the latters are already sent, they won't be updated
                if (letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Sent.Value)
                {
                    isReprint = true;
                    break;
                }

                letter.UserRole = role;
                letter.UserCreatedUniqueId = userUniqueId;
                letter.UserFirstName = userFirstName;
                letter.UserLastSurname = userLastsurname;
                letter.ResolutionDate = DateTime.Now;
                letter.AttendanceLetterStatusId = AttendanceLetterStatusEnum.Sent.Value;
            }

            var entities = letters.Select(x => MapAttendanceLetterModelToEntity(x)).ToList();

            var resultingEntities = await _commands.UpdateLetterBulk(entities);

            var attendanceActionsToAdd = entities.Select(x => MapEntityAttendanceLettersEntityToStudentExtraHoursEntity(x)).ToList();

            await _studentExtraHoursService.CreateStudentExtraHourBulk(attendanceActionsToAdd);


            var schoolExtraInfo = await _schoolsService.GetSchoolInfo(letters.FirstOrDefault().SchoolId);
            string htmlString = await GenerateHtmlFromLetters(letters, schoolExtraInfo, isReprint);
            var file = _pdfProvider.GetPdfFromHtmlString(htmlString, loadPdfGeneralStyles());
            var today = DateTime.Now;
            var fileName = $"{schoolExtraInfo.ShortNameOfInstitution.Trim()}_{today.ToString("yyyy-MM-dd")}_{today.ToString("HH.mm.ss")}";

            File.WriteAllBytes(Path.Combine(_env.ContentRootPath, $"Letters/{fileName}.pdf"), file);

            //Generate ZIP

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/{fileName}.pdf"), fileName + ".pdf");

                    List<AttendanceLetterModel> schools = letters.GroupBy(m => m.SchoolId).Select(x => x.First()).ToList();

                    String newLine = "";
                    StreamWriter stream;
                    foreach (var school in schools)
                    {
                        var letterSchool3 = letters.Where(m => m.SchoolId == school.SchoolId && m.AttendanceLetterTypeId == 1);
                        if (letterSchool3.Count() > 0)
                        {

                            stream = File.CreateText(Path.Combine(_env.ContentRootPath, $"Letters/3day{school.SchoolId}.txt"));

                            newLine = "Student ID, Qualifying date, Type";
                            stream.WriteLine(newLine);
                            foreach (var letter in letterSchool3)
                            {
                                newLine = letter.StudentUsi + "," + letter.CreateDate.ToShortDateString() + "," + "3";
                                stream.WriteLine(newLine);
                            }
                            stream.Close();
                            archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/3day{school.SchoolId}.txt"), $"3day{school.SchoolId}.txt");
                        }

                        var letterSchool5 = letters.Where(m => m.SchoolId == school.SchoolId && m.AttendanceLetterTypeId == 2);
                        if (letterSchool5.Count() > 0)
                        {
                            stream = File.CreateText(Path.Combine(_env.ContentRootPath, $"Letters/5day{school.SchoolId}.txt"));

                            newLine = "Student ID, Qualifying date, Type";
                            stream.WriteLine(newLine);
                            foreach (var letter in letterSchool5)
                            {
                                newLine = letter.StudentUsi + "," + letter.CreateDate.ToShortDateString() + "," + "5";
                                stream.WriteLine(newLine);
                            }
                            stream.Close();
                            archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/5day{school.SchoolId}.txt"), $"5day{school.SchoolId}.txt");
                        }

                        var letterSchool10 = letters.Where(m => m.SchoolId == school.SchoolId && m.AttendanceLetterTypeId == 3);
                        if (letterSchool10.Count() > 0)
                        {
                            stream = File.CreateText(Path.Combine(_env.ContentRootPath, $"Letters/10day{school.SchoolId}.txt"));

                            newLine = "Student ID, Qualifying date, Type";
                            stream.WriteLine(newLine);
                            foreach (var letter in letterSchool5)
                            {
                                newLine = letter.StudentUsi + "," + letter.CreateDate.ToShortDateString() + "," + "10";
                                stream.WriteLine(newLine);
                            }
                            stream.Close();

                            archive.CreateEntryFromFile(Path.Combine(_env.ContentRootPath, $"Letters/10day{school.SchoolId}.txt"), $"10day{school.SchoolId}.txt");
                        }
                    }
                }

                //File(memoryStream.ToArray(), "application/zip", fileDownloadName: pdf.FileName);
                return new LetterFileModel { FileContent = memoryStream.ToArray(), FileName = fileName };
            }
        }

        private async Task<string> GenerateHtmlFromLetters(List<AttendanceLetterModel> letters, EducationOrganiztionInformationModel schoolExtraInfo, bool reprint)
        {
            var htmlResult = "";

            foreach (var letter in letters)
                htmlResult += await GenerateHtmlContent(letter, schoolExtraInfo, reprint);

            return htmlResult;
        }

        private async Task<string> GenerateHtmlContent(AttendanceLetterModel letter, EducationOrganiztionInformationModel schoolExtraInfo, bool reprint)
        {
            // Apply logic by Schoold and by LetterType
            var studentExtraInfo = await _studentGeneralDataForDnaService.GetById(letter.StudentUsi);
            
            var  letterType = GetLetterTypeValue(letter.AttendanceLetterTypeId);
            string template;
            string pronoun;

            switch (studentExtraInfo.Sex) {
                case "Male":
                    pronoun = "his";
                    break;
                case "Female":
                    pronoun = "her";
                    break;
                default:
                    pronoun = "its";
                    break;
            }

            template = loadPdfAttendanceLetterTemplate(letterType);

           template = template.Replace("{{StudentName}}", $"{letter.FirstName} {(string.IsNullOrEmpty(letter.MiddleName) ? "" : letter.MiddleName + " ")}{letter.LastSurname}")
            .Replace("{{StudentUniqueId}}", letter.StudentUniqueId)
            .Replace("{{LetterType}}", letterType.ToString())
            .Replace("{{StudentAddress}}", studentExtraInfo.StreetNumberName)
            .Replace("{{StudentCity}}", studentExtraInfo.City)
            .Replace("{{StudentState}}", studentExtraInfo.State)
            .Replace("{{StudentPostalCode}}", studentExtraInfo.PostalCode)
            .Replace("{{SchoolAddress}}", schoolExtraInfo.StreetNumberName)
            .Replace("{{SchoolCity}}", schoolExtraInfo.City)
            .Replace("{{SchoolState}}", schoolExtraInfo.State)
            .Replace("{{SchoolPostalCode}}", schoolExtraInfo.PostalCode)
            .Replace("{{SchoolPhoneNumber}}", schoolExtraInfo.Phone)
            .Replace("{{SchoolName}}", schoolExtraInfo.NameOfInstitution)
            .Replace("{{Today}}", $"{letter.ResolutionDate.Value.ToString("MMMM dd, yyyy")}{(reprint? $" Reprint Date: {DateTime.Now.ToString("MMMM dd, yyyy")}" : "")}")
            .Replace("{{AlertLevel}}", GetAlertLevel(letter.AttendanceLetterTypeId))
            .Replace("{{Pronoun}}", pronoun)
            .Replace("{{SchoolPrincipal}}", $"{schoolExtraInfo.PrincipalFirstName} {schoolExtraInfo.PrincipalLastSurname}");

            return template;
        }

        private string GetAlertLevel(int attendanceLetterTypeId)
        {

            if (attendanceLetterTypeId == AttendanceLetterTypeEnum.Day3Letter.Value)
                return "ALERT";
            else if (attendanceLetterTypeId == AttendanceLetterTypeEnum.Day5Letter.Value)
                return "HIGH ALERT";
            else if (attendanceLetterTypeId == AttendanceLetterTypeEnum.Day10Letter.Value)
                return "WARNING";
            else
                throw new NotImplementedException("This attendance letter type is not valid");
        }

        private int GetLetterTypeValue(int attendanceLetterTypeId)
        {
            if (attendanceLetterTypeId == AttendanceLetterTypeEnum.Day3Letter.Value)
                return 3;
            else if (attendanceLetterTypeId == AttendanceLetterTypeEnum.Day5Letter.Value)
                return 5;
            else if (attendanceLetterTypeId == AttendanceLetterTypeEnum.Day10Letter.Value)
                return 10;
            else
                throw new NotImplementedException("This attendance letter type is not valid");
        }

        private string loadPdfAttendanceLetterTemplate(int letterType)
        {
            // Get alert template
            var pathToTemplate =
                Path.Combine(_env.ContentRootPath, $"Templates/Day{letterType}AttendanceLetter.txt");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }


        private string loadPdfGeneralStyles()
        {
            // Get alert template
            var pathToTemplate =
                Path.Combine(_env.ContentRootPath, "Styles/GeneralPdfStyles.txt");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }

        public async Task GenerateLetters()
        {
            var students = await _studentAbsencesForEmailService.GetAbsencesByStudent();
            var currentSchoolYear = (await _schoolYearService.Get()).FirstOrDefault().SchoolYear;
            // If a student  doesn't have more than 2 absences it should be ignored

            foreach (var student in students)
            {
                var letters = await _queries.GetLettersByStudentId(student.StudentUniqueId);

                var Day3Letter = letters
                    .Where(x => x.AttendanceLetterTypeId == AttendanceLetterTypeEnum.Day3Letter.Value)
                    .OrderByDescending(x => x.LastAbsence)
                    .FirstOrDefault();
                var Day5Letter = letters
                    .Where(x => x.AttendanceLetterTypeId == AttendanceLetterTypeEnum.Day5Letter.Value)
                    .OrderByDescending(x => x.LastAbsence)
                    .FirstOrDefault();
                var Day10Letter = letters
                    .Where(x => x.AttendanceLetterTypeId == AttendanceLetterTypeEnum.Day10Letter.Value)
                    .OrderByDescending(x => x.LastAbsence)
                    .FirstOrDefault();

                bool create3DayLetter = false, create5DayLetter = false, create10DayLetter = false;
                string Day3Period = "", Day5Period = "", Day10Period = "";
                foreach (var period in student.Periods)
                {
                    if (create10DayLetter)
                        continue;

                    foreach (var date in period.Absences)
                    {
                        if (create10DayLetter)
                            continue;

                        var last10LetterDateRange = await _calendarQueries
                            .GetFutureDateFromAbsenceDate(student.SchoolId, date, Day10Range);
                        var absencesBy10DayLetterRange = period.Absences.Where(x => x.Date.Date >= date.Date && x.Date.Date <= last10LetterDateRange.Date.Date).Take(10).Count();
                        if (!create10DayLetter)
                        {
                            create10DayLetter = absencesBy10DayLetterRange > 9;
                            if (create10DayLetter)
                                Day10Period = period.Period;
                        }

                        if (create5DayLetter)
                            continue;

                        var last5LetterDateRange = await _calendarQueries
                            .GetFutureDateFromAbsenceDate(student.SchoolId, date, Day5Range);
                        var absencesBy5DayLetterRange = period.Absences.Where(x => x.Date.Date >= date.Date && x.Date.Date <= last5LetterDateRange.Date.Date).Take(5).Count();
                        if (!create5DayLetter)
                        {
                            create5DayLetter = absencesBy5DayLetterRange > 4;
                            if (create5DayLetter)
                                Day5Period = period.Period;
                        }

                        if (create3DayLetter)
                            continue;

                        var last3LetterDateRange = await _calendarQueries
                            .GetFutureDateFromAbsenceDate(student.SchoolId, date, Day3Range);
                        var absencesBy3DayLetterRange = period.Absences.Where(x => x.Date.Date >= date.Date && x.Date.Date <= last3LetterDateRange.Date.Date).Take(3).Count();
                        if (!create3DayLetter)
                        {
                            create3DayLetter = absencesBy3DayLetterRange > 2;
                            if (create3DayLetter)
                                Day3Period = period.Period;
                        }

                    }
                }


                if (create3DayLetter == false && Day3Letter != null)
                {
                    Day3Letter.AttendanceLetterStatus = null;
                    Day3Letter.AttendanceLetterStatusId = AttendanceLetterStatusEnum.AutoCancelled.Value;
                    await _commands.UpdateLetter(Day3Letter);
                }
                else if (create3DayLetter && Day3Letter == null)
                {
                    Persistence.Models.AttendanceLetters new3DayLetter = 
                        GenerateAttendanceLetter(student,
                        Day3Period,
                        currentSchoolYear, 
                        AttendanceLetterStatusEnum.Open.Value, 
                        AttendanceLetterTypeEnum.Day3Letter.Value);

                    await _commands.SaveLetter(new3DayLetter);
                }
                else if (create3DayLetter && Day3Letter != null && Day3Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Open.Value)
                {
                    var absences = student.Periods.Where(x => x.Period == Day3Period).FirstOrDefault().Absences;
                    Day3Letter.ClassPeriodName = Day3Period;
                    Day3Letter.FirstAbsence = absences.First();
                    Day3Letter.LastAbsence = absences.Last();

                    await _commands.UpdateLetter(Day3Letter);
                }

                if (create5DayLetter == false && Day5Letter != null)
                {
                    Day5Letter.AttendanceLetterStatus = null;
                    Day5Letter.AttendanceLetterStatusId = AttendanceLetterStatusEnum.AutoCancelled.Value;
                    await _commands.UpdateLetter(Day5Letter);
                }
                else if (create5DayLetter && Day5Letter == null)
                {
                    Persistence.Models.AttendanceLetters new5DayLetter =
                       GenerateAttendanceLetter(student,
                       Day5Period,
                       currentSchoolYear,
                       AttendanceLetterStatusEnum.Open.Value,
                       AttendanceLetterTypeEnum.Day5Letter.Value);

                    await _commands.SaveLetter(new5DayLetter);
                }
                else if (create5DayLetter && Day5Letter != null && Day5Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Open.Value)
                {

                    var absences = student.Periods.Where(x => x.Period == Day5Period).FirstOrDefault().Absences;
                    Day5Letter.ClassPeriodName = Day5Period;
                    Day5Letter.FirstAbsence = absences.First();
                    Day5Letter.LastAbsence = absences.Last();

                    await _commands.UpdateLetter(Day5Letter);
                }


                if (create10DayLetter == false && Day10Letter != null)
                {
                    Day10Letter.AttendanceLetterStatus = null;
                    Day10Letter.AttendanceLetterStatusId = AttendanceLetterStatusEnum.AutoCancelled.Value;
                    await _commands.UpdateLetter(Day10Letter);
                }
                else if (create10DayLetter && Day10Letter == null)
                {
                    Persistence.Models.AttendanceLetters new10DayLetter =
                       GenerateAttendanceLetter(student,
                       Day10Period,
                       currentSchoolYear,
                       AttendanceLetterStatusEnum.Open.Value,
                       AttendanceLetterTypeEnum.Day10Letter.Value);

                    await _commands.SaveLetter(new10DayLetter);
                }
                else if (create10DayLetter && Day10Letter != null && Day10Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Open.Value)
                {

                    var absences = student.Periods.Where(x => x.Period == Day10Period).FirstOrDefault().Absences;
                    Day10Letter.ClassPeriodName = Day10Period;
                    Day10Letter.FirstAbsence = absences.First();
                    Day10Letter.LastAbsence = absences.Last();

                    await _commands.UpdateLetter(Day10Letter);
                }

                // Reverse Logic
                create3DayLetter = false; create5DayLetter = false; create10DayLetter = false;
                Day3Period = ""; Day5Period = ""; Day10Period = "";
                foreach (var period in student.Periods)
                {
                    if (create10DayLetter)
                        continue;

                    var today = DateTime.Now.Date;

                    var last10LetterDateRange = await _calendarQueries.GetPastDateFromAbsenceDate(student.SchoolId, today, Day10Range);
                    var absencesBy10DayLetterRange = period.Absences.Where(x => x.Date.Date <= today && x.Date.Date >= last10LetterDateRange.Date.Date).Take(10).Count();

                    if (!create10DayLetter)
                    {
                        create10DayLetter = absencesBy10DayLetterRange > 9;
                        if (create10DayLetter)
                            Day10Period = period.Period;
                    }

                    if (create5DayLetter)
                        continue;

                    var last5LetterDateRange = await _calendarQueries.GetPastDateFromAbsenceDate(student.SchoolId, today, Day5Range);
                    var absencesBy5DayLetterRange = period.Absences.Where(x => x.Date.Date <= today && x.Date.Date >= last5LetterDateRange.Date.Date).Take(5).Count();

                    if (!create5DayLetter)
                    {
                        create5DayLetter = absencesBy5DayLetterRange > 4;
                        if (create5DayLetter)
                            Day5Period = period.Period;
                    }

                    if (create3DayLetter)
                        continue;

                    var last3LetterDateRange = await _calendarQueries.GetPastDateFromAbsenceDate(student.SchoolId, today, Day3Range);
                    var absencesBy3DayLetterRange = period.Absences.Where(x => x.Date.Date <= today && x.Date.Date >= last3LetterDateRange.Date.Date).Take(3).Count();

                    if (!create3DayLetter)
                    {
                        create3DayLetter = absencesBy3DayLetterRange > 2;
                        if (create3DayLetter)
                            Day3Period = period.Period;
                    }
                }

                if (create3DayLetter && ((Day10Letter != null && Day10Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Sent.Value
                        && student.Periods.Where(x => x.Period == Day3Period).FirstOrDefault().Absences.First() > Day10Letter.LastAbsence)
                    || (Day5Letter != null && Day5Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Sent.Value
                        && student.Periods.Where(x => x.Period == Day3Period).FirstOrDefault().Absences.First() > Day5Letter.FirstAbsence.AddDays(120))
                    || (Day3Letter != null && student.Periods.Where(x => x.Period == Day3Period).FirstOrDefault().Absences.Last() > Day3Letter.FirstAbsence.AddDays(60))))
                {
                    Persistence.Models.AttendanceLetters new3DayLetter =
                        GenerateAttendanceLetter(student,
                        Day3Period,
                        currentSchoolYear,
                        AttendanceLetterStatusEnum.Open.Value,
                        AttendanceLetterTypeEnum.Day3Letter.Value);

                    await _commands.SaveLetter(new3DayLetter);
                }

                if(create5DayLetter && ((Day10Letter != null && Day10Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Sent.Value
                        && student.Periods.Where(x => x.Period == Day5Period).FirstOrDefault().Absences.First() > Day10Letter.LastAbsence)
                    || (Day5Letter != null && student.Periods.Where(x => x.Period == Day5Period).FirstOrDefault().Absences.Last() > Day5Letter.FirstAbsence.AddDays(120))))
                {
                    Persistence.Models.AttendanceLetters new5DayLetter =
                        GenerateAttendanceLetter(student,
                        Day5Period,
                        currentSchoolYear,
                        AttendanceLetterStatusEnum.Open.Value,
                        AttendanceLetterTypeEnum.Day5Letter.Value);
                  
                    await _commands.SaveLetter(new5DayLetter);
                }

                if(create10DayLetter && (Day10Letter != null && Day10Letter.AttendanceLetterStatusId == AttendanceLetterStatusEnum.Sent.Value
                    && student.Periods.Where(x => x.Period == Day10Period).FirstOrDefault().Absences.First() > Day10Letter.LastAbsence))
                {
                    Persistence.Models.AttendanceLetters new10DayLetter =
                        GenerateAttendanceLetter(student,
                        Day10Period,
                        currentSchoolYear,
                        AttendanceLetterStatusEnum.Open.Value,
                        AttendanceLetterTypeEnum.Day10Letter.Value);

                    await _commands.SaveLetter(new10DayLetter);
                }
            }
        }

        private Persistence.Models.AttendanceLetters GenerateAttendanceLetter(StudentAbsencesModel student, string period, short currentSchoolYear, int status, int type)
        {
            var absences = student.Periods.Where(x => x.Period == period).FirstOrDefault().Absences;
            return  new Persistence.Models.AttendanceLetters
                    {
                        ClassPeriodName = period,
                        FirstName = student.StudentFirstName,
                        MiddleName = student.StudentMiddleName,
                        LastSurname = student.StudentLastName,
                        FirstAbsence = absences.First(),
                        LastAbsence = absences.Last(),
                        AttendanceLetterStatusId = status,
                        SchoolId = student.SchoolId,
                        StudentUniqueId = student.StudentUniqueId,
                        SchoolYear = currentSchoolYear,
                        GradeLevel = student.GradeLevel,
                        AttendanceLetterTypeId = type,
                    };
        }

        private Persistence.Models.AttendanceLetters UpdateAttendanceLetter(StudentAbsencesModel student, string period, short currentSchoolYear, int status, int type)
        {
            var absences = student.Periods.Where(x => x.Period == period).FirstOrDefault().Absences;
            return new Persistence.Models.AttendanceLetters
            {
                ClassPeriodName = period,
                FirstName = student.StudentFirstName,
                MiddleName = student.StudentMiddleName,
                LastSurname = student.StudentLastName,
                FirstAbsence = absences.First(),
                LastAbsence = absences.Last(),
                AttendanceLetterStatusId = status,
                SchoolId = student.SchoolId,
                StudentUniqueId = student.StudentUniqueId,
                SchoolYear = currentSchoolYear,
                GradeLevel = student.GradeLevel,
                AttendanceLetterTypeId = type,
            };
        }

        private AttendanceLetterStatusModel MapAttendanceLetterStatusEntityToModel(AttendanceLetterStatus entity)
        {
            return new AttendanceLetterStatusModel
            {
                AttendanceLetterStatusId = entity.AttendanceLetterStatusId,
                CodeValue = entity.CodeValue,
                Description = entity.Description,
                ShortDescription = entity.ShortDescription
            };
        }

        private AttendanceLetterModel MapAttendanceLetterEntityToModel(Persistence.Models.AttendanceLetters entity)
        {
            return new AttendanceLetterModel
            {
                AttendanceLetterStatusId = entity.AttendanceLetterStatusId,
                AttendanceLetterId = entity.AttendanceLetterId,
                AttendanceLetterTypeId = entity.AttendanceLetterTypeId,
                Type = entity.AttendanceLetterType?.CodeValue,
                ClassPeriodName = entity.ClassPeriodName,
                FirstAbsence = entity.FirstAbsence,
                LastAbsence = entity.LastAbsence,
                UserLastSurname = entity.UserLastSurname,
                UserCreatedUniqueId = entity.UserCreatedUniqueId,
                UserFirstName = entity.UserFirstName,
                UserRole = entity.UserRole,
                FirstName = entity.FirstName,
                LastSurname = entity.LastSurname,
                Id = entity.Id,
                CreateDate = entity.CreateDate,
                MiddleName = entity.MiddleName,
                SchoolId = entity.SchoolId,
                Status = entity.AttendanceLetterStatus?.CodeValue,
                StudentUniqueId =entity.StudentUniqueId,
                GradeLevel = entity.GradeLevel,
                SchoolYear = entity.SchoolYear,
                ResolutionDate = entity.ResolutionDate,
                Comments = entity.Comments,
            };
        }

        private Persistence.Models.AttendanceLetters MapAttendanceLetterModelToEntity(AttendanceLetterModel model)
        {
            return new Persistence.Models.AttendanceLetters
            {
                AttendanceLetterStatusId = model.AttendanceLetterStatusId,
                AttendanceLetterId = model.AttendanceLetterId,
                AttendanceLetterTypeId = model.AttendanceLetterTypeId,
                ClassPeriodName = model.ClassPeriodName,
                FirstAbsence = model.FirstAbsence,
                LastAbsence = model.LastAbsence,
                UserLastSurname = model.UserLastSurname,
                UserCreatedUniqueId = model.UserCreatedUniqueId,
                UserFirstName = model.UserFirstName,
                UserRole = model.UserRole,
                FirstName = model.FirstName,
                LastSurname = model.LastSurname,
                Id = model.Id,
                CreateDate = model.CreateDate,
                MiddleName = model.MiddleName,
                SchoolId = model.SchoolId,
                StudentUniqueId = model.StudentUniqueId,
                GradeLevel = model.GradeLevel,
                SchoolYear = model.SchoolYear,
                ResolutionDate = model.ResolutionDate,
                Comments = model.Comments,
            };
        }

        private Persistence.Models.StudentExtraHours MapEntityAttendanceLettersEntityToStudentExtraHoursEntity(Persistence.Models.AttendanceLetters letter)
        {
            return new Persistence.Models.StudentExtraHours
            {
                Comments = letter.Comments,
                Date = letter.LastAbsence,
                FirstName = letter.FirstName,
                LastSurname = letter.LastSurname,
                GradeLevel = letter.GradeLevel,
                Hours = 0,
                ReasonId = GetReasonIdFromAttendanceLetterTypeId(letter.AttendanceLetterTypeId),
                SchoolYear = letter.SchoolYear,
                StudentUniqueId = letter.StudentUniqueId,
                UserCreatedUniqueId = letter.UserCreatedUniqueId,
                UserFirstName = letter.UserFirstName,
                UserLastSurname = letter.UserLastSurname,
                UserRole = letter.UserRole,
                Version = 1
            };
        }


        private int GetReasonIdFromAttendanceLetterTypeId(int attendanceTypeId)
        {
            if (attendanceTypeId == AttendanceLetterTypeEnum.Day3Letter.Value)
                return ReasonEnum.Day3Letter.Value;
            else if (attendanceTypeId == AttendanceLetterTypeEnum.Day5Letter.Value)
                return ReasonEnum.Day5Letter.Value;
            else if (attendanceTypeId == AttendanceLetterTypeEnum.Day10Letter.Value)
                return ReasonEnum.Day10Letter.Value;
            else
                throw new NotImplementedException("This attendance letter type is not valid");
        }
    }
}
