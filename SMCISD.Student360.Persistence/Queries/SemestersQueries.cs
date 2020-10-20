using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface ISemestersQueries 
    {
        Task<List<Semesters>> Get();  
    }

    public class SemestersQueries : ISemestersQueries

    {
        private readonly Student360Context _db;

        public SemestersQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<Semesters>> Get() => await _db.Semesters.ToListAsync();
    }
}
