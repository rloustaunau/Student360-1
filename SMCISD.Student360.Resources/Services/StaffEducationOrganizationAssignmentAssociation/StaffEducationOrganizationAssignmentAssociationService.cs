using SMCISD.Student360.Persistence.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StaffEducationOrganizationAssignmentAssociation
{
    public interface IStaffEducationOrganizationAssignmentAssociationService
    {
        Task<List<StaffEducationOrganizationAssignmentAssociationModel>> GetByStaffUSI(int staffUsi);
        Task<StaffEducationOrganizationAssignmentAssociationModel> Save(StaffEducationOrganizationAssignmentAssociationModel staff);
        Task<StaffEducationOrganizationAssignmentAssociationModel> Delete(StaffEducationOrganizationAssignmentAssociationModel staff);
    }

    public class StaffEducationOrganizationAssignmentAssociationService : IStaffEducationOrganizationAssignmentAssociationService
    {
        private readonly IStaffEducationOrganizationAssignmentAssociationCommands _queries;
        public StaffEducationOrganizationAssignmentAssociationService(IStaffEducationOrganizationAssignmentAssociationCommands queries)
        {
            _queries = queries;
        }

        public async Task<List<StaffEducationOrganizationAssignmentAssociationModel>> GetByStaffUSI(int staffUsi)
        {
            var entityList = await _queries.GetStaffUSI(staffUsi);

            return entityList.Select(x => MapStaffEducationOrganizationAssignmentAssociationEntityToStaffEducationOrganizationAssignmentAssociationModel(x)).ToList();
        }
        public async Task<StaffEducationOrganizationAssignmentAssociationModel> Save(StaffEducationOrganizationAssignmentAssociationModel staff)
        {
            var entity = MapStaffEducationOrganizationAssignmentAssociationModelToStaffEducationOrganizationAssignmentAssociationEntity(staff);

            Persistence.Models.StaffEducationOrganizationAssignmentAssociation result = new Persistence.Models.StaffEducationOrganizationAssignmentAssociation();

            entity.LastModifiedDate = DateTime.Now; 

            if (entity.Id == null)
            {
                entity.CreateDate = DateTime.Now;
                result = await _queries.Add(entity);
            }
            else {
                result = await _queries.Update(entity);
            }

            var model= MapStaffEducationOrganizationAssignmentAssociationEntityToStaffEducationOrganizationAssignmentAssociationModel(result);
            return model;
        }

        public async Task<StaffEducationOrganizationAssignmentAssociationModel> Delete(StaffEducationOrganizationAssignmentAssociationModel staff)
        {
            var entity = MapStaffEducationOrganizationAssignmentAssociationModelToStaffEducationOrganizationAssignmentAssociationEntity(staff);

            Persistence.Models.StaffEducationOrganizationAssignmentAssociation result = new Persistence.Models.StaffEducationOrganizationAssignmentAssociation();

            result = await _queries.Delete(entity);

            var model = MapStaffEducationOrganizationAssignmentAssociationEntityToStaffEducationOrganizationAssignmentAssociationModel(result);
            return model;
        }
        private Persistence.Models.StaffEducationOrganizationAssignmentAssociation MapStaffEducationOrganizationAssignmentAssociationModelToStaffEducationOrganizationAssignmentAssociationEntity(StaffEducationOrganizationAssignmentAssociationModel model)
        {
            return new Persistence.Models.StaffEducationOrganizationAssignmentAssociation
            {
                Id = model.Id,
                BeginDate = model.BeginDate,
                EducationOrganizationId = model.EducationOrganizationId,
                StaffClassificationDescriptorId = model.StaffClassificationDescriptorId,
                StaffUSI = model.StaffUSI,
                PositionTitle = model.PositionTitle,
                EndDate = model.EndDate,
                OrderOfAssignment = model.OrderOfAssignment,
                EmploymentEducationOrganizationId = model.EmploymentEducationOrganizationId,
                EmploymentStatusDescriptorId = model.EmploymentStatusDescriptorId,
                EmploymentHireDate = model.EmploymentHireDate,
                CredentialIdentifier = model.CredentialIdentifier,
                StateOfIssueStateAbbreviationDescriptorId = model.StateOfIssueStateAbbreviationDescriptorId,
                Discriminator = model.Discriminator,
                CreateDate = model.CreateDate,
                LastModifiedDate = model.LastModifiedDate
            };
        }

        private StaffEducationOrganizationAssignmentAssociationModel MapStaffEducationOrganizationAssignmentAssociationEntityToStaffEducationOrganizationAssignmentAssociationModel(Persistence.Models.StaffEducationOrganizationAssignmentAssociation entity)
        {
            return new StaffEducationOrganizationAssignmentAssociationModel
            {
                Id = entity.Id,
                BeginDate = entity.BeginDate,
                EducationOrganizationId = entity.EducationOrganizationId,
                StaffClassificationDescriptorId = entity.StaffClassificationDescriptorId,
                StaffUSI = entity.StaffUSI,
                PositionTitle = entity.PositionTitle,
                EndDate = entity.EndDate,
                OrderOfAssignment = entity.OrderOfAssignment,
                EmploymentEducationOrganizationId = entity.EmploymentEducationOrganizationId,
                EmploymentStatusDescriptorId = entity.EmploymentStatusDescriptorId,
                EmploymentHireDate = entity.EmploymentHireDate,
                CredentialIdentifier = entity.CredentialIdentifier,
                StateOfIssueStateAbbreviationDescriptorId = entity.StateOfIssueStateAbbreviationDescriptorId,
                Discriminator = entity.Discriminator,
                CreateDate = entity.CreateDate,
                LastModifiedDate = entity.LastModifiedDate
            };
        }
    }
}
