using System.Web;
using System.Web.Mvc;
using System;
using System.Web.Security;
using CamDoAnhTu.Models;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Security.Claims;

namespace CamDoAnhTu.Helper
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
            {
                filterContext.Result = new RedirectResult("~/User/Login");
                return;
            }
            var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var userData = JsonConvert.DeserializeObject<User>(authTicket.UserData);            
            filterContext.HttpContext.User = new GenericPrincipal(new FormsIdentity(authTicket), null);
        }
    }
}