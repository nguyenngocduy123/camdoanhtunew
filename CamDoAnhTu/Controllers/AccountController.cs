using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CamDoAnhTu.Helper;
using CamDoAnhTu.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;

namespace CamDoAnhTu.Controllers
{
    [SessionTimeout]
    public class AccountController : Controller
    {
        [HttpPost]
        public ActionResult Logout()
        {
            Session["IsLogin"] = 0;
            Session["CurUser"] = null;
            Session["User"] = null;
            FormsAuthentication.SignOut();
            //Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
            //HttpCookie reqCookies = Request.Cookies["userInfo"];
            //reqCookies.Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Login", "Account");
        }

        private const int WrongPass = 1;
        private const int SamePass = 2;
        private const int PwdChanged = 1;
        private const int InfoChanged = 2;

        [HttpPost]
        public ActionResult UpdatePWD(string uid, string pwd, string newPwd)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                User usr = ctx.Users.FirstOrDefault(u => u.UserName == uid);
                if (usr.PassWord != pwd)
                {
                    return Json(new { Error = WrongPass });
                }
                else
                {
                    usr.PassWord = newPwd;
                    ctx.Entry(usr).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();

                    return Json(new { Success = PwdChanged });
                }
            }
            //return RedirectToAction("Profile", "Account");
        }

        public ActionResult ChangePassWord(ChangePasswordModel model)
        {           
            return View(model);
        }

        public ActionResult LoadAccount()
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                List<User> lstUser = ctx.Users.ToList();
                return View(lstUser);
            }
        }
    }
}