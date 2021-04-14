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

namespace SMCISD.Student360.Resources.Services.AccessLevelDefinition
{
    public interface IAccessLevelDefinitionService
    {
        Task<AccessLevelDefinitionModel> GetByEmail(string email, IPrincipal currentUser);
    }

    public class AccessLevelDefinitionService : IAccessLevelDefinitionService
    {
        private readonly IAccessLevelDefinitionCommands _commands;
        private readonly IPeopleService _peopleService;
        public AccessLevelDefinitionService(IAccessLevelDefinitionCommands commands, IPeopleService peopleService)
        {
            _commands = commands;
            _peopleService = peopleService;
        }

        public async Task<AccessLevelDefinitionModel> GetByEmail(string email, IPrincipal currentUser)
        {
            var entity = await _commands.GetByEmail(email);
            if (entity == null)
                return null;

            return MapAccessLevelDefinitionEntityToAccessLevelDefinitionModel(entity);
        }

        private Persistence.Models.AccessLevelDefinition MapAccessLevelDefinitionModelToAccessLevelDefinitionEntity(AccessLevelDefinitionModel model)
        {
            if (model == null)
                return null;

            return new Persistence.Models.AccessLevelDefinition
            {
                Id = model.Id,
                Email = model.Email
            };
        }

        private AccessLevelDefinitionModel MapAccessLevelDefinitionEntityToAccessLevelDefinitionModel(Persistence.Models.AccessLevelDefinition entity)
        {
            return new AccessLevelDefinitionModel
            {
                Id = entity.Id,
                Email = entity.Email
            };
        }
    }
}
