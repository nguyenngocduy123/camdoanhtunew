﻿using System.Web;
using System.Web.Mvc;
using System;
using CamDoAnhTu.Models;

namespace CamDoAnhTu.Helper
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //HttpCookie cookie = HttpContext.Current.Request.Cookies["userInfo"];

            //if (cookie == null)
            //{
            //    filterContext.Result = new RedirectResult("~/Account/Login");
            //    return;
            //}
            //else
            //{
            //    HttpCookie newcookie = HttpContext.Current.Request.Cookies["userInfo"];
            //    newcookie.Expires = DateTime.Now.AddMinutes(1);
            //    HttpContext.Current.Request.Cookies.Add(newcookie);
            //}
            if (HttpContext.Current.Session["User"] == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}