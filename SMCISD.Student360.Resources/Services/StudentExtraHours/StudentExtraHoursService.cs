using SMCISD.Student360.Persistence.Commands;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Services.Reasons;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentExtraHours
{
    public interface IStudentExtraHoursService : IGridData
    {
        Task<GridResponse> GetCurrentStudentExtraHours(GridRequest request, IPrincipal currentUser);
        Task<GridResponse> GetHistoryDataById(GridRequest request, IPrincipal currentUser);
        Task<StudentExtraHoursModel> CreateStudentExtraHours(StudentExtraHoursModel data, IPrincipal currentUser);
        Task<StudentExtraHoursModel> UpdateStudentExtraHours(StudentExtraHourGridModel data, IPrincipal currentUser);
        Task<List<Persistence.Models.StudentExtraHours>> CreateStudentExtraHourBulk(List<Persistence.Models.StudentExtraHours> studentExtraHours);
        Task<List<StudentExtraHoursModel>> ImportStudentExtraHours(List<StudentExtraHoursModel> studentExtraHours, IPrincipal currentUser);
        Task<List<StudentExtraHoursModel>> UpdateBulkStudentExtraHours(List<StudentExtraHourGridModel> list, IPrincipal currentUser);
    }

    public class StudentExtraHoursService : IStudentExtraHoursService
    {
        private readonly IStudentExtraHoursQueries _queries;
        private readonly IStudentExtraHoursCommands _commands;
        public StudentExtraHoursService(IStudentExtraHoursQueries queries, IStudentExtraHoursCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            return await _queries.GetGridData(request, currentUser);
        }

        public async Task<GridResponse> GetHistoryDataById(GridRequest request, IPrincipal currentUser)
        {
            request.AllData = true;
            return await _queries.GetHistoryDataById(request, currentUser);
        }

        public async Task<GridResponse> GetCurrentStudentExtraHours(GridRequest request, IPrincipal currentUser)
        {
            request.AllData = true;
            return await _queries.GetCurrentStudentExtraHours(request, currentUser);
        }

        public async Task<StudentExtraHoursModel> CreateStudentExtraHours(StudentExtraHoursModel data, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            
            data.UserRole = claims.First(x => x.Type.Contains("role")).Value;
            data.UserCreatedUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
            data.UserFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
            data.UserLastSurname = claims.First(x => x.Type.Contains("lastsurname")).Value;

            var newEntity = MapStudentExtraHoursModelToStudentExtraHoursEntity(data);

            var entity = await _commands.CreateStudentExtraHours(newEntity);

            return MapStudentExtraHoursEntityToStudentExtraHoursModel(entity);
        }


        public async Task<StudentExtraHoursModel> UpdateStudentExtraHours(StudentExtraHourGridModel data, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            data.UserRole = claims.First(x => x.Type.Contains("role")).Value;
            data.UserCreatedUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
            data.UserFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
            data.UserLastSurname = claims.First(x => x.Type.Contains("lastsurname")).Value;

            var newEntity = MapStudentExtraHourGridModelToStudentExtraHoursEntity(data);

            var entity = await _commands.UpdateStudentExtraHours(newEntity, currentUser);

            return MapStudentExtraHoursEntityToStudentExtraHoursModel(entity);
        }

        public async Task<List<StudentExtraHoursModel>> UpdateBulkStudentExtraHours(List<StudentExtraHourGridModel> list, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            foreach(var data in list)
            {
                data.UserRole = claims.First(x => x.Type.Contains("role")).Value;
                data.UserCreatedUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
                data.UserFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
                data.UserLastSurname = claims.First(x => x.Type.Contains("lastsurname")).Value;
            }
           

            var entityList =  list.Select(x => MapStudentExtraHourGridModelToStudentExtraHoursEntity(x)).ToList();

            var result = await _commands.UpdateBulkStudentExtraHours(entityList, currentUser);



            return  result.Select( x => MapStudentExtraHoursEntityToStudentExtraHoursModel(x)).ToList();
        }

        public async Task<List<StudentExtraHoursModel>> ImportStudentExtraHours(List<StudentExtraHoursModel> studentExtraHours, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            var role = claims.First(x => x.Type.Contains("role")).Value;
            var userUniqueId = claims.First(x => x.Type.Contains("person_unique_id")).Value;
            var userFirstName = claims.First(x => x.Type.Contains("firstname")).Value;
            var userLastsurname = claims.First(x => x.Type.Contains("lastsurname")).Value;

            foreach (var student in studentExtraHours)
            {
                student.UserRole = role;
                student.UserCreatedUniqueId = userUniqueId;
                student.UserFirstName = userFirstName;
                student.UserLastSurname = userLastsurname;
            }

            var entities = await _commands.ImportStudentExtraHours(studentExtraHours.Select(x => MapStudentExtraHoursModelToStudentExtraHoursEntity(x)).ToList());

            return entities.Select(x => MapStudentExtraHoursEntityToStudentExtraHoursModel(x)).ToList();
        }
        public async Task<List<Persistence.Models.StudentExtraHours>> CreateStudentExtraHourBulk(List<Persistence.Models.StudentExtraHours> studentExtraHours)
        {
            return await _commands.CreateStudentExtraHourBulk(studentExtraHours);
        }

        private Persistence.Models.StudentExtraHours MapStudentExtraHoursModelToStudentExtraHoursEntity(StudentExtraHoursModel model)
        {
            if (model == null)
                return null;

            return new Persistence.Models.StudentExtraHours
            {
                StudentExtraHoursId = model.StudentExtraHoursId,
                Version = model.Version,
                StudentUniqueId = model.StudentUniqueId,
                GradeLevel = model.GradeLevel,
                FirstName = model.FirstName,
                LastSurname = model.LastSurname,
                Date = model.Date,
                Hours = model.Hours,
                UserCreatedUniqueId = model.UserCreatedUniqueId,
                UserRole = model.UserRole,
                CreateDate = model.CreateDate,
                UserLastSurname = model.UserLastSurname,
                UserFirstName = model.UserFirstName,
                SchoolYear = model.SchoolYear,
                ReasonId = model.ReasonId,
                Reason = MapReasonModelToEntity(model.Reason),
                Comments = model.Comments,
                Id = model.Id
            };
        }

        private StudentExtraHoursModel MapStudentExtraHoursEntityToStudentExtraHoursModel(Persistence.Models.StudentExtraHours entity)
        {
            return new StudentExtraHoursModel
            {
                StudentExtraHoursId = entity.StudentExtraHoursId,
                Version = entity.Version,
                StudentUniqueId = entity.StudentUniqueId,
                GradeLevel = entity.GradeLevel,
                FirstName = entity.FirstName,
                LastSurname = entity.LastSurname,
                Date = entity.Date,
                Hours = entity.Hours,
                UserCreatedUniqueId = entity.UserCreatedUniqueId,
                UserRole = entity.UserRole,
                CreateDate = entity.CreateDate,
                UserLastSurname = entity.UserLastSurname,
                UserFirstName = entity.UserFirstName,
                SchoolYear = entity.SchoolYear,
                ReasonId = entity.ReasonId,
                Reason = MapReasonEntityToModel(entity.Reason),
                Comments = entity.Comments,
                Id = entity.Id
            };
        }

        private Persistence.Models.StudentExtraHours MapStudentExtraHourGridModelToStudentExtraHoursEntity(StudentExtraHourGridModel model)
        {
            if (model == null)
                return null;

            return new Persistence.Models.StudentExtraHours
            {
                StudentExtraHoursId = model.StudentExtraHoursId,
                Version = model.Version,
                StudentUniqueId = model.StudentUniqueId,
                GradeLevel = model.GradeLevel,
                FirstName = model.FirstName,
                LastSurname = model.LastSurname,
                Date = model.Date,
                Hours = model.Hours,
                UserCreatedUniqueId = model.UserCreatedUniqueId,
                UserRole = model.UserRole,
                UserLastSurname = model.UserLastSurname,
                UserFirstName = model.UserFirstName,
                SchoolYear = model.SchoolYear,
                Comments = model.Comments,
                ReasonId = model.ReasonId,
                Reason = new Persistence.Models.Reasons { Value = model.Reason },
                Id = model.Id
            };
        }
        private Persistence.Models.Reasons MapReasonModelToEntity(ReasonsModel model)
        {
            if (model == null)
                return null;

            return new Persistence.Models.Reasons
            {
                Value = model.Value,
                ReasonId = model.ReasonId,
                Description = model.Description,
                CreateDate = model.CreateDate
            };
        }

        private ReasonsModel MapReasonEntityToModel(Persistence.Models.Reasons entity)
        {
            if (entity == null)
                return null;

            return new ReasonsModel
            {
                Value = entity.Value,
                ReasonId = entity.ReasonId,
                Description = entity.Description,
                CreateDate = entity.CreateDate
            };
        }

    }
}
