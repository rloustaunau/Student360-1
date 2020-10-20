using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IGradeQueries 
    {
        Task<List<Grade>> Get();  
    }

    public class GradeQueries : IGradeQueries

    {
        private readonly Student360Context _db;

        public GradeQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<Grade>> Get() => await _db.Grade.ToListAsync();
    }
}
