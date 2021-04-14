using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SMCISD.Student360.Persistence.Queries;

namespace SMCISD.Student360.Persistence.Commands
{
    public interface IStaffEducationOrganizationAssignmentAssociationCommands
    {
        Task<List<StaffEducationOrganizationAssignmentAssociation>> GetStaffUSI(int staffUSI);
        Task<StaffEducationOrganizationAssignmentAssociation> Add(StaffEducationOrganizationAssignmentAssociation staff);
        Task<StaffEducationOrganizationAssignmentAssociation> Update(StaffEducationOrganizationAssignmentAssociation staff);
        Task<StaffEducationOrganizationAssignmentAssociation> Delete(StaffEducationOrganizationAssignmentAssociation staff);
    }

    public class StaffEducationOrganizationAssignmentAssociationCommands : IStaffEducationOrganizationAssignmentAssociationCommands

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public StaffEducationOrganizationAssignmentAssociationCommands(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }

        

        public async Task<List<StaffEducationOrganizationAssignmentAssociation>> GetStaffUSI(int staffUSI)
        {
            List<StaffEducationOrganizationAssignmentAssociation> list = new List<StaffEducationOrganizationAssignmentAssociation>();

            try
            {
                list=await _db.StaffEducationOrganizationAssignmentAssociation.Where(m => m.StaffUSI == staffUSI).OrderBy(m=> m.EmploymentStatusDescriptorId).ToListAsync();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            return list;          
        }

        public async Task<StaffEducationOrganizationAssignmentAssociation> Add(StaffEducationOrganizationAssignmentAssociation staff)
        {
            _db.StaffEducationOrganizationAssignmentAssociation.Add(staff);
            await _db.SaveChangesAsync();

            if (staff.StaffClassificationDescriptorId == 40789) { 
                var people= await _db.People.FirstOrDefaultAsync(x => x.Usi == staff.StaffUSI);
                if (people != null) {
                    var accessLevel = await _db.AccessLevelDefinition.FirstOrDefaultAsync(m => m.Email == people.ElectronicMailAddress);
                    if (accessLevel == null) {
                        AccessLevelDefinition newAccess = new AccessLevelDefinition()
                        {
                            Email=people.ElectronicMailAddress
                        };
                        _db.AccessLevelDefinition.Add(newAccess);
                        await _db.SaveChangesAsync();
                    }
                }
            }

            return staff;
        }

        public async Task<StaffEducationOrganizationAssignmentAssociation> Update(StaffEducationOrganizationAssignmentAssociation staff)
        {
             _db.StaffEducationOrganizationAssignmentAssociation.Update(staff);
             await _db.SaveChangesAsync();

            if (staff.StaffClassificationDescriptorId == 40789)
            {
                var people = await _db.People.FirstOrDefaultAsync(x => x.Usi == staff.StaffUSI);
                if (people != null)
                {
                    var accessLevel = await _db.AccessLevelDefinition.FirstOrDefaultAsync(m => m.Email == people.ElectronicMailAddress);
                    if (accessLevel == null)
                    {
                        AccessLevelDefinition newAccess = new AccessLevelDefinition()
                        {
                            Email = people.ElectronicMailAddress
                        };
                        _db.AccessLevelDefinition.Add(newAccess);
                        await _db.SaveChangesAsync();
                    }
                }
            }


            return staff;
        }

        public async Task<StaffEducationOrganizationAssignmentAssociation> Delete(StaffEducationOrganizationAssignmentAssociation staff)
        {
            _db.StaffEducationOrganizationAssignmentAssociation.Remove(staff);
            await _db.SaveChangesAsync();

            return staff;
        }

    }
}
