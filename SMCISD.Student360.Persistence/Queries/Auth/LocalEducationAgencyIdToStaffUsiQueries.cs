using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System.Linq;

namespace SMCISD.Student360.Persistence.Queries.Auth
{
    public interface ILocalEducationAgencyIdToStaffUsiQueries
    {
        IQueryable<LocalEducationAgencyIdToStaffUsi> GetStaffLocalEducationAgencies(int staffUsi);
    }

    public class LocalEducationAgencyIdToStaffUsiQueries : ILocalEducationAgencyIdToStaffUsiQueries

    {
        private readonly Student360Context _db;

        public LocalEducationAgencyIdToStaffUsiQueries(Student360Context db)
        {
            _db = db;
        }
        public IQueryable<LocalEducationAgencyIdToStaffUsi> GetStaffLocalEducationAgencies(int staffUsi) =>  _db.LocalEducationAgencyIdToStaffUsi.Where(x => x.StaffUsi == staffUsi);

    }
}
