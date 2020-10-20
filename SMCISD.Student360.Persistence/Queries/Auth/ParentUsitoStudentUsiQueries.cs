using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries.Auth
{
    public interface IParentUsitoStudentUsiQueries 
    {
        IQueryable<ParentUsitoStudentUsi> GetParentStudents(int parentUsi);  
    }

    public class ParentUsitoStudentUsiQueries : IParentUsitoStudentUsiQueries

    {
        private readonly Student360Context _db;

        public ParentUsitoStudentUsiQueries(Student360Context db)
        {
            _db = db;
        }
        public IQueryable<ParentUsitoStudentUsi> GetParentStudents(int parentUsi) => _db.ParentUsitoStudentUsi.Where(x => x.ParentUsi == parentUsi);
    }
}
