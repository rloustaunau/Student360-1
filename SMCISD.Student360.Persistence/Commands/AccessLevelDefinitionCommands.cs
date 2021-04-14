using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Commands
{
    public interface IAccessLevelDefinitionCommands
    {
        Task<AccessLevelDefinition> GetByEmail(string email);
    }

    public class AccessLevelDefinitionCommands : IAccessLevelDefinitionCommands

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public AccessLevelDefinitionCommands(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }

        

        public async Task<AccessLevelDefinition> GetByEmail(string email)
        {
            AccessLevelDefinition access= new AccessLevelDefinition();
            access = await _db.AccessLevelDefinition.FirstOrDefaultAsync(m => m.Email==email );
            
            return access;
        }

        public async Task<AccessLevelDefinition> Add(AccessLevelDefinition staff)
        {
            _db.AccessLevelDefinition.Add(staff);
            await _db.SaveChangesAsync();

            return staff;
        }

        public async Task<AccessLevelDefinition> Delete(AccessLevelDefinition staff)
        {
            _db.AccessLevelDefinition.Remove(staff);
            await _db.SaveChangesAsync();

            return staff;
        }

    }
}
