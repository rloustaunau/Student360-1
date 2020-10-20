using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IAdacampusQueries 
    {
        Task<List<Adacampus>> Get();  
    }

    public class AdacampusQueries : IAdacampusQueries

    {
        private readonly Student360Context _db;

        public AdacampusQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<Adacampus>> Get() => await _db.Adacampus.ToListAsync();
    }
}
