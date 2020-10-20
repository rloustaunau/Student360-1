using Microsoft.AspNetCore.Mvc.Controllers;
using SMCISD.Student360.Resources.Services.People;
using SMCISD.Student360.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using SMCISD.Student360.Persistence.Security;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using SMCISD.Student360.Resources.Services.StudentAbsencesByCourse;
using System.Linq.Dynamic.Core;

namespace SMCISD.Student360.Web.Filters
{
    public class SecurityActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.ActionDescriptor.FilterDescriptors.Count(x => x.Filter is NoAuthFilterAttribute) > 0)
                return; // Does not require further security logic

            if ((context.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(typeof(NoAuthFilterAttribute), inherit: true).Any())
                return; // Does not require further security logic

            if (context.Result == null)
                return;

            var res = context.Result as ObjectResult;

            object objectResult = new object();
            if (res == null)
                return;
            else
                objectResult = res.Value;

            //if (!CanCastResultObject(res.Value))
            //    throw new UnauthorizedAccessException("Result object could not be cast to known interfaces and we can't validate security.");

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = context.HttpContext.User;
                var auth = context.HttpContext.RequestServices.GetService<IAuthenticationProvider>();
                var gridResponse = objectResult as IGridResponse;

                if (gridResponse != null && gridResponse.Data != null && gridResponse.Data.Count() > 0)
                {

                    var gridList = gridResponse.Data.ToDynamicList();
                    if (gridList.FirstOrDefault().GetType().GetProperty("StudentUsi") != null)
                    {
                        var studentGrid = gridList.Select( x => new Student { StudentUsi = x.StudentUsi, SchoolId = x.SchoolId, LocalEducationAgencyId = x.LocalEducationAgencyId });
                        if (studentGrid != null && !auth.IsResultSecured(studentGrid, studentGrid, user))
                            throw new UnauthorizedAccessException("Data is not secured, please add security to the query.");
                    }
                    else 
                    {
                        var schoolGrid = gridList.Select(x => new School { SchoolId = x.SchoolId, LocalEducationAgencyId = x.LocalEducationAgencyId });
                        if (schoolGrid != null && !auth.IsResultSecured(schoolGrid, schoolGrid, user))
                            throw new UnauthorizedAccessException("Data is not secured, please add security to the query.");
                    }
                }

                var listResponse = objectResult as IEnumerable<object>;

                if (listResponse != null)
                {
                    if(listResponse.FirstOrDefault() as IStudent != null)
                    {
                        var studentList = listResponse.Cast<IStudent>();
                        if (studentList != null && !auth.IsResultSecured(studentList, studentList, user))
                            throw new UnauthorizedAccessException("Data is not secured, please add security to the query.");
                    }
                    if (listResponse.FirstOrDefault() as ISchool != null)
                    {
                        var schoolList = listResponse.Cast<ISchool>();
                        if (schoolList != null && !auth.IsResultSecured(schoolList, schoolList, user))
                            throw new UnauthorizedAccessException("Data is not secured, please add security to the query.");
                    }
                }

                var studentProfileResponse = objectResult as IStudentProfileModel;

                if(studentProfileResponse != null)
                {
                    // This will only have a Grid with IStudent.
                    var studentProfileData = studentProfileResponse.Grid.Data.Cast<IStudent>();
                    if (studentProfileData != null && !auth.IsResultSecured(studentProfileData, studentProfileData, user))
                        throw new UnauthorizedAccessException("Data is not secured, please add security to the query.");
                }


                // Add other interfaces as needed to validate the object result
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        private bool CanCastResultObject(Object o)
        {
            // Add other interfaces as needed in order to validate the object result 
            // This have to be in order, from specific to general
            // You can verify most of the models interfaces beeing used in SMCISD.Student360.Persistenace/Models/MyModels/ModelImplementations.cs
            // There are some special cases like the IStudentProfileModel which is on the service part.

            return o as IGridResponse != null
                || o as ICurrentUser != null
                || o as IStudentProfileModel != null
                || o as IEnumerable<IStudent> != null
                || o as IEnumerable<ISchool> != null
                || o as IEnumerable<ILocalEducationAgency> != null;
        }
    }
}
