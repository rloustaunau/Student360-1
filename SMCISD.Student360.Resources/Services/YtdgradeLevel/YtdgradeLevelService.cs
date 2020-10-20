using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Services.Adadistrict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.YtdgradeLevel
{
    public interface IYtdgradeLevelService 
    {
        Task<List<YtdgradeLevelModel>> Get();
    }

    public class YtdgradeLevelService : IYtdgradeLevelService
    {
        private readonly IYtdgradeLevelQueries _queries;
        private readonly IAdadistrictService _adadistrictService;
        public YtdgradeLevelService(IYtdgradeLevelQueries queries, IAdadistrictService adadistrictService)
        {
            _queries = queries;
            _adadistrictService = adadistrictService;
        }

        public async Task<List<YtdgradeLevelModel>> Get()
        {
            var entityList = await _queries.Get();

            var firstGrades = new string[]{"EE", "PK", "KG"};
            var ee = entityList.Where(x => x.GradeLevel == "EE");
            var pk = entityList.Where(x => x.GradeLevel == "PK");
            var kg = entityList.Where(x => x.GradeLevel == "KG");
            var allOtherGrades = entityList.Where(x => !firstGrades.Contains(x.GradeLevel));
            var allGrades = ee.Concat(pk).Concat(kg).Concat(allOtherGrades);

            return allGrades.Select(x => MapYtdgradeLevelEntityToYtdgradeLevelModel(x)).ToList();
        }
      private Persistence.Models.YtdgradeLevel MapYtdgradeLevelModelToYtdgradeLevelEntity(YtdgradeLevelModel model)
        {
            return new Persistence.Models.YtdgradeLevel
            {
                GradeLevel = model.GradeLevel,
                StudentAttendance = model.StudentAttendance,
                MaxStudentAttendance = model.MaxStudentAttendance,
                AttendancePercent = model.AttendancePercent,
                SchoolYear = model.SchoolYear
            };
        }

        private YtdgradeLevelModel MapYtdgradeLevelEntityToYtdgradeLevelModel(Persistence.Models.YtdgradeLevel entity)
        {
            return new YtdgradeLevelModel
            {
                GradeLevel = entity.GradeLevel,
                StudentAttendance = entity.StudentAttendance,
                MaxStudentAttendance = entity.MaxStudentAttendance,
                AttendancePercent = entity.AttendancePercent,
                SchoolYear = entity.SchoolYear
            };
        }
    }
}
