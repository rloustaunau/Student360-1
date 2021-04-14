using SMCISD.Student360.Persistence.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentGeneralDataForDna
{
    public interface IStudentGeneralDataForDnaService
    {
        Task<StudentGeneralDataForDnaModel> GetById(int studentUsi);
    }

    public class StudentGeneralDataForDnaService : IStudentGeneralDataForDnaService
    {
        private readonly IStudentGeneralDataForDnaQueries _queries;
        public StudentGeneralDataForDnaService(IStudentGeneralDataForDnaQueries queries)
        {
            _queries = queries;
        }

        public async Task<StudentGeneralDataForDnaModel> GetById(int studentUsi)
        {
            var entity = await _queries.GetById(studentUsi);

            return MapStudentGeneralDataForDnaEntityToStudentGeneralDataForDnaModel(entity);
        }

        private Persistence.Models.StudentGeneralDataForDna MapStudentGeneralDataForDnaModelToStudentGeneralDataForDnaEntity(StudentGeneralDataForDnaModel model)
        {
            return new Persistence.Models.StudentGeneralDataForDna
            {
                StudentUsi = model.StudentUsi,
                NameOfInstitution = model.NameOfInstitution,
                StreetNumberName = model.StreetNumberName,
                ApartmentRoomSuiteNumber = model.ApartmentRoomSuiteNumber,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                Gpa = model.Gpa,
                Sex = model.Sex
            };
        }

        private StudentGeneralDataForDnaModel MapStudentGeneralDataForDnaEntityToStudentGeneralDataForDnaModel(Persistence.Models.StudentGeneralDataForDna entity)
        {
            return new StudentGeneralDataForDnaModel
            {
                StudentUsi = entity.StudentUsi,
                NameOfInstitution = entity.NameOfInstitution,
                StreetNumberName = entity.StreetNumberName,
                ApartmentRoomSuiteNumber = entity.ApartmentRoomSuiteNumber,
                City = entity.City,
                State = entity.State,
                PostalCode = entity.PostalCode,
                Gpa = entity.Gpa,
                Sex = entity.Sex
            };
        }
    }
}
