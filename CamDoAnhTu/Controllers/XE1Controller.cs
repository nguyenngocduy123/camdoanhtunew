using CamDoAnhTu.Helper;
using CamDoAnhTu.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CamDoAnhTu.Controllers
{
    [SessionTimeout]
    public class XE1Controller : Controller
    {
        public int update = 0;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadCustomerXE1(int? pageSize, int? type, int page = 1)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                int pageSz = pageSize ?? 10;
                var lishExample = dbcontext.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();

                foreach (var cs in lishExample)
                {
                    if (Const.masotradungArr.Any(cs.Code.Contains))
                    {
                        int code = Int32.Parse(cs.Code.Substring(2, cs.Code.Length - 2));

                        if (cs.Code[0] == 'A')
                            cs.CodeSort = code + 1000;
                        else
                            cs.CodeSort = (((cs.Code[0] - 'A') + 1) * 1000) + code;

                    }
                    else
                        cs.CodeSort = Int32.Parse(cs.Code);

                }

                var tiengoc = dbcontext.GetTienGoc_Dung(type).SingleOrDefault();

                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc:N0}";

                //DateTime startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //DateTime enddate = startdate.AddMonths(1).AddDays(-1);

                DateTime startdate = new DateTime(2018, 10, 1);
                DateTime enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var tienlaithucte = dbcontext.GetTienLaiThatTe_Dung(type, startdate, enddate).SingleOrDefault();

                if (tienlaithucte != null)
                    ViewBag.tienlai = $"{tienlaithucte.Value:N0}";

                if (tienlaithucte != null)
                {
                    //save tien lai

                    var message = dbcontext.Messages.Where(p => p.Date == DateTime.Now)
                        .FirstOrDefault();
                    if (message == null && DateTime.Now.Day == 1)
                    {
                        Message newMsg = new Message();
                        newMsg.Message1 = $"Tiền lãi thực tế : {tienlaithucte.Value:N0} ";
                        newMsg.type = type;
                        newMsg.Date = DateTime.Now;
                        dbcontext.Messages.Add(newMsg);
                        dbcontext.SaveChanges();
                    }

                    ViewBag.tienlaithucte = string.Format("{0:N0}", tienlaithucte.Value.ToString("N0"));
                }

                List<Customer> list1 = dbcontext.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();
                int count1 = list1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                List<Customer> list = list1.OrderBy(p => p.CodeSort).Skip((page - 1) * pageSz).Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;
                ViewBag.type = type.Value;

                foreach (Customer cs in list1)
                {
                    cs.nodung = false;
                    List<Loan> t = dbcontext.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

                    foreach (var item in t)
                    {
                        DateTime tempDT = item.Date;

                        if (tempDT.Date <= DateTime.Now.Date && item.Status == 0)
                        {
                            cs.nodung = true;
                        }
                    }

                    dbcontext.SaveChanges();
                }

                return View(list);
            }
           

        }

        public ActionResult XE1(int type)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                ViewBag.type = type;
                List<Customer> list = dbcontext.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();

                return PartialView(list);
            }
            
        }

        public ActionResult AddCustomerXE1(int type)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
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

                int newId = 0;
                int id = 0;

                var lstType = dbcontext.Customers.Where(p => p.type == type).ToList();
                if (lstType.Count <= 0)
                    newId = 1;
                else
                {
                    id = lstType.Count;
                    newId = id + 1;
                }
                string temp = "";
                switch (type)
                {
                    case 12:
                        temp = "BD" + newId;
                        break;
                    case 13:
                        temp = "CD" + newId;
                        break;
                    case 14:
                        temp = "MD" + newId;
                        break;
                    case 15:
                        temp = "ZD" + newId;
                        break;
                    case 16:
                        temp = "YD" + newId;
                        break;
                    case 17:
                        temp = "TD" + newId;
                        break;
                    default:
                        break;
                }

                mvViewModel.model.Code = temp.Trim();
                return View(mvViewModel);
            }
            
        }

        [HttpPost]
        public ActionResult AddCustomerXE1(MyViewModel myViewModel)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                var model = myViewModel.model;
                model.IsDeleted = false;
                model.loaigiayto = myViewModel.SelectedLoaiGiayTo;

                List<Loan> lstLoan = new List<Loan>();
                dbcontext.Customers.Add(model);
                dbcontext.SaveChanges();

                DateTime k = model.StartDate;

                for (int i = 0; i < 90; i++)
                {
                    Loan temp = new Loan();
                    temp.Date = k.AddDays(i);
                    temp.IDCus = model.ID;
                    temp.Status = 0;
                    dbcontext.Loans.Add(temp);

                }
                ViewData["Loans"] = lstLoan;
                dbcontext.SaveChanges();


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
                return RedirectToAction("LoadCustomerXE1", "Xe1", new { type = myViewModel.model.type });
            }
            
        }

        public ActionResult UpdateXE1(string id)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
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
                Customer pro = dbcontext.Customers.Where(p => p.Code == id).FirstOrDefault();

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
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                var model = myViewModel.model;

                if (model.DayBonus == null)
                    model.DayBonus = 0;

                Customer pro = dbcontext.Customers.Where(p => p.Code == model.Code).FirstOrDefault();
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

                    List<Loan> lstTong = dbcontext.Loans.Where(p => p.IDCus == model.ID).ToList();
                    List<Loan> lstLoandadong = dbcontext.Loans.Where(p => p.IDCus == model.ID && p.Status == 1).ToList();
                    int sldadong = lstLoandadong.Count;

                    foreach (var item in lstTong)
                        dbcontext.Loans.Remove(item);

                    //int day = model.songayno == 0 ? 0 : (int)model.songayno;
                    //DateTime k = model.StartDate;

                    //for (int i = 0; i < 90; i++)
                    //{
                    //    Loan temp = new Loan();
                    //    temp.Date = k.AddDays(i);
                    //    temp.IDCus = model.ID;
                    //    temp.Status = 0;
                    //    dbcontext.Loans.Add(temp);

                    //    k = temp.Date;
                    //    lstLoan.Add(temp);
                    //}

                    //for (int i = 0; i < sldadong; i++)
                    //{
                    //    var temp = lstLoan.ElementAt(i);
                    //    temp.Status = 1;
                    //}

                    dbcontext.SaveChanges();
                    ViewData["Loans"] = lstLoan;
                }

                dbcontext.SaveChanges();


                if (myViewModel.fuMain != null && myViewModel.fuMain.ContentLength > 0)
                {
                    string spDirPath = Server.MapPath("~/image");
                    string targetDirPath = Path.Combine(spDirPath, myViewModel.model.Code.ToString());
                    Directory.CreateDirectory(targetDirPath);

                    string mainFileName = Path.Combine(targetDirPath, "main.jpg");
                    //myViewModel.fuMain.SaveAs(mainFileName);

                    Image bm = Image.FromStream(myViewModel.fuMain.InputStream);
                    bm = Helper.Helper.ResizeBitmap((Bitmap)bm, 160, 160);
                    bm.Save(Path.Combine(targetDirPath, "main.jpg"));
                }

                return RedirectToAction("LoadCustomerXE1", "XE1", new { type = myViewModel.model.type });
            }
            
        }

        public ActionResult SearchXE1(string Code, string Name, string Phone, string Address,
            string tentaisan, int? Noxau, int? hetno, int page = 1, int type = -1)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                int pageSz = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
                List<Loan> lstLoan = new List<Loan>();

                var list = dbcontext.Customers.ToList();

                DateTime startdate = new DateTime(2018, 10, 1);
                DateTime enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                var tiengoc = dbcontext.GetTienGoc_Dung(type).SingleOrDefault();
                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc:N0}";

                var tienlaithucte = dbcontext.GetTienLaiThatTe_Dung(type, startdate, enddate).SingleOrDefault();

                if (tienlaithucte != null)
                    ViewBag.tienlaithucte = $"{tienlaithucte.Value:N0}";

                //DateTime startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                //DateTime enddate = startdate.AddMonths(1).AddDays(-1);

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
                            where Const.masotradungArr.Any(val => p.Code.Contains(val))
                            select p).ToList();
                int count = lsttrave.Count();
                int nPages = count / pageSz + (count % pageSz > 0 ? 1 : 0);
                List<Customer> lsttrave1 = lsttrave.OrderBy(p => p.CodeSort)
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

                    List<Loan> t = dbcontext.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

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
                    dbcontext.SaveChanges();
                }

                return View(lsttrave1);
            }
            

        }
    }
}