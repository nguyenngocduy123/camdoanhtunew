﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using CamDoAnhTu.Models;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CamDoAnhTu.Controllers
{
    public class AccountController : Controller
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
                    string jsonItem = JsonConvert.SerializeObject(us);
                    //cookie
                    HttpCookie cookie = new HttpCookie("userInfo");
                    cookie.Expires = DateTime.Now.AddMinutes(1);
                    cookie.Values["username"] = jsonItem;
                    HttpContext.Response.Cookies.Add(cookie);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Usersname hoặc password không đúng";
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Session["IsLogin"] = 0;
            Session["CurUser"] = null;
            Session["User"] = null;

            if (Request.Cookies["userInfo"] != null)
            {
                HttpCookie reqCookies = Request.Cookies["userInfo"];
                reqCookies.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(reqCookies);
            }

            if (Request.Cookies["Chonngaylam"] != null)
            {
                HttpCookie reqCookies1 = Request.Cookies["Chonngaylam"];
                reqCookies1.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(reqCookies1);
            }            
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