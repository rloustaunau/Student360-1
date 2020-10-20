using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Services.Adadistrict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.Adacampus
{
    public interface IAdacampusService 
    {
        Task<List<AdacampusModel>> Get();
        Task<List<AdacampusModel>> GetAdaYtdRates();
    }

    public class AdacampusService : IAdacampusService
    {
        private readonly IAdacampusQueries _queries;
        private readonly IAdadistrictService _adadistrictService;
        public AdacampusService(IAdacampusQueries queries, IAdadistrictService adadistrictService)
        {
            _queries = queries;
            _adadistrictService = adadistrictService;
        }

        public async Task<List<AdacampusModel>> Get()
        {
            var entityList = await _queries.Get();

            return entityList.Select(x => MapAdacampusEntityToAdacampusModel(x)).ToList();
        }
  
        public async Task<List<AdacampusModel>> GetAdaYtdRates()
        {
            var districtmodelList = await _adadistrictService.Get();
            var resultList = new List<AdacampusModel>();
            var campusList = await Get();

            var highSchools = campusList.Where(x => x.NameOfInstitution.Contains("High")).OrderBy(x => x.NameOfInstitution);
            var middleSchools = campusList.Where(x => x.NameOfInstitution.Contains("Middle")).OrderBy(x => x.NameOfInstitution);
            var elementarySchools = campusList.Where(x => x.NameOfInstitution.Contains("Elementary")).OrderBy(x => x.NameOfInstitution);
            var pkSchools = campusList.Where(x => x.NameOfInstitution.Contains("PK")).OrderBy(x => x.NameOfInstitution);
            resultList.AddRange(highSchools);
            resultList.AddRange(middleSchools);
            resultList.AddRange(elementarySchools);
            resultList.AddRange(pkSchools);

            return resultList;
        }

      private Persistence.Models.Adacampus MapAdacampusModelToAdacampusEntity(AdacampusModel model)
        {
            return new Persistence.Models.Adacampus
            {
                NameOfInstitution = model.NameOfInstitution,
                StudentAttendance = model.StudentAttendance,
                MaxStudentAttendance = model.MaxStudentAttendance,
                AttendancePercent = model.AttendancePercent
            };
        }

        private AdacampusModel MapAdacampusEntityToAdacampusModel(Persistence.Models.Adacampus entity)
        {
            return new AdacampusModel
            {
                NameOfInstitution = entity.NameOfInstitution,
                StudentAttendance = entity.StudentAttendance,
                MaxStudentAttendance = entity.MaxStudentAttendance,
                AttendancePercent = entity.AttendancePercent
            };
        }
    }
}
