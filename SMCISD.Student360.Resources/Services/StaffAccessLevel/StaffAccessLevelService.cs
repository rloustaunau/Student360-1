using SMCISD.Student360.Persistence.Commands;
using SMCISD.Student360.Resources.Services.People;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StaffAccessLevel
{
    public interface IStaffAccessLevelService
    {
        Task<List<StaffAccessLevelModel>> Get();
    }

    public class StaffAccessLevelService : IStaffAccessLevelService
    {
        private readonly IStaffAccessLevelCommands _commands;
        private readonly IPeopleService _peopleService;
        public StaffAccessLevelService(IStaffAccessLevelCommands commands, IPeopleService peopleService)
        {
            _commands = commands;
            _peopleService = peopleService;
        }

        public async Task<List<StaffAccessLevelModel>> Get()
        {
            var entityList = await _commands.Get();

            return entityList.Select(x => MapStaffAccessLevelEntityToStaffAccessLevelModel(x)).ToList();
        }

        private Persistence.Models.StaffAccessLevel MapStaffAccessLevelModelToStaffAccessLevelEntity(StaffAccessLevelModel model)
        {
            if (model == null)
                return null;

            return new Persistence.Models.StaffAccessLevel
            {
                Id = model.Id,
                Description = model.Description,
                IsAdmin=model.IsAdmin,
                StaffClassificationDescriptorId=model.StaffClassificationDescriptorId
            };
        }

        private StaffAccessLevelModel MapStaffAccessLevelEntityToStaffAccessLevelModel(Persistence.Models.StaffAccessLevel entity)
        {
            return new StaffAccessLevelModel
            {
                Id = entity.Id,
                Description=entity.Description,
                IsAdmin=entity.IsAdmin,
                StaffClassificationDescriptorId=entity.StaffClassificationDescriptorId
            };
        }
    }
}
