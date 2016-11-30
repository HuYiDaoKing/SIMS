using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIMS.Web.EntityDataModel;
using SIMS.Web.Models;
using SIMS.Web.Helper;

namespace SIMS.Web.Areas.Admins.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminUserAuthentication : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(SkipAdminUserAuthentication), true)
              || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipAdminUserAuthentication), true);
            if (skipAuthorization)
            {
                base.OnAuthorization(filterContext);
                return;
            }

            bool isStayInLoginPage = filterContext.RouteData.Values.ContainsValue("Login");
            if (!isStayInLoginPage)
            {
                var currentAdminUser = filterContext.HttpContext.Session[AccountHashKeys.CurrentAdminUser] as AdminUser;
                bool isLoginAction = filterContext.RouteData.Values.ContainsValue("Login") || filterContext.RouteData.Values.ContainsValue("login");
                bool isUnlogin = (currentAdminUser == null) && (!isLoginAction);

                if (isUnlogin)
                {
                    filterContext.HttpContext.Response.Redirect("~/Account/Login");
                }
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
        public class SkipAdminUserAuthentication : AuthorizeAttribute
        {
        }
    }
}