

using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SMCISD.Student360.Web.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class NoAuthFilterAttribute : ActionFilterAttribute
    {
       
    }
}
