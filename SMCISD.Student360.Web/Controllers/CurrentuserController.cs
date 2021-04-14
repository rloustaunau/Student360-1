using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMCISD.Student360.Resources.Services.AccessToSystem;
using SMCISD.Student360.Resources.Services.AccessLevelDefinition;
using SMCISD.Student360.Resources.Services.People;
using System.Collections.Generic;
using SMCISD.Student360.Resources.Services.StaffEducationOrganizationAssignmentAssociation;
using SMCISD.Student360.Resources.Services.StaffAccessLevel;

namespace SMCISD.Student360.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrentUserController : ControllerBase
    {
        private readonly IPeopleService _peopleService;
        private readonly IAccessToSystemService _accessToSystemService;
        private readonly IAccessLevelDefinitionService _accessLevelDefinitionService;
        private readonly IStaffEducationOrganizationAssignmentAssociationService _staffEducationOrganizationAssignmentAssociationService;
        private readonly IStaffAccessLevelService _staffAccessLevelService;
        public CurrentUserController(IPeopleService peopleService, 
            IAccessToSystemService accessToSystemService, 
            IAccessLevelDefinitionService accessLevelDefinitionService,
            IStaffEducationOrganizationAssignmentAssociationService staffEducationOrganizationAssignmentAssociationService,
            IStaffAccessLevelService staffAccessLevelService)
        {
            _peopleService = peopleService;
            _accessToSystemService = accessToSystemService;
            _accessLevelDefinitionService = accessLevelDefinitionService;
            _staffEducationOrganizationAssignmentAssociationService = staffEducationOrganizationAssignmentAssociationService;
            _staffAccessLevelService = staffAccessLevelService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<PeopleModel>>  GetProfile()
        {
            return await _peopleService.Get(User.FindFirst(x => x.Type.Contains("email")).Value);
        }

        [HttpGet("getStaffAccessLevel")]
        public async Task<ActionResult<List<StaffAccessLevelModel>>> GetStaffAccessLevel()
        {
            return await _staffAccessLevelService.Get();
        }

        [HttpGet("getStaffByName/{name}")]
        public async Task<ActionResult<List<PeopleModel>>> GetStaffByName(string name)
        {
            return await _peopleService.GetStaffByName(name);
        }

        [HttpGet("getAssignmentByStaffUsi/{usi}")]
        public async Task<ActionResult<List<StaffEducationOrganizationAssignmentAssociationModel>>> GetAssignmentByStaffUsi(int usi)
        {
            return await _staffEducationOrganizationAssignmentAssociationService.GetByStaffUSI(usi);
        }

        [HttpPost("createAccess")]
        public async Task<ActionResult<AccessToSystemModel>> AddAccessToSystem([FromBody] AccessToSystemModel data)
        {
            return await _accessToSystemService.CreateAccessToSystem(data,User);
        }

        [HttpPost("createAssignmentAssociation")]
        public async Task<ActionResult<StaffEducationOrganizationAssignmentAssociationModel>> CreateAssignmentAssociation([FromBody] StaffEducationOrganizationAssignmentAssociationModel data)
        {
            return await _staffEducationOrganizationAssignmentAssociationService.Save(data);
        }

        [HttpPost("deleteAssignmentAssociation")]
        public async Task<ActionResult<StaffEducationOrganizationAssignmentAssociationModel>> DeleteAssignmentAssociation([FromBody] StaffEducationOrganizationAssignmentAssociationModel data)
        {
            return await _staffEducationOrganizationAssignmentAssociationService.Delete(data);
        }

        [HttpGet("accessLevel")]
        public async Task<ActionResult<AccessLevelDefinitionModel>> GetAccessLevel()
        {
            return await _accessLevelDefinitionService.GetByEmail(User.FindFirst(x => x.Type.Contains("email")).Value, User);
        }
    }
}
