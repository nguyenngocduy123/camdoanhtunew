using CamDoAnhTu.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CamDoAnhTu.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TimKiemNoKhachHang()
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                int result;
                List<Customer> lst = ctx.Customers.ToList();
                List<Customer> lst1 = new List<Customer>();
                foreach (var cus in lst)
                {
                    string t = cus.Code[cus.Code.Length - 1].ToString();
                    if (int.TryParse(t, out result))
                    {
                        int id = Int32.Parse(t);
                        if (id % 2 == 0 && Helper.Helper.CheckHetNo(cus) == false)
                        {
                            lst1.Add(cus);
                        }
                    }
                    else
                    {
                        // Not a number, do something else with it.
                    }
                }
                //lst1 = ctx.GetCustomerEven1().ToList();
                return View(lst1);
            }
        }

        public ActionResult TimKiemNoKhachHang1()
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                int result;
                List<Customer> lst = ctx.Customers.ToList();
                List<Customer> lst1 = new List<Customer>();
                foreach (var cus in lst)
                {
                    string t = cus.Code[cus.Code.Length - 1].ToString();
                    if (int.TryParse(t, out result))
                    {
                        int id = Int32.Parse(t);
                        if (id % 2 == 1 && Helper.Helper.CheckHetNo(cus) == false)
                        {
                            lst1.Add(cus);
                        }
                    }
                    else
                    {
                        // Not a number, do something else with it.
                    }
                }
                //lst1 = ctx.GetCustomerOdd1().ToList();

                foreach (Customer cs in lst1)
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

                return View(lst1);
            }
        }

        public ActionResult TimKiemNoKhachHangEvenXE1()
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                int result;
                List<Customer> lst = ctx.Customers.Where(p => p.type == 12).ToList();
                List<Customer> lst1 = new List<Customer>();
                foreach (var cus in lst)
                {
                    string t = cus.Code[cus.Code.Length - 1].ToString();
                    if (int.TryParse(t, out result))
                    {
                        int id = Int32.Parse(t);
                        if (id % 2 == 0 && Helper.Helper.CheckHetNo(cus) == false)
                        {
                            lst1.Add(cus);
                        }
                    }
                    else
                    {
                        // Not a number, do something else with it.
                    }
                }
                return View(lst1);
            }
        }

        public ActionResult TimKiemNoKhachHangOddXE1()
        {
            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                int result;
                List<Customer> lst = ctx.Customers.Where(p => p.type == 12).ToList();
                List<Customer> lst1 = new List<Customer>();
                foreach (var cus in lst)
                {
                    string t = cus.Code[cus.Code.Length - 1].ToString();
                    if (int.TryParse(t, out result))
                    {
                        int id = Int32.Parse(t);
                        if (id % 2 == 1 && Helper.Helper.CheckHetNo(cus) == false)
                        {
                            lst1.Add(cus);
                        }
                    }
                    else
                    {
                        // Not a number, do something else with it.
                    }
                }
                return View(lst1);
            }
        }
        public ActionResult LoadCustomerEven(int page = 1, int type = -1)
        {
            int pageSz = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;

                List<Customer> lst = ctx.Customers
                    .Where(p => (type == -1 || p.type == type)).ToList();
                List<Customer> lst1 = new List<Customer>();

                int result;

                foreach (var cus in lst)
                {
                    string t = cus.Code[cus.Code.Length - 1].ToString();
                    if (int.TryParse(t, out result))
                    {
                        int id = Int32.Parse(t);
                        if (id % 2 == 0)
                        {
                            lst1.Add(cus);
                        }
                    }
                    else
                    {
                        // Not a number, do something else with it.
                    }
                }

                int count1 = lst1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                lst1 = lst1.OrderBy(p => p.CodeSort)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;

                foreach (Customer cs in lst1)
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

                return View(lst1);
            }
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
    }
}