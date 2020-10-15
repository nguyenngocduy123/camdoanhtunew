using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CamDoAnhTu.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;

namespace CamDoAnhTu.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            string Username = model.UserName;
            string password = model.PassWord;
            string host = Request.Url.Host;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                User us = ctx.Users.Where(u => u.UserName == Username && u.PassWord == password && u.Enabled == true).FirstOrDefault();

                if (us != null)
                {
                    string jsondata = JsonConvert.SerializeObject(us);
                    var ticket = new FormsAuthenticationTicket(1, us.UserName,
                        DateTime.Now, DateTime.Now.AddMinutes(3), model.Remember, jsondata);
                    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));

                    //Session["User"] = us;
                    //Session.Timeout = 2;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Usersname hoặc password không đúng";
                }
            }
            return View(model);
        }
        
    }
}