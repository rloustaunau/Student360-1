using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IPeopleQueries 
    {
        Task<List<People>> Get(string email);
        Task<List<People>> GetStaffByName(string name);
        Task<People> GetByUSI(int usi);
    }

    public class PeopleQueries : IPeopleQueries
    {
        private readonly Student360Context _db;

        public PeopleQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<People>> Get(string email) {
           return await _db.People.Where(x => x.ElectronicMailAddress == email && x.PositionTitle != null && x.AccessLevel != null).ToListAsync();
        }

        public async Task<List<People>> GetStaffByName(string name)
        {
            return await _db.People.Where(x => (x.FirstName == name || x.LastSurname==name) && x.PersonType=="Staff" && x.PositionTitle != null && x.AccessLevel != null).ToListAsync();
        }

        public async Task<People> GetByUSI(int usi)
        {
            return await _db.People.FirstOrDefaultAsync(x => x.Usi == usi);
        }
    }
}
