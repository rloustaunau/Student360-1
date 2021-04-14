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
    public interface IAccessToSystemCommands
    {
        Task<AccessToSystem> CreateAccessToSystem(AccessToSystem data);
    }

    public class AccessToSystemCommands : IAccessToSystemCommands

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public AccessToSystemCommands(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }

        

        public async Task<AccessToSystem> CreateAccessToSystem(AccessToSystem data)
        {
            AccessToSystem access= new AccessToSystem();
            access = await _db.AccessToSystem.FirstOrDefaultAsync(m => m.LastLogin.Date == data.LastLogin.Date && m.LastLogin.Hour==data.LastLogin.Hour && m.LastLogin.Minute==data.LastLogin.Minute  && m.Email==data.Email);

            if (access==null)
            {
                using (var transaction = _db.Database.BeginTransaction())
                {
                    if (data.Id != 0)
                        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AccessToSystem] On");

                    _db.AccessToSystem.Add(data);
                    await _db.SaveChangesAsync();

                    if (data.Id != 0)
                        await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AccessToSystem] Off");

                    await transaction.CommitAsync();

                }
            }
            return data;
        }

       
    }
}
