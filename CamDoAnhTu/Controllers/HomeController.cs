using CamDoAnhTu.Helper;
using CamDoAnhTu.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CamDoAnhTu.Controllers
{
    [SessionTimeout]
    public class HomeController : Controller
    {
        private static int update = 0;

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult XE1(int type)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ViewBag.type = type;
                List<Customer> list = ctx.Customers.Where(p => p.type == type &&
                p.IsDeleted == false).ToList();

                return PartialView(list);
            }
        }
        public ActionResult Management(int type)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ViewBag.type = type;
                List<Customer> list = new List<Customer>();

                if (type == -1)
                {
                    list = ctx.Customers.Where(p => (p.type == 1 || p.type == 2 || p.type == 3 ||
                    p.type == 4 || p.type == 5 || p.type == 6 || p.type == 7)
                    && p.IsDeleted == false).ToList();
                    return PartialView(list);
                }
                else if (type == 1 || type == 2 || type == 3 ||
                    type == 4 || type == 5 || type == 6 || type == 7)
                {
                    list = ctx.Customers.Where(p => p.type == type
                    && p.IsDeleted == false).ToList();

                    switch (type)
                    {
                        case 1:
                            var list1 = ctx.Customers.Where(p => p.type == type).ToList();

                            break;
                        default:
                            break;
                    }
                }

                return PartialView(list);
            }
        }

        public string[] masotragopArr = { "BA", "CA", "MA", "ZA", "YA", "TA", "QA" };
        public string[] masotradungArr = { "BD", "CD", "MD", "ZD", "YD", "TD" };
        public ActionResult LoadCustomer(int? pageSize, int? type, int page = 1)
        {

            int pageSz = pageSize ?? 10;
            StringBuilder str = new StringBuilder();
            decimal k = 0;

            int todayYear = DateTime.Now.Year;
            int todayMonth = DateTime.Now.Month;
            int todayDay = DateTime.Now.Day;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {

                ctx.Configuration.ValidateOnSaveEnabled = false;
                var lishExample = ctx.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();

                //foreach (var cs in lishExample)
                //{
                //    if (masotragopArr.Any(cs.Code.Contains))
                //    {
                //        int code = Int32.Parse(cs.Code.Substring(2, cs.Code.Length - 2));

                //        if (cs.Code[0] == 'A')
                //            cs.CodeSort = code + 1000;
                //        else
                //            cs.CodeSort = (((cs.Code[0] - 'A') + 1) * 1000) + code;
                //    }
                //    else
                //        cs.CodeSort = Int32.Parse(cs.Code);
                //}

                var tiengoc = ctx.GetTienGoc(type).SingleOrDefault();
                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc.Value:N0}";

                var tienlai = ctx.GetTienLai(type).SingleOrDefault();
                DateTime startdate = new DateTime(2018, 10, 1);
                DateTime enddate = startdate.AddMonths(1).AddDays(-1);

                //DateTime startdate = new DateTime(DateTime.Now.Year, 1, 1);
                //DateTime enddate = new DateTime(DateTime.Now.Year, 12, 31);
                var tienlaithucte = ctx.GetTienLaiThatTe(type, startdate, enddate).SingleOrDefault();

                if (tienlai != null)
                    ViewBag.tienlai = $"{tienlai.Value:N0}";

                if (tienlaithucte != null)
                {
                    //save tien lai

                    var message = ctx.Messages.Where(p => p.Date == DateTime.Now)
                        .FirstOrDefault();
                    if (message == null && DateTime.Now.Day == 1)
                    {
                        Message newMsg = new Message();
                        newMsg.Message1 = $"Tiền lãi thực tế : {tienlaithucte.Value:N0} ";
                        newMsg.type = type;
                        newMsg.Date = DateTime.Now;
                        ctx.Messages.Add(newMsg);
                        ctx.SaveChanges();
                    }

                    ViewBag.tienlaithucte = string.Format("{0:N0}", tienlaithucte.Value.ToString("N0"));
                }
                List<Customer> list1 = new List<Customer>();

                if (type == -1)
                {
                    list1 = ctx.Customers.Where(p => p.IsDeleted == false
                    && p.type != 12).ToList();

                }
                else if (type == 1 || type == 2 || type == 3 || type == 4 ||
                    type == 5 || type == 6 || type == 7)
                {
                    list1 = ctx.Customers.Where(p => p.IsDeleted == false
                                && p.type == type).ToList();

                }

                int count1 = list1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                List<Customer> list = list1.OrderBy(p => p.Code)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.type = type.Value;
                ViewBag.CurPage = page;

                var summoney = (from l in ctx.Loans
                                join cs in ctx.Customers on l.IDCus equals cs.ID
                                where (cs.type == type || type == -1 || type == -2)
                                 && l.Date.Year == todayYear
                                && l.Date.Month == todayMonth
                                && l.Date.Day == todayDay
                                && cs.Description != "End" && cs.IsDeleted == false
                                select new
                                {
                                    cs.Price,
                                    cs.Code
                                }).ToList();

                foreach (var x in summoney)
                    k += x.Price ?? 0;

                foreach (Customer cs in list)
                {
                    cs.NgayNo = 0;

                    int countMax = 0;

                    DateTime EndDate = DateTime.Now;

                    List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    Loan t1 = new Loan();

                    if (t.Count != 0)
                    {
                        t1 = t.First();
                    }

                    DateTime StartDate = t1.Date;

                    List<Loan> query = t.Where(p => p.Date >= StartDate && p.Date <= EndDate).ToList();
                    int count = 0;
                    foreach (Loan temp in query)
                    {
                        if (temp.Status == 0)
                        {
                            count++;
                            countMax = count;
                        }
                        else
                        {
                            count = 0;
                        }
                    }

                    cs.NgayNo = countMax;
                    ctx.SaveChanges();
                }

                str.Append("Số tiền phải thu trong ngày " + DateTime.Now.Date.ToShortDateString() + " : " + k.ToString("N0"));
                ViewBag.Message1 = str.ToString();

                return View(list);
            }
        }

        public ActionResult BadCustomer(int? pageSize, int type, int page = 1)
        {

            int pageSz = pageSize ?? 10;
            StringBuilder str = new StringBuilder();
            decimal? k = 0;

            int todayYear = DateTime.Now.Year;
            int todayMonth = DateTime.Now.Month;
            int todayDay = DateTime.Now.Day;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                List<Customer> list1 = new List<Customer>();

                if (type == -1)
                {
                    list1 = ctx.Customers.Where(p => p.type != 12 && p.Loans.OrderByDescending(x => x.ID)
                    .FirstOrDefault().Date > DateTime.Now).ToList();
                }
                else
                {
                    list1 = ctx.Customers.Where(p => p.type == type && p.Loans.OrderByDescending(x => x.ID)
                    .FirstOrDefault().Date > DateTime.Now).ToList();
                }

                int count1 = list1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                List<Customer> list = list1.OrderBy(p => p.Code)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.type = type;
                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;

                var summoney = (from l in ctx.Loans
                                join cs in ctx.Customers on l.IDCus equals cs.ID
                                where cs.type == type && l.Date.Year == todayYear
                                && l.Date.Month == todayMonth
                                && l.Date.Day == todayDay && cs.IsDeleted == false
                                select new
                                {
                                    cs.Price
                                }).ToList();

                foreach (var x in summoney)
                {
                    k += x.Price;
                }

                foreach (Customer cs in list)
                {
                    cs.NgayNo = 0;

                    int countMax = 0;

                    DateTime EndDate = DateTime.Now;

                    List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    Loan t1 = new Loan();

                    if (t.Count != 0)
                    {
                        t1 = t.First();
                    }

                    DateTime StartDate = t1.Date;

                    List<Loan> query = t.Where(p => p.Date >= StartDate && p.Date <= EndDate).ToList();
                    int count = 0;
                    foreach (Loan temp in query)
                    {
                        if (temp.Status == 0)
                        {
                            count++;
                            countMax = count;
                        }
                        else
                        {
                            count = 0;
                        }
                    }

                    cs.NgayNo = countMax;
                    ctx.SaveChanges();
                }

                str.Append("Số tiền phải thu trong ngày " + DateTime.Now.Date.ToShortDateString() + " : " + k.ToString());

                if (TempData["message"] != null)
                {
                    ViewBag.Message = TempData["message"].ToString();
                }

                return View(list);
            }
        }

        public ActionResult LoadCustomerXE1(int? pageSize, int? type, int page = 1)
        {

            int pageSz = pageSize ?? 10;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;

                var lishExample = ctx.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();

                //foreach (var cs in lishExample)
                //{
                //    if (masotradungArr.Any(cs.Code.Contains))
                //    {
                //        int code = Int32.Parse(cs.Code.Substring(2, cs.Code.Length - 2));

                //        if (cs.Code[0] == 'A')
                //            cs.CodeSort = code + 1000;
                //        else
                //            cs.CodeSort = (((cs.Code[0] - 'A') + 1) * 1000) + code;

                //    }
                //    else
                //        cs.CodeSort = Int32.Parse(cs.Code);

                //}

                var tiengoc = ctx.GetTienGoc_Dung(type).SingleOrDefault();

                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc:N0}";

                //DateTime startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //DateTime enddate = startdate.AddMonths(1).AddDays(-1);

                DateTime startdate = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime enddate = new DateTime(DateTime.Now.Year, 12, 31);
                var tienlaithucte = ctx.GetTienLaiThatTe_Dung(type, startdate, enddate).SingleOrDefault();

                if (tienlaithucte != null)
                    ViewBag.tienlai = $"{tienlaithucte.Value:N0}";

                if (tienlaithucte != null)
                {
                    //save tien lai

                    var message = ctx.Messages.Where(p => p.Date == DateTime.Now)
                        .FirstOrDefault();
                    if (message == null && DateTime.Now.Day == 1)
                    {
                        Message newMsg = new Message();
                        newMsg.Message1 = $"Tiền lãi thực tế : {tienlaithucte.Value:N0} ";
                        newMsg.type = type;
                        newMsg.Date = DateTime.Now;
                        ctx.Messages.Add(newMsg);
                        ctx.SaveChanges();
                    }

                    ViewBag.tienlaithucte = string.Format("{0:N0}", tienlaithucte.Value.ToString("N0"));
                }


                List<Customer> list1 = ctx.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();
                int count1 = list1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                List<Customer> list = list1.OrderBy(p => p.Code)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;
                ViewBag.type = type.Value;

                foreach (Customer cs in list1)
                {
                    cs.nodung = false;
                    List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    foreach (var item in t)
                    {
                        DateTime tempDT = item.Date;

                        if (tempDT.Date <= DateTime.Now.Date && item.Status == 0)
                        {
                            cs.nodung = true;
                        }
                    }

                    ctx.SaveChanges();
                }

                return View(list);
            }
        }

        public ActionResult Search(string Code, string Name, string Phone,
            string Address, int? Noxau, int? hetno, int page = 1, int type = -1)
        {
            int pageSz = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            List<Loan> lstLoan = new List<Loan>();

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                DateTime startdate = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime enddate = new DateTime(DateTime.Now.Year, 12, 31);
                var tienlai = ctx.GetTienLai(type).SingleOrDefault();
                var tienlaithucte = ctx.GetTienLaiThatTe(type, startdate, enddate).SingleOrDefault();
                var tiengoc = ctx.GetTienGoc(type).SingleOrDefault();

                if (tienlai != null)
                    ViewBag.tienlai = $"{tienlai.Value:N0}";

                if (tienlaithucte != null)
                    ViewBag.tienlaithucte = $"{tienlaithucte.Value:N0}";

                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc.Value:N0}";

                ctx.Configuration.ValidateOnSaveEnabled = false;//masotragopArr.Any(cs.Code.Contains)
                var list = new List<Customer>();
                if (type != -1)
                {
                    list = ctx.Customers.Where(p => p.type == type).ToList();
                }                

                List<Customer> lsttrave = new List<Customer>();

                if (type != -1)
                    lsttrave = list.Where(p => p.type == type).ToList();
                else
                    lsttrave = list;

                if (!string.IsNullOrEmpty(Code))
                    lsttrave = lsttrave.Where(p => p.Code == Code || p.Code.StartsWith(Code.ToUpper())).ToList();

                if (!string.IsNullOrEmpty(Name))
                    lsttrave = lsttrave.Where(p => p.Name.Contains(Name)).ToList();

                if (!string.IsNullOrEmpty(Phone))
                    lsttrave = lsttrave.Where(p => p.Phone.Contains(Phone)).ToList();

                if (!string.IsNullOrEmpty(Address))
                    lsttrave = lsttrave.Where(p => p.Address.Contains(Address)).ToList();


                lsttrave = lsttrave.Where(p => p.IsDeleted == false).ToList();
                var lstNoxau = new List<Customer>();
                if (Noxau == 1)
                {
                    foreach (Customer p in lsttrave)
                    {
                        if (p.NgayNo >= 5)
                        {
                            lstNoxau.Add(p);
                        }
                    }
                    lsttrave = lstNoxau;
                }

                //if (hetno == 1)
                //{
                //    List<Customer> lstCus = ctx.Customers.Where(p => p.Price != null && p.Loan != null).ToList();
                //    foreach (Customer p in lstCus)
                //    {
                //        int day = 0;
                //        if (Int32.Parse(p.Price.ToString()) == 0)
                //        {
                //            day = 0;
                //        }
                //        else
                //            day = Int32.Parse(p.Loan.ToString()) / Int32.Parse(p.Price.ToString());

                //        if (p.DayPaids == day || p.Description == "End")
                //        {
                //            lsttrave.Add(p);
                //        }
                //    }
                //}
                //lsttrave = (from p in lsttrave
                //            where masotragopArr.Any(val => p.Code.Contains(val))
                //            select p).ToList();
                int count = lsttrave.Count();
                int nPages = count / pageSz + (count % pageSz > 0 ? 1 : 0);
                List<Customer> lsttrave1 = lsttrave.OrderBy(p => p.Code)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;
                ViewBag.Code = Code;
                ViewBag.Name = Name;
                ViewBag.Phone = Phone;
                ViewBag.Noxau = Noxau;
                ViewBag.hetno = hetno;
                ViewBag.Address = Address;
                ViewBag.type = type;

                foreach (Customer cs in lsttrave1)
                {
                    cs.NgayNo = 0;
                    int count1 = 0;
                    int countMax = 0;

                    DateTime EndDate = DateTime.Now;

                    List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    Loan t1 = new Loan();

                    if (t.Count != 0)
                    {
                        t1 = t.First();
                    }

                    DateTime StartDate = t1.Date;

                    List<Loan> query = t.Where(p => p.Date >= StartDate && p.Date <= EndDate).ToList();

                    foreach (Loan temp in query)
                    {
                        if (temp.Status == 0)
                        {
                            count1++;
                            countMax = count1;
                        }
                        else
                        {
                            count1 = 0;
                        }
                    }

                    cs.NgayNo = countMax;
                    ctx.SaveChanges();
                }

                return View(lsttrave1);
            }
        }

        public ActionResult SearchXE1(string Code, string Name, string Phone, string Address,
            string tentaisan, int? Noxau, int? hetno, int page = 1, int type = -1)
        {
            int pageSz = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            List<Loan> lstLoan = new List<Loan>();

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                //var list = ctx.Customers.Where(p => masotradungArr.AsQueryable().Any(p.Code.Contains)).ToList();
                var list = ctx.Customers.ToList();

                List<Customer> lsttrave = new List<Customer>();

                if (type != -1)
                    lsttrave = list.Where(p => p.type == type).ToList();
                else
                    lsttrave = list;

                if (!string.IsNullOrEmpty(Code))
                    lsttrave = lsttrave.Where(p => p.Code == Code).ToList();

                if (!string.IsNullOrEmpty(Name))
                    lsttrave = lsttrave.Where(p => p.Name.Contains(Name)).ToList();

                if (!string.IsNullOrEmpty(Phone))
                    lsttrave = lsttrave.Where(p => p.Phone.Contains(Phone)).ToList();

                if (!string.IsNullOrEmpty(tentaisan))
                    lsttrave = lsttrave.Where(p => p.tentaisan.Contains(tentaisan)).ToList();

                lsttrave = (from p in lsttrave
                            where masotradungArr.Any(val => p.Code.Contains(val))
                            select p).ToList();
                int count = lsttrave.Count();
                int nPages = count / pageSz + (count % pageSz > 0 ? 1 : 0);
                List<Customer> lsttrave1 = lsttrave.OrderBy(p => p.Code)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;
                ViewBag.Code = Code;
                ViewBag.Name = Name;
                ViewBag.Phone = Phone;
                ViewBag.Noxau = Noxau;
                ViewBag.hetno = hetno;
                ViewBag.Address = Address;
                ViewBag.tentaisan = tentaisan;
                ViewBag.type = type;

                foreach (Customer cs in lsttrave1)
                {
                    cs.NgayNo = 0;
                    int count1 = 0;
                    int countMax = 0;

                    DateTime EndDate = DateTime.Now;

                    List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    Loan t1 = new Loan();

                    if (t.Count != 0)
                        t1 = t.First();


                    DateTime StartDate = t1.Date;

                    List<Loan> query = t.Where(p => p.Date >= StartDate && p.Date <= EndDate).ToList();

                    foreach (Loan temp in query)
                    {
                        if (temp.Status == 0)
                        {
                            count1++;
                            countMax = count1;
                        }
                        else
                            count1 = 0;

                    }

                    cs.NgayNo = countMax;
                    ctx.SaveChanges();
                }

                return View(lsttrave1);
            }
        }

        public ActionResult Refresh(int type)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                var query1 = ctx.Customers.ToList();

                foreach (Customer cs in query1)
                {
                    cs.NgayNo = 0;

                    int countMax = 0;

                    DateTime EndDate = DateTime.Now;

                    List<Loan> t = ctx.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    Loan t1 = new Loan();

                    if (t.Count != 0)
                        t1 = t.First();

                    DateTime StartDate = t1.Date;

                    List<Loan> query = t.Where(p => p.Date >= StartDate && p.Date <= EndDate).ToList();
                    int count = 0;
                    foreach (Loan temp in query)
                    {
                        if (temp.Status == 0)
                        {
                            count++;
                            countMax = count;
                        }
                        else
                            count = 0;

                    }

                    cs.NgayNo = countMax;
                    ctx.SaveChanges();
                }
            }
            return RedirectToAction("LoadCustomer", "Home", new { type = type });
        }
        public ActionResult AddCustomer(int type)
        {

            ViewBag.ListLoaiGiayTo = new SelectList(
                new List<SelectListItem>
                {
                        new SelectListItem { Text = "Giấy tờ chính chủ", Value = "1"},
                        new SelectListItem { Text = "Giấy tờ photo", Value = "2"},
                        new SelectListItem { Text = "Không có giấy tờ", Value = "3"}
                }, "Value", "Text");

            MyViewModel mvViewModel = new MyViewModel();
            mvViewModel.model.type = type;

            return View(mvViewModel);
        }

        [HttpPost]
        public ActionResult AddCustomer(MyViewModel myViewModel)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ViewBag.ListLoaiGiayTo = new SelectList(
                new List<SelectListItem>
                {
                        new SelectListItem { Text = "Giấy tờ chính chủ", Value = "1"},
                        new SelectListItem { Text = "Giấy tờ photo", Value = "2"},
                        new SelectListItem { Text = "Không có giấy tờ", Value = "3"}
                }, "Value", "Text");
                ctx.Configuration.ValidateOnSaveEnabled = false;

                float day = float.Parse(myViewModel.model.Loan.ToString()) / float.Parse(myViewModel.model.Price.ToString());

                if (day != (int)day)
                {
                    ViewBag.Message = "Số ngày không hợp lệ";
                    return View(myViewModel);
                }

                //if (day > 60)
                //{
                //    ViewBag.Message = "Số ngày nợ lớn hơn 60";
                //    return View(myViewModel);
                //}

                myViewModel.model.DayPaids = 0;
                myViewModel.model.AmountPaid = 0;
                myViewModel.model.RemainingAmount = myViewModel.model.Loan.Value;
                myViewModel.model.loaigiayto = myViewModel.SelectedLoaiGiayTo;
                myViewModel.model.NgayNo = 0;
                myViewModel.model.DayBonus = myViewModel.model.DayBonus ?? 0;
                myViewModel.model.IsDeleted = false;
                myViewModel.model.Code = myViewModel.model.Code.Trim();

                ctx.Customers.Add(myViewModel.model);

                for (int i = 0; i < day; i++)
                {
                    Loan temp = new Loan();
                    temp.Date = myViewModel.model.StartDate.AddDays(i);
                    temp.IDCus = myViewModel.model.ID;
                    temp.Status = 0;
                    ctx.Loans.Add(temp);
                }
                ctx.SaveChanges();
            }

            if (myViewModel.fuMain != null && myViewModel.fuMain.ContentLength > 0)
            {
                string spDirPath = Server.MapPath("~/image");
                string targetDirPath = Path.Combine(spDirPath, myViewModel.model.Code.ToString());
                Directory.CreateDirectory(targetDirPath);

                string mainFileName = Path.Combine(targetDirPath, "main.jpg");
                //myViewModel.fuMain.SaveAs(mainFileName);

                Image bm = Image.FromStream(myViewModel.fuMain.InputStream);
                bm = Helper.Helper.ResizeBitmap((Bitmap)bm, 160, 160); /// new width, height
                bm.Save(Path.Combine(targetDirPath, "main.jpg"));
            }
            return RedirectToAction("LoadCustomer", "Home", new { type = myViewModel.model.type });
        }

        public ActionResult Update(int id)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                MyViewModel viewModel = new MyViewModel();
                ViewBag.ListLoaiGiayTo = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Giấy tờ chính chủ", Value = "1"},
                        new SelectListItem { Text = "Giấy tờ photo", Value = "2"},
                        new SelectListItem { Text = "Không có giấy tờ", Value = "3"}
                    }, "Value", "Text");

                update = 0;
                Customer pro = ctx.Customers.Where(p => p.ID == id).FirstOrDefault();

                if (pro.Loan == null || pro.Price == null)
                    update = 1;

                if (pro == null)
                    return RedirectToAction("LoadCustomer", "Home");

                viewModel.model = pro;
                viewModel.SelectedLoaiGiayTo = pro.loaigiayto.Value;
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Update(MyViewModel myViewModel)
        {
            try
            {
                int i = 1;

                if (myViewModel.model.DayBonus == null)
                    myViewModel.model.DayBonus = 0;

                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    Customer pro = ctx.Customers.Where(p => p.Code == myViewModel.model.Code).FirstOrDefault();
                    pro.Name = myViewModel.model.Name;
                    pro.Phone = myViewModel.model.Phone;
                    pro.Address = myViewModel.model.Address;
                    pro.Loan = myViewModel.model.Loan;
                    pro.Price = myViewModel.model.Price;
                    pro.DayPaids = myViewModel.model.DayPaids;
                    pro.AmountPaid = myViewModel.model.AmountPaid;
                    pro.RemainingAmount = myViewModel.model.RemainingAmount;
                    pro.DayBonus = myViewModel.model.DayBonus;
                    pro.OldCode = myViewModel.model.OldCode;
                    pro.Note = myViewModel.model.Note;
                    pro.loaigiayto = myViewModel.SelectedLoaiGiayTo;
                    pro.tiengoc = myViewModel.model.tiengoc;
                    //pro.StartDate = myViewModel.model.StartDate;

                    //if (pro.StartDate != myViewModel.model.StartDate)
                    //{
                    //    pro.StartDate = myViewModel.model.StartDate;
                    //    int t = Int32.Parse(myViewModel.model.Loan.ToString()) / Int32.Parse(myViewModel.model.Price.ToString());
                    //    List<Loan> l = ctx.Loans.Where(p => p.IDCus == myViewModel.model.ID).ToList();

                    //    foreach (Loan temp in l)
                    //    {
                    //        temp.Date = myViewModel.model.StartDate.AddDays(i);
                    //        temp.IDCus = myViewModel.model.ID;
                    //        temp.Status = 0;
                    //        i++;
                    //        ctx.SaveChanges();
                    //    }
                    //}

                    ctx.SaveChanges();

                    if (update == 1)
                    {
                        pro = ctx.Customers.Where(p => p.Code == pro.Code).FirstOrDefault();

                        int day = Int32.Parse(pro.Loan.ToString()) / Int32.Parse(pro.Price.ToString());

                        for (int s = 1; s <= day; s++)
                        {
                            Loan temp = new Loan();
                            temp.Date = pro.StartDate.AddDays(s);
                            temp.IDCus = pro.ID;
                            temp.Status = 0;
                            ctx.Loans.Add(temp);
                            ctx.SaveChanges();
                        }
                        ctx.SaveChanges();
                    }
                }

                if (myViewModel.fuMain != null && myViewModel.fuMain.ContentLength > 0)
                {

                    string spDirPath = Server.MapPath("~/image");
                    string targetDirPath = Path.Combine(spDirPath, myViewModel.model.Code.ToString());
                    //Directory.CreateDirectory(targetDirPath);

                    //string mainFileName = Path.Combine(targetDirPath, "main.jpg");
                    //myViewModel.fuMain.SaveAs(mainFileName);

                    Image bm = Image.FromStream(myViewModel.fuMain.InputStream);
                    bm = Helper.Helper.ResizeBitmap((Bitmap)bm, 160, 160); /// new width, height
                    bm.Save(Helper.Helper.GetAbsoluteFilePath("main.jpg", myViewModel));
                    //bm.Save(Path.Combine(targetDirPath, "main.jpg"));

                }

                return RedirectToAction("LoadCustomer", "Home", new { type = myViewModel.model.type });
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult AddCustomerXE1(int type)
        {
            ViewBag.ListLoaiGiayTo = new SelectList(
                new List<SelectListItem>
                {
                        new SelectListItem { Text = "Giấy tờ chính chủ", Value = "1"},
                        new SelectListItem { Text = "Giấy tờ photo", Value = "2"},
                        new SelectListItem { Text = "Không có giấy tờ", Value = "3"}
                }, "Value", "Text");
            MyViewModel mvViewModel = new MyViewModel();
            mvViewModel.model.type = type;

            return View(mvViewModel);
        }

        [HttpPost]
        public ActionResult AddCustomerXE1(MyViewModel myViewModel)
        {
            var model = myViewModel.model;
            model.IsDeleted = false;
            model.loaigiayto = myViewModel.SelectedLoaiGiayTo;
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                List<Loan> lstLoan = new List<Loan>();

                ctx.Customers.Add(model);
                ctx.SaveChanges();

                DateTime k = model.StartDate;

                for (int i = 0; i < 90; i++)
                {
                    Loan temp = new Loan();
                    temp.Date = k.AddDays(i);
                    temp.IDCus = model.ID;
                    temp.Status = 0;
                    ctx.Loans.Add(temp);

                }
                ViewData["Loans"] = lstLoan;
                ctx.SaveChanges();
            }

            if (myViewModel.fuMain != null && myViewModel.fuMain.ContentLength > 0)
            {
                string spDirPath = Server.MapPath("~/image");
                string targetDirPath = Path.Combine(spDirPath, myViewModel.model.Code.ToString());
                Directory.CreateDirectory(targetDirPath);

                string mainFileName = Path.Combine(targetDirPath, "main.jpg");
                //myViewModel.fuMain.SaveAs(mainFileName);

                Image bm = Image.FromStream(myViewModel.fuMain.InputStream);
                bm = Helper.Helper.ResizeBitmap((Bitmap)bm, 160, 160); /// new width, height
                bm.Save(Path.Combine(targetDirPath, "main.jpg"));
            }
            return RedirectToAction("LoadCustomerXE1", "Home", new { type = myViewModel.model.type });
        }

        public ActionResult UpdateXE1(string id)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                MyViewModel viewModel = new MyViewModel();
                ViewBag.ListLoaiGiayTo = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Giấy tờ chính chủ", Value = "1"},
                        new SelectListItem { Text = "Giấy tờ photo", Value = "2"},
                        new SelectListItem { Text = "Không có giấy tờ", Value = "3"}
                    }, "Value", "Text");

                update = 0;
                Customer pro = ctx.Customers.Where(p => p.Code == id).FirstOrDefault();

                if (pro.Loan == null || pro.Price == null)
                    update = 1;

                if (pro == null)
                    return RedirectToAction("LoadCustomer", "Home");

                viewModel.model = pro;
                viewModel.SelectedLoaiGiayTo = pro.loaigiayto.Value;
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult UpdateXE1(MyViewModel myViewModel)
        {
            var model = myViewModel.model;

            if (model.DayBonus == null)
                model.DayBonus = 0;


            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                Customer pro = ctx.Customers.Where(p => p.Code == model.Code).FirstOrDefault();
                pro.Name = model.Name;
                pro.Phone = model.Phone;
                pro.Address = model.Address;
                pro.Note = model.Note;
                pro.tentaisan = model.tentaisan;
                pro.loaigiayto = myViewModel.SelectedLoaiGiayTo;
                pro.Price = myViewModel.model.Price;

                if (pro.StartDate != model.StartDate)
                {
                    List<Loan> lstLoan = new List<Loan>();
                    pro.StartDate = model.StartDate;

                    List<Loan> lstTong = ctx.Loans.Where(p => p.IDCus == model.ID).ToList();
                    List<Loan> lstLoandadong = ctx.Loans.Where(p => p.IDCus == model.ID && p.Status == 1).ToList();
                    int sldadong = lstLoandadong.Count;

                    foreach (var item in lstTong)
                        ctx.Loans.Remove(item);


                    //int day = model.songayno == 0 ? 0 : (int)model.songayno;
                    //DateTime k = model.StartDate;

                    //for (int i = 0; i < 90; i++)
                    //{
                    //    Loan temp = new Loan();
                    //    temp.Date = k.AddDays(i);
                    //    temp.IDCus = model.ID;
                    //    temp.Status = 0;
                    //    ctx.Loans.Add(temp);

                    //    k = temp.Date;
                    //    lstLoan.Add(temp);
                    //}

                    //for (int i = 0; i < sldadong; i++)
                    //{
                    //    var temp = lstLoan.ElementAt(i);
                    //    temp.Status = 1;
                    //}

                    ctx.SaveChanges();

                    ViewData["Loans"] = lstLoan;

                }

                ctx.SaveChanges();
            }

            if (myViewModel.fuMain != null && myViewModel.fuMain.ContentLength > 0)
            {
                string spDirPath = Server.MapPath("~/image");
                string targetDirPath = Path.Combine(spDirPath, myViewModel.model.Code.ToString());
                Directory.CreateDirectory(targetDirPath);

                string mainFileName = Path.Combine(targetDirPath, "main.jpg");
                //myViewModel.fuMain.SaveAs(mainFileName);

                Image bm = Image.FromStream(myViewModel.fuMain.InputStream);
                bm = Helper.Helper.ResizeBitmap((Bitmap)bm, 160, 160); /// new width, height
                bm.Save(Path.Combine(targetDirPath, "main.jpg"));
            }

            return RedirectToAction("LoadCustomerXE1", "Home", new { type = myViewModel.model.type });
        }

        public ActionResult Detail(int id)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                Customer model = ctx.Customers.FirstOrDefault(p => p.ID == id);
                List<Loan> list = ctx.Loans.Where(p => p.IDCus == id).ToList();
                ViewData["Loan"] = list;

                return View(model);
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult GetCusDetail(string code)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                var codeList = ctx.Customers.Where(i => i.Code == code).ToList();

                var viewmodel = codeList.Select(x => new
                {
                    Code = x.Code,
                    Name = x.Name,
                    Phone = x.Phone,
                    StartDate = x.StartDate,
                    Address = x.Address,
                    Loan = x.Loan,
                    Price = x.Price,
                    tiengoc = x.tiengoc,
                    loaigiayto = x.loaigiayto,
                    Ghichu = x.Note,
                    OldCode = x.OldCode,
                    songayduocphepnothem = x.DayBonus
                });

                return Json(viewmodel);
            }
        }
        public ActionResult Addday(Loan model)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                ctx.Loans.Add(model);
                ctx.SaveChanges();
            }

            return RedirectToAction("LoadCustomer", "Home");
        }

        public static string timetemp;

        [HttpGet]
        public ActionResult UpdateLoan(int loanid, string songaydong, int idcus)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                decimal? ct = 0;
                int t = -1;
                int songay;
                decimal? amount = 0;
                decimal remainingamount = 0;
                int? songaydatra;

                if (String.IsNullOrEmpty(songaydong))
                    songay = 0;
                else
                    songay = Int32.Parse(songaydong);

                if (songay == 0)
                {
                    Loan item = new Loan();

                    Customer csCustomer = new Customer();

                    item = ctx.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();

                    if (item.Type == true)
                        return Json(new { success = false, message = "Ngày đóng không hợp lệ" },
                            JsonRequestBehavior.AllowGet);

                    timetemp = item.Date.ToShortDateString();

                    item.Status = item.Status + 1;

                    if (item.Status >= 2)
                        item.Status = 0;

                    if (item.Status == 1)
                    {
                        csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        List<Loan> lstSongaydatra = ctx.Loans.Where(p => p.IDCus == idcus && p.Status == 1 && p.Type == false).ToList();
                        songaydatra = lstSongaydatra.Count;

                        songaydatra++;
                        csCustomer.AmountPaid = songaydatra * csCustomer.Price;
                        csCustomer.RemainingAmount = csCustomer.Loan.Value - csCustomer.AmountPaid.Value;
                        t = 1;
                        csCustomer.DayPaids = songaydatra;

                        WriteHistory(csCustomer, 0, loanid);
                    }
                    else
                    {
                        csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        List<Loan> lstSongaydatra1 = ctx.Loans.Where(p => p.IDCus == csCustomer.ID && p.Status == 1 && p.Type == false).ToList();
                        songaydatra = lstSongaydatra1.Count;
                        songaydatra--;
                        csCustomer.AmountPaid = songaydatra * csCustomer.Price;
                        csCustomer.RemainingAmount = csCustomer.Loan.Value - csCustomer.AmountPaid.Value;
                        t = 0;
                        csCustomer.DayPaids = songaydatra;

                        WriteHistory(csCustomer, 0, loanid);
                    }

                    ct = csCustomer.Price;
                    amount = csCustomer.AmountPaid ?? 0;
                    remainingamount = csCustomer.RemainingAmount.Value;

                    Helper.Helper.UpdateLoanCustomer(csCustomer);
                }
                else
                {
                    for (int i = 0; i < songay; i++)
                    {
                        Loan item = new Loan();

                        Customer csCustomer = new Customer();
                        item = ctx.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();
                        timetemp = item.Date.ToShortDateString();

                        if (item.Type == true)
                            return Json(new { success = false, message = "Ngày đóng không hợp lệ" },
                                JsonRequestBehavior.AllowGet);

                        loanid++;
                        item.Status = item.Status + 1;

                        if (item.Status >= 2)
                            item.Status = 0;

                        if (item.Status == 1)
                        {
                            csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();

                            List<Loan> lstSongaydatra2 = ctx.Loans.Where(p => p.IDCus == csCustomer.ID && p.Status == 1 && p.Type == false).ToList();
                            songaydatra = lstSongaydatra2.Count;
                            songaydatra++;
                            csCustomer.AmountPaid = songaydatra * csCustomer.Price;
                            csCustomer.RemainingAmount = csCustomer.Loan.Value - csCustomer.AmountPaid.Value;
                            t = 1;
                            csCustomer.DayPaids = songaydatra;

                            WriteHistory(csCustomer, 0, loanid);
                            ctx.SaveChanges();
                        }
                        else
                        {
                            csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                            List<Loan> lstSongaydatra3 = ctx.Loans.Where(p => p.IDCus == csCustomer.ID && p.Status == 1 && p.Type == false).ToList();
                            songaydatra = lstSongaydatra3.Count;
                            songaydatra--;
                            csCustomer.AmountPaid = songaydatra * csCustomer.Price;
                            csCustomer.RemainingAmount = csCustomer.Loan.Value - csCustomer.AmountPaid.Value;
                            t = 0;
                            csCustomer.DayPaids = songaydatra;

                            WriteHistory(csCustomer, 0, loanid);
                            ctx.SaveChanges();
                        }
                        ct = csCustomer.Price * songay;
                        amount = csCustomer.AmountPaid ?? 0;
                        remainingamount = csCustomer.RemainingAmount.Value;

                        Helper.Helper.UpdateLoanCustomer(csCustomer);
                    }
                }

                ctx.SaveChanges();

                return Json(new { success = true, oldval = loanid, status = t, songay = songay, amount = amount, remainingamount = remainingamount, ct = ct },
               JsonRequestBehavior.AllowGet);

            }
            //UpdateAllSongaydatra();
            //return View();
        }

        [HttpGet]
        public ActionResult UpdateNodung(int loanid, string songaydong, int idcus)
        {
            int t = -1;
            int songay;

            if (String.IsNullOrEmpty(songaydong))
                songay = 0;
            else
                songay = Int32.Parse(songaydong);

            if (songay == 0)
            {
                Loan item = new Loan();

                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    Customer csCustomer = new Customer();

                    item = ctx.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();
                    timetemp = item.Date.ToShortDateString();

                    item.Status = item.Status + 1;

                    if (item.Status >= 2)
                        item.Status = 0;

                    if (item.Status == 1)
                    {
                        csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        t = 1;
                        WriteHistory(csCustomer, 0, loanid);
                    }
                    else
                    {
                        csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        t = 0;
                        WriteHistory(csCustomer, 0, loanid);
                    }

                    ctx.SaveChanges();
                }
            }
            else
            {
                for (int i = 0; i < songay; i++)
                {
                    Loan item = new Loan();

                    using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                    {
                        ctx.Configuration.ValidateOnSaveEnabled = false;
                        Customer csCustomer = new Customer();
                        item = ctx.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();
                        timetemp = item.Date.ToShortDateString();
                        item.Status = item.Status + 1;

                        if (item.Status >= 2)
                            item.Status = 0;


                        if (item.Status == 1)
                        {
                            csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                            t = 1;
                            WriteHistory(csCustomer, 0, loanid);
                        }
                        else
                        {
                            csCustomer = ctx.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                            t = 0;
                            WriteHistory(csCustomer, 0, loanid);
                        }
                        loanid++;
                        ctx.SaveChanges();
                    }
                }
            }

            return Json(new { success = true, oldval = loanid, status = t, songay = songay },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reset(int id)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                Customer cs = ctx.Customers.FirstOrDefault(p => p.ID == id);

                cs.Description = "End";
                timetemp = DateTime.Now.Date.ToShortDateString();
                WriteHistory(cs, -1, -1);
                ctx.SaveChanges();
            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchName(string term)
        {
            var products = Helper.Helper.GetCustomer(term).Select(c => new { id = c.Code, value = c.Name });
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchCode(string term)
        {
            var products = Helper.Helper.GetCustomer1(term).Select(c => new { id = c.Code, value = c.Code });

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchPhone(string term)
        {
            var products = Helper.Helper.GetCustomer2(term).Select(c => new { id = c.Code, value = c.Phone });
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchAddress(string term)
        {
            var products = Helper.Helper.GetCustomer3(term).Select(c => new { id = c.Code, value = c.Address });
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchTentaisan(string term)
        {
            var products = Helper.Helper.Gettentaisan(term).Select(c => new { id = c.Code, value = c.tentaisan });
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveItem(int proId)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                Customer ep = ctx.Customers.Where(p => p.ID == proId).FirstOrDefault();
                List<Loan> lstLoans = ctx.Loans.Where(p => p.IDCus == proId).ToList();

                ctx.Customers.Remove(ep);

                foreach (var item in lstLoans)
                {
                    ctx.Loans.Remove(item);
                }
                ctx.SaveChanges();
            }
            ViewBag.Delete = true;
            return RedirectToAction("LoadCustomer", "Home");
        }

        [HttpPost]
        public JsonResult DeleteCustomer(int id)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            try
            {
                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    Customer cus = ctx.Customers.Where(o => o.ID == id).FirstOrDefault();
                    List<Loan> lstLoans = ctx.Loans.Where(p => p.IDCus == id).ToList();
                    if (cus != null)
                    {

                        Customer temp = new Customer();
                        temp.Name = cus.Name;
                        temp.Phone = cus.Phone;
                        temp.Address = cus.Address;
                        temp.Description = cus.Description;
                        temp.type = cus.type;
                        temp.StartDate = cus.StartDate;
                        temp.IsDeleted = true;
                        temp.Code = Helper.Helper.RandomString(4);
                        temp.Loan = cus.Loan;
                        temp.tiengoc = cus.tiengoc;
                        foreach (var item in lstLoans)
                        {
                            ctx.Loans.Remove(item);
                        }

                        ctx.Customers.Remove(cus);
                        ctx.SaveChanges();
                        result["status"] = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                result["status"] = "error";
                result["message"] = ex.Message;
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult AddLoan(CustomLoan l)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                //string t = l.Money.Replace(',', ' ');
                //t = t.ToCharArray()
                // .Where(c => !Char.IsWhiteSpace(c))
                // .Select(c => c.ToString())
                // .Aggregate((a, b) => a + b);

                //int i = t.IndexOf('.');
                //t = t.Remove(i);

                int money = Int32.Parse(l.Money);

                Customer cs = ctx.Customers.Where(p => p.ID == l.IDCus).FirstOrDefault();

                Loan model = new Loan();
                model.Date = l.Date;
                model.Status = 1;
                model.IDCus = l.IDCus;
                model.Type = true;
                model.money = money;

                ctx.Loans.Add(model);

                timetemp = DateTime.Now.ToString();
                ctx.SaveChanges();
                WriteHistory(cs, money, model.ID);

                return Json(new { amountpaid = cs.AmountPaid, remainingamount = cs.RemainingAmount },
                    JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ResetDatetime(int type, string message, DateTime datetime)
        {

            if (!string.IsNullOrEmpty(message))
            {
                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    Message msg = new Message();
                    msg.Message1 = message;
                    msg.Date = datetime;
                    msg.type = 1;
                    ctx.Messages.Add(msg);
                    ctx.SaveChanges();
                }
            }

            return Json(type);
        }
        public void WriteHistory(Customer p, int money, int loanid)
        {
            StringBuilder str = new StringBuilder();
            int type = 0;

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {

                if (loanid != -1)
                {
                    var checkhs = ctx.histories.Where(s => s.loanid == loanid).FirstOrDefault();
                    if (checkhs == null)
                    {
                        var checkLoan = ctx.Loans.Where(s => s.ID == loanid).FirstOrDefault();

                        if (checkLoan.Status == 1)
                        {
                            type = 0;
                            history hs = new history();
                            str.Append("Xóa đóng tiền cho ngày: " + timetemp);
                            hs.CustomerId = p.ID;
                            hs.CustomerCode = p.Code;
                            hs.Detail = str.ToString();
                            hs.Ngaydongtien = DateTime.Now;
                            hs.price = money == 0 ? p.Price : money;
                            hs.status = type;
                            hs.loanid = loanid;
                            ctx.histories.Add(hs);
                        }
                        else
                        {
                            type = 1;
                            history hs = new history();
                            str.Append("Đóng tiền cho ngày: " + timetemp);
                            hs.CustomerId = p.ID;
                            hs.CustomerCode = p.Code;
                            hs.Detail = str.ToString();
                            hs.Ngaydongtien = DateTime.Now;
                            hs.price = money == 0 ? p.Price : money;
                            hs.status = type;
                            hs.loanid = loanid;
                            ctx.histories.Add(hs);
                        }


                    }
                    else
                    {

                        int oldtype = checkhs.status.Value;

                        if (oldtype == 1) // xóa dong tien
                        {
                            str.Append("Xóa đóng tiền cho ngày: " + timetemp);
                            checkhs.Ngaydongtien = DateTime.Now;
                            type = 0;
                            checkhs.status = type;
                            checkhs.Detail = str.ToString();
                        }
                        else // đóng tien
                        {
                            str.Append("Đóng tiền cho ngày: " + timetemp);
                            type = 1;
                            checkhs.Ngaydongtien = DateTime.Now;
                            checkhs.status = type;
                            checkhs.Detail = str.ToString();
                        }

                    }
                }
                else if (loanid == -1)
                {
                    history hs = new history();
                    str.Append("Kết thúc dây nợ ngày : " + timetemp);
                    hs.CustomerId = p.ID;
                    hs.Detail = str.ToString();
                    hs.CustomerCode = p.Code;
                    hs.Ngaydongtien = DateTime.Now;
                    hs.price = money == 0 ? p.Price : money;
                    hs.status = 0;
                    hs.loanid = -1;
                    ctx.histories.Add(hs);
                }

                ctx.SaveChanges();
            }
        }

        public static DateTime? chonngaylam = null;

        public ActionResult ChonNgay(DateTime? chonngaylamSubmit)
        {
            if (chonngaylamSubmit.HasValue)
            {
                chonngaylam = chonngaylamSubmit.Value;
            }
            //chonngaylam = DateTime.Parse(chonngaylamVal);
            return RedirectToAction("Index");
        }

        public ActionResult ExcelExport(int? type)
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                var customers = ctx.Customers.Where(p => p.type == type.Value).ToList();

                DataTable Dt = new DataTable();
                Dt.Columns.Add("Code", typeof(string));
                Dt.Columns.Add("Name", typeof(string));
                Dt.Columns.Add("Phone", typeof(string));
                Dt.Columns.Add("Address", typeof(string));

                foreach (Customer p in customers)
                {
                    if (p.NgayNo >= 5)
                    {
                        DataRow row = Dt.NewRow();
                        row[0] = p.Code;
                        row[1] = p.Name;
                        row[2] = p.Phone;
                        row[3] = p.Address;
                        Dt.Rows.Add(row);
                    }
                }

                var memoryStream = new MemoryStream();
                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(Dt, true);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 18;

                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.DefaultColWidth = 20;
                    worksheet.Column(2).AutoFit();

                    Session["DownloadExcel_FileManager"] = excelPackage.GetAsByteArray();
                    return Json("", JsonRequestBehavior.AllowGet);
                }

            }
            return View();
        }

        public ActionResult Download()
        {

            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", "khachhangnoxau.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }
    }
}