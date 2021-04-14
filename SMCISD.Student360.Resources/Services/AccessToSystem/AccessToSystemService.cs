using SMCISD.Student360.Persistence.Commands;
using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Resources.Services.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.AccessToSystem
{
    public interface IAccessToSystemService
    {
        Task<AccessToSystemModel> CreateAccessToSystem(AccessToSystemModel data, IPrincipal currentUser);

    }

    public class AccessToSystemService : IAccessToSystemService
    {
        private readonly IAccessToSystemCommands _commands;
        private readonly IPeopleService _peopleService;
        public AccessToSystemService(IAccessToSystemCommands commands, IPeopleService peopleService)
        {
            _commands = commands;
            _peopleService = peopleService;
        }

        public async Task<AccessToSystemModel> CreateAccessToSystem(AccessToSystemModel data, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            data.LastLogin = DateTime.Now;
            data.FullName= claims.First(x => x.Type.Contains("firstname")).Value;
            data.SchoolCode = claims.First(x => x.Type.Contains("person_unique_id")).Value;

            /* try
             {
                 var people = await _peopleService.Get(data.Email);
                 data.SchoolCode = people.SchoolId.Value.ToString(); //claims.First(x => x.Type.Contains("person_unique_id")).Value;
             }
             catch (Exception) {
                 data.SchoolCode = "";
             }*/

            
            var newEntity = MapAccessToSystemModelToAccessToSystemEntity(data);
            try
            {
                var entity = await _commands.CreateAccessToSystem(newEntity);
                return MapAccessToSystemEntityToAccessToSystemModel(entity);
            }
            catch (Exception e) {
                Console.Write(e);
            }

            return null;
        }

        private Persistence.Models.AccessToSystem MapAccessToSystemModelToAccessToSystemEntity(AccessToSystemModel model)
        {
            if (model == null)
                return null;

            return new Persistence.Models.AccessToSystem
            {
                Id = model.Id,
                Email = model.Email,
                FullName = model.FullName,
                LastLogin = model.LastLogin,
                SchoolCode = model.SchoolCode
            };
        }

        private AccessToSystemModel MapAccessToSystemEntityToAccessToSystemModel(Persistence.Models.AccessToSystem entity)
        {
            return new AccessToSystemModel
            {
                Id = entity.Id,
                Email = entity.Email,
                FullName = entity.FullName,
                LastLogin = entity.LastLogin,
                SchoolCode = entity.SchoolCode
            };
        }
    }
}
