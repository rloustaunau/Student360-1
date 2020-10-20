using Newtonsoft.Json;
using SMCISD.Student360.Persistence.Queries.Auth;
using SMCISD.Student360.Persistence.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Auth
{

    public interface IAuthenticationProvider
    {
        IQueryable<T> ApplySecurity<T>(IQueryable<T> query, IQueryable<IStudent> queryType, IPrincipal currentUser) where T : IStudent;
        IQueryable<T> ApplySecurity<T>(IQueryable<T> query, IQueryable<ISchool> queryType, IPrincipal currentUser) where T : ISchool;
        bool IsResultSecured<T>(IEnumerable<T> result, IEnumerable<ISchool> resultType, IPrincipal currentUser) where T : ISchool;
        bool IsResultSecured<T>(IEnumerable<T> result, IEnumerable<IStudent> resultType, IPrincipal currentUser) where T : IStudent;

    }
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly ITeacherToStudentUsiQueries _teacherToStudentUsiQueries;
        private readonly IParentUsitoStudentUsiQueries _parentUsitoStudentUsiQueries;
        private readonly ILocalEducationAgencyIdToStaffUsiQueries _localEducationAgencyIdToStaffUsiQueries;
        private readonly IParentUsitoSchoolIdQueries _parentUsitoSchoolIdQueries;
        private readonly ISchoolIdToStaffUsiQueries _schoolIdToStaffUsiQueries;
        public AuthenticationProvider(ITeacherToStudentUsiQueries teacherToStudentUsiQueries, IParentUsitoStudentUsiQueries parentUsitoStudentUsiQueries, ILocalEducationAgencyIdToStaffUsiQueries localEducationAgencyIdToStaffUsiQueries, IParentUsitoSchoolIdQueries parentUsitoSchoolIdQueries, ISchoolIdToStaffUsiQueries schoolIdToStaffUsiQueries)
        {
            _teacherToStudentUsiQueries = teacherToStudentUsiQueries;
            _parentUsitoStudentUsiQueries = parentUsitoStudentUsiQueries;
            _localEducationAgencyIdToStaffUsiQueries = localEducationAgencyIdToStaffUsiQueries;
            _parentUsitoSchoolIdQueries = parentUsitoSchoolIdQueries;
            _schoolIdToStaffUsiQueries = schoolIdToStaffUsiQueries;
        }

        public IQueryable<T> ApplySecurity<T>(IQueryable<T> query, IQueryable<IStudent> queryType, IPrincipal currentUser) where T : IStudent
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            var usi = Int32.Parse(claims.First(x => x.Type == "person_usi").Value);
            var userRole = claims.First(x => x.Type.Contains("role")).Value;
            var accessLevel = claims.First(x => x.Type == "access_level").Value;

            if (accessLevel == "Section")
                return ApplyTeacherStudentSecurity(query, queryType, usi);
            if (accessLevel == "Student")
                return ApplyParentStudentSecurity(query, queryType, usi);

            if (accessLevel == "School")
                return ApplyStaffSchoolStudentSecurity(query, queryType, usi);
            if (accessLevel == "District")
                return ApplyStaffLocalEducationAgencyStudentSecurity(query, queryType, usi);

            throw new UnauthorizedAccessException("The identity trying to access the data is not configured on the server.");
        }

        public IQueryable<T> ApplySecurity<T>(IQueryable<T> query, IQueryable<ISchool> queryType, IPrincipal currentUser) where T : ISchool
        {

            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            var usi = Int32.Parse(claims.First(x => x.Type == "person_usi").Value);
            var userRole = claims.First(x => x.Type.Contains("role")).Value;
            var accessLevel = claims.First(x => x.Type == "access_level").Value;

            if (accessLevel == "Student")
                return ApplyParentSchoolSecurity(query, queryType, usi);
            if (accessLevel == "School")
                return ApplyStaffSchoolSecurity(query, queryType, usi);
            if (accessLevel == "District")
                return ApplyStaffLocalEducationAgencySecurity(query, queryType, usi);

            throw new UnauthorizedAccessException("The identity trying to access the data is not configured on the server.");
        }
        private IQueryable<T> ApplyTeacherStudentSecurity<T>(IQueryable<T> query, IQueryable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _teacherToStudentUsiQueries.GetTeacherStudents(usi)
                    on q.StudentUsi equals ps.StudentUsi
                    select q);
        }

        private IQueryable<T> ApplyParentStudentSecurity<T>(IQueryable<T> query, IQueryable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _parentUsitoStudentUsiQueries.GetParentStudents(usi)
                    on q.StudentUsi equals ps.StudentUsi
                    select q);
        }

        private IQueryable<T> ApplyStaffSchoolStudentSecurity<T>(IQueryable<T> query, IQueryable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _schoolIdToStaffUsiQueries.GetStaffSchools(usi)
                    on q.SchoolId equals ps.SchoolId
                    select q);
        }

        private IQueryable<T> ApplyStaffLocalEducationAgencyStudentSecurity<T>(IQueryable<T> query, IQueryable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _localEducationAgencyIdToStaffUsiQueries.GetStaffLocalEducationAgencies(usi)
                    on q.LocalEducationAgencyId equals ps.LocalEducationAgencyId
                    select q);
        }

        private IQueryable<T> ApplyStaffSchoolSecurity<T>(IQueryable<T> query, IQueryable<ISchool> queryType, int usi) where T : ISchool
        {
            return (from q in query
                    join ps in _schoolIdToStaffUsiQueries.GetStaffSchools(usi)
                    on q.SchoolId equals ps.SchoolId
                    select q);
        }

        private IQueryable<T> ApplyStaffLocalEducationAgencySecurity<T>(IQueryable<T> query, IQueryable<ISchool> queryType, int usi) where T : ISchool
        {
            return (from q in query
                    join ps in _localEducationAgencyIdToStaffUsiQueries.GetStaffLocalEducationAgencies(usi)
                    on q.LocalEducationAgencyId equals ps.LocalEducationAgencyId
                    select q);
        }

        private IQueryable<T> ApplyParentSchoolSecurity<T>(IQueryable<T> query, IQueryable<ISchool> queryType, int usi) where T : ISchool
        {
            return (from q in query
                    join ps in _parentUsitoSchoolIdQueries.GetParentSchools(usi)
                    on q.SchoolId equals ps.SchoolId
                    select q);
        }
        public bool IsResultSecured<T>(IEnumerable<T> result, IEnumerable<ISchool> resultType, IPrincipal currentUser) where T : ISchool
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            var usi = Int32.Parse(claims.First(x => x.Type == "person_usi").Value);
            var userRole = claims.First(x => x.Type.Contains("role")).Value;
            var accessLevel = claims.First(x => x.Type == "access_level").Value;

            if (accessLevel == "Student")
                return ApplyParentSchoolSecurity(result, resultType, usi).Count() == result.Count();
            if (accessLevel == "School")
                return ApplyStaffSchoolSecurity(result, resultType, usi).Count() == result.Count();
            if (accessLevel == "District")
                return ApplyStaffLocalEducationAgencySecurity(result, resultType, usi).Count() == result.Count();

            throw new UnauthorizedAccessException("The identity trying to access the data is not configured on the server.");

            throw new UnauthorizedAccessException("The identity trying to access the data is not configured on the server.");
        }

        public bool IsResultSecured<T>(IEnumerable<T> result, IEnumerable<IStudent> resultType, IPrincipal currentUser) where T : IStudent
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            var usi = Int32.Parse(claims.First(x => x.Type == "person_usi").Value);
            var userRole = claims.First(x => x.Type.Contains("role")).Value;
            var accessLevel = claims.First(x => x.Type == "access_level").Value;
            var resultCount = result.Count();

            if (accessLevel == "Section")
                return ApplyTeacherStudentSecurity(result, resultType, usi).Count() == result.Count();
            if (accessLevel == "Student")
                return ApplyParentStudentSecurity(result, resultType, usi).Count() == result.Count();

            if (accessLevel == "School")
                return ApplyStaffSchoolStudentSecurity(result, resultType, usi).Count() == result.Count();
            if (accessLevel == "District")
                return ApplyStaffLocalEducationAgencyStudentSecurity(result, resultType, usi).Count() == result.Count();

            throw new UnauthorizedAccessException("The identity trying to access the data is not configured on the server.");
        }

        private IEnumerable<T> ApplyTeacherStudentSecurity<T>(IEnumerable<T> query, IEnumerable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _teacherToStudentUsiQueries.GetTeacherStudents(usi)
                    on q.StudentUsi equals ps.StudentUsi
                    select q);
        }

        private IEnumerable<T> ApplyParentStudentSecurity<T>(IEnumerable<T> query, IEnumerable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _parentUsitoStudentUsiQueries.GetParentStudents(usi)
                    on q.StudentUsi equals ps.StudentUsi
                    select q);
        }

        private IEnumerable<T> ApplyStaffSchoolStudentSecurity<T>(IEnumerable<T> query, IEnumerable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _schoolIdToStaffUsiQueries.GetStaffSchools(usi)
                    on q.SchoolId equals ps.SchoolId
                    select q);
        }

        private IEnumerable<T> ApplyStaffLocalEducationAgencyStudentSecurity<T>(IEnumerable<T> query, IEnumerable<IStudent> queryType, int usi) where T : IStudent
        {
            return (from q in query
                    join ps in _localEducationAgencyIdToStaffUsiQueries.GetStaffLocalEducationAgencies(usi)
                    on q.LocalEducationAgencyId equals ps.LocalEducationAgencyId
                    select q);
        }

        private IEnumerable<T> ApplyStaffSchoolSecurity<T>(IEnumerable<T> query, IEnumerable<ISchool> queryType, int usi) where T : ISchool
        {
            return (from q in query
                    join ps in _schoolIdToStaffUsiQueries.GetStaffSchools(usi)
                    on q.SchoolId equals ps.SchoolId
                    select q);
        }

        private IEnumerable<T> ApplyStaffLocalEducationAgencySecurity<T>(IEnumerable<T> query, IEnumerable<ISchool> queryType, int usi) where T : ISchool
        {
            return (from q in query
                    join ps in _localEducationAgencyIdToStaffUsiQueries.GetStaffLocalEducationAgencies(usi)
                    on q.LocalEducationAgencyId equals ps.LocalEducationAgencyId
                    select q);
        }

        private IEnumerable<T> ApplyParentSchoolSecurity<T>(IEnumerable<T> query, IEnumerable<ISchool> queryType, int usi) where T : ISchool
        {
            return (from q in query
                    join ps in _parentUsitoSchoolIdQueries.GetParentSchools(usi)
                    on q.SchoolId equals ps.SchoolId
                    select q);
        }
    }

    public class EdOrgAssociation
    {
        public int EducationOrganizationId { get; set; }
        public string EdOrgType { get; set; }
        public string Role { get; set; }
        public string AccessLevel { get; set; }
    }

}

