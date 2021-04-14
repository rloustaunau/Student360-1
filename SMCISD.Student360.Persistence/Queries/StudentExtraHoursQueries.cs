using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentExtraHoursQueries : IGridData
    {
        Task<GridResponse> GetHistoryDataById(GridRequest request, IPrincipal currentUser);
        Task<GridResponse> GetCurrentStudentExtraHours(GridRequest request, IPrincipal currentUser);  
    }

    public class StudentExtraHoursQueries : IStudentExtraHoursQueries

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public StudentExtraHoursQueries(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }
        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            // Gets a list of all the last versions of the student extra hours.
            var query = _db.StudentExtraHourGrid;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentExtraHourGrid());

            return await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);
        }
        public async Task<GridResponse> GetCurrentStudentExtraHours(GridRequest request, IPrincipal currentUser)
        {
            // Gets a list of a full historic records of a student extra hour.
            // GridRequest.Filters must have a studentusi filter assigned to it.
            var query = _db.StudentExtraHourCurrentGrid;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentExtraHourCurrentGrid());

            var grid=await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);

            foreach (StudentExtraHourCurrentGrid obj in grid.Data.ToList()) {
                obj.Date = obj.Date.Substring(0, 10);
            }

            return grid;
        }

        public async Task<GridResponse> GetHistoryDataById(GridRequest request, IPrincipal currentUser)
        {
            // Gets a list of a full historic records of a student extra hour.
            // GridRequest.Filters must have a studentusi filter assigned to it.
            var query = _db.StudentExtraHourHistory;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentExtraHourHistory());

            return await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);
        }
        
    }
}
