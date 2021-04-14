using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IAdadistrictQueries 
    {
        Task<List<Adadistrict>> Get();  
    }

    public class AdadistrictQueries : IAdadistrictQueries
    {
        private readonly Student360Context _db;

        public AdadistrictQueries(Student360Context db)
        {
            _db = db;
        }
        public async Task<List<Adadistrict>> Get() => await _db.Adadistrict.ToListAsync();
    }
}
