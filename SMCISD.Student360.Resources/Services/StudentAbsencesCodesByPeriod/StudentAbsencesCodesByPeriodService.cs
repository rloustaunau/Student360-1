using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Services.StudentGeneralDataForDna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesCodesByPeriod
{
    public interface IStudentAbsencesCodesByPeriodService
    {
        Task<GeneralStudentDnaDataModel> Get(int studentUsi);
    }

    public class StudentAbsencesCodesByPeriodService : IStudentAbsencesCodesByPeriodService
    {
        private readonly IStudentAbsencesCodesByPeriodQueries _queries;
        private readonly IStudentGeneralDataForDnaService _studentGeneralDataForDnaService;
        public StudentAbsencesCodesByPeriodService(IStudentAbsencesCodesByPeriodQueries queries, IStudentGeneralDataForDnaService studentGeneralDataForDnaService)
        {
            _queries = queries;
            _studentGeneralDataForDnaService = studentGeneralDataForDnaService;
        }

        public async Task<GeneralStudentDnaDataModel> Get(int studentUsi)
        {
            var entityList = await _queries.Get(studentUsi);
            var generalStudentData = await _studentGeneralDataForDnaService.GetById(studentUsi);
            
            var model = MapEntityListToStudentAbsencesCodesByPeriodModel(entityList);

            var result = new GeneralStudentDnaDataModel { 
                Periods = model.OrderBy(x => x.ClassPeriodName).ToList(),
                ApartmentRoomSuiteNumber = generalStudentData.ApartmentRoomSuiteNumber,
                City = generalStudentData.City,
                Gpa = generalStudentData.Gpa,
                NameOfInstitution = generalStudentData.NameOfInstitution,
                StreetNumberName = generalStudentData.StreetNumberName,
                State = generalStudentData.State,
                PostalCode = generalStudentData.PostalCode
            };

            return result;
        }

        private List<StudentAbsencesCodesByPeriodModel> MapEntityListToStudentAbsencesCodesByPeriodModel(List<Persistence.Models.StudentAbsencesCodesByPeriod> entityList)
        {
            return entityList.GroupBy(x => new { x.StudentUsi, x.ClassPeriodName })
                .Select(x => new StudentAbsencesCodesByPeriodModel
                {
                    ClassPeriodName = x.Key.ClassPeriodName,
                    StudentUsi = x.Key.StudentUsi,
                    AbsenceCodes = x.Select(y => new AbsencesCodesByPeriodModel { AbsenceCode = y.AbsenceCode, Quantity = y.Quantity, Description = y.Description }).ToList()
                }).ToList();
        }
        private Persistence.Models.StudentAbsencesCodesByPeriod MapStudentAbsencesCodesByPeriodModelToStudentAbsencesCodesByPeriodEntity(StudentAbsencesCodesByPeriodModel model)
        {
            return new Persistence.Models.StudentAbsencesCodesByPeriod
            {
                StudentUsi = model.StudentUsi,
                ClassPeriodName = model.ClassPeriodName
            };
        }

        private StudentAbsencesCodesByPeriodModel MapStudentAbsencesCodesByPeriodEntityToStudentAbsencesCodesByPeriodModel(Persistence.Models.StudentAbsencesCodesByPeriod entity)
        {
            return new StudentAbsencesCodesByPeriodModel
            {
                StudentUsi = entity.StudentUsi,
                ClassPeriodName = entity.ClassPeriodName
            };
        }
    }
}
