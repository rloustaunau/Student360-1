using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Linq;

namespace SMCISD.Student360.Persistence.Queries.Auth
{
    public interface ISchoolIdToStaffUsiQueries
    {
        IQueryable<SchoolIdToStaffUsi> GetStaffSchools(int staffUsi);
    }

    public class SchoolIdToStaffUsiQueries : ISchoolIdToStaffUsiQueries

    {
        private readonly Student360Context _db;

        public SchoolIdToStaffUsiQueries(Student360Context db)
        {
            _db = db;
        }
        public IQueryable<SchoolIdToStaffUsi> GetStaffSchools(int staffUsi) =>  _db.SchoolIdToStaffUsi.Where(x => x.StaffUsi == staffUsi);

    }
}
