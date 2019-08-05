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
using System.Web;
using System.Web.Mvc;

namespace CamDoAnhTu.Controllers
{
    [SessionTimeout]
    public class HomeController : Controller
    {
        private static int update = 0;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Management(int type)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                ViewBag.type = type;
                List<Customer> list = new List<Customer>();

                if (type == -1)
                {
                    list = dbcontext.Customers.Where(p => Const.tragopArr.Contains(p.type.Value) && p.IsDeleted == false).ToList();

                    return PartialView(list);
                }
                else if (Const.tragopArr.Contains(type))
                {
                    list = dbcontext.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();

                    switch (type)
                    {
                        case 1:
                            var list1 = dbcontext.Customers.Where(p => p.type == type).ToList();

                            break;
                        default:
                            break;
                    }
                }
                return PartialView(list);
            }
               

        }
        public ActionResult LoadCustomer(int? pageSize, int? type, int page = 1)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                int pageSz = pageSize ?? 10;
                StringBuilder str = new StringBuilder();
                decimal k = 0;

                var lishExample = dbcontext.Customers.Where(p => p.type == type && p.IsDeleted == false).ToList();

                foreach (var cs in lishExample)
                {
                    if (Const.masotragopArr.Any(cs.Code.Contains))
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

                var tiengoc = dbcontext.GetTienGoc(type).SingleOrDefault();
                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc.Value:N0}";

                var tienlai = dbcontext.GetTienLai(type).SingleOrDefault();

                if (tienlai != null)
                    ViewBag.tienlai = $"{tienlai.Value:N0}";

                DateTime startdate = new DateTime(2018, 10, 1);
                DateTime enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                //DateTime startdate = new DateTime(DateTime.Now.Year, 1, 1);
                //DateTime enddate = new DateTime(DateTime.Now.Year, 12, 31);

                var tienlaithucte = dbcontext.GetTienLaiThatTe(type, startdate, enddate).SingleOrDefault();

                if (tienlaithucte != null)
                {
                    var message = dbcontext.Messages.Where(p => p.Date == DateTime.Now).FirstOrDefault();
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
                List<Customer> list1 = new List<Customer>();

                if (type == -1)
                {
                    list1 = dbcontext.Customers.Where(p => p.IsDeleted == false && p.type != 12 && (p.Description == "End" || p.NgayNo < 59)).ToList();
                }
                else if (Const.tragopArr.Contains(type.Value))
                {
                    list1 = dbcontext.Customers.Where(p => p.IsDeleted == false && p.type == type
                    && (p.Description == "End" || p.DayPaids == (p.Loan / p.Price) || p.NgayNo < 59)).ToList();
                }

                int count1 = list1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                List<Customer> list = list1.OrderBy(p => p.CodeSort).Skip((page - 1) * pageSz).Take(pageSz).ToList();

                ViewBag.PageCount = nPages;
                ViewBag.type = type.Value;
                ViewBag.CurPage = page;

                var summoney = (from l in dbcontext.Loans
                                join cs in dbcontext.Customers on l.IDCus equals cs.ID
                                where (cs.type == type || type == -1 || type == -2)
                                 && l.Date.Year == Const.todayYear && l.Date.Month == Const.todayMonth && l.Date.Day == Const.todayDay
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
                    List<Loan> t = dbcontext.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

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
                    dbcontext.SaveChanges();
                }

                str.Append("Số tiền phải thu trong ngày " + DateTime.Now.Date.ToShortDateString() + " : " + k.ToString("N0"));
                ViewBag.Message1 = str.ToString();

                return View(list);
            }
                
        }

        public ActionResult BadCustomer(int? pageSize, int type, int page = 1)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                int pageSz = pageSize ?? 10;
                StringBuilder str = new StringBuilder();
                decimal? k = 0;

                List<Customer> list1 = new List<Customer>();

                if (type == -1)
                {
                    list1 = dbcontext.Customers.Where(p => p.type != 12 && p.Loans.OrderByDescending(x => x.ID)
                    .FirstOrDefault().Date > DateTime.Now).ToList();
                }
                else
                {
                    list1 = dbcontext.Customers.Where(p => p.type == type && p.Loans.OrderByDescending(x => x.ID)
                    .FirstOrDefault().Date > DateTime.Now).ToList();
                }

                int count1 = list1.Count();
                int nPages = count1 / pageSz + (count1 % pageSz > 0 ? 1 : 0);

                List<Customer> list = list1.OrderBy(p => p.CodeSort)
                    .Skip((page - 1) * pageSz)
                     .Take(pageSz).ToList();

                ViewBag.type = type;
                ViewBag.PageCount = nPages;
                ViewBag.CurPage = page;

                var summoney = (from l in dbcontext.Loans
                                join cs in dbcontext.Customers on l.IDCus equals cs.ID
                                where cs.type == type && l.Date.Year == Const.todayYear
                                && l.Date.Month == Const.todayMonth
                                && l.Date.Day == Const.todayDay && cs.IsDeleted == false
                                select new
                                {
                                    cs.Price
                                }).ToList();

                foreach (var x in summoney)
                    k += x.Price;


                foreach (Customer cs in list)
                {
                    cs.NgayNo = 0;

                    int countMax = 0;

                    DateTime EndDate = DateTime.Now;

                    List<Loan> t = dbcontext.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

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
                    dbcontext.SaveChanges();
                }

                str.Append("Số tiền phải thu trong ngày " + DateTime.Now.Date.ToShortDateString() + " : " + k.ToString());

                if (TempData["message"] != null)
                {
                    ViewBag.Message = TempData["message"].ToString();
                }

                return View(list);
            }
                

        }

        public ActionResult Search(string Code, string Name, string Phone,
            string Address, int? Noxau, int? hetno, int page = 1, int type = -1)
        {
            int pageSz = Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            List<Loan> lstLoan = new List<Loan>();

            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                DateTime startdate = new DateTime(2018, 10, 1);
                DateTime enddate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                var tienlai = dbcontext.GetTienLai(type).SingleOrDefault();
                var tienlaithucte = dbcontext.GetTienLaiThatTe(type, startdate, enddate).SingleOrDefault();
                var tiengoc = dbcontext.GetTienGoc(type).SingleOrDefault();

                if (tienlai != null)
                    ViewBag.tienlai = $"{tienlai.Value:N0}";

                if (tienlaithucte != null)
                    ViewBag.tienlaithucte = $"{tienlaithucte.Value:N0}";

                if (tiengoc != null)
                    ViewBag.tiengoc = $"{tiengoc.Value:N0}";

                var list = dbcontext.Customers.ToList();

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

                lsttrave = (from p in lsttrave
                            where Const.masotragopArr.Any(val => p.Code.Contains(val))
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
                ViewBag.type = type;

                return View(lsttrave1);
            }
        }

        public ActionResult Refresh(int type)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {

                var query1 = dbcontext.Customers.ToList();

                foreach (Customer cs in query1)
                {
                    cs.NgayNo = 0;

                    int countMax = 0;
                    DateTime EndDate = DateTime.Now;
                    List<Loan> t = dbcontext.Loans.Where(p => p.IDCus == cs.ID).OrderBy(p => p.Date).ToList();

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
                    dbcontext.SaveChanges();
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

            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
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
                    case 1:
                        temp = "BA" + newId;
                        break;
                    case 2:
                        temp = "CA" + newId;
                        break;
                    case 3:
                        temp = "MA" + newId;
                        break;
                    case 4:
                        temp = "ZA" + newId;
                        break;
                    case 5:
                        temp = "YA" + newId;
                        break;
                    case 6:
                        temp = "TA" + newId;
                        break;
                    case 7:
                        temp = "QA" + newId;
                        break;
                    default:
                        break;
                }

                mvViewModel.model.Code = temp.Trim();
            }
            return View(mvViewModel);
        }

        [HttpPost]
        public ActionResult AddCustomer(MyViewModel myViewModel)
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


                float day = float.Parse(myViewModel.model.Loan.ToString()) / float.Parse(myViewModel.model.Price.ToString());

                if (day != (int)day)
                {
                    ViewBag.Message = "Số ngày không hợp lệ";
                    return View(myViewModel);
                }

                if (day > 60)
                {
                    ViewBag.Message = "Số ngày nợ lớn hơn 60";
                    return View(myViewModel);
                }

                myViewModel.model.DayPaids = 0;
                myViewModel.model.AmountPaid = 0;
                myViewModel.model.RemainingAmount = myViewModel.model.Loan.Value;
                myViewModel.model.loaigiayto = myViewModel.SelectedLoaiGiayTo;
                myViewModel.model.NgayNo = 0;
                myViewModel.model.DayBonus = myViewModel.model.DayBonus ?? 0;
                myViewModel.model.IsDeleted = false;
                myViewModel.model.Code = myViewModel.model.Code.Trim();

                dbcontext.Customers.Add(myViewModel.model);

                for (int i = 0; i < day; i++)
                {
                    Loan temp = new Loan();
                    temp.Date = myViewModel.model.StartDate.AddDays(i);
                    temp.IDCus = myViewModel.model.ID;
                    temp.Status = 0;
                    dbcontext.Loans.Add(temp);
                }
                dbcontext.SaveChanges();
            }

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
            return RedirectToAction("LoadCustomer", "Home", new { type = myViewModel.model.type });
        }

        public ActionResult Update(string id)
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
        public ActionResult Update(MyViewModel myViewModel)
        {
            try
            {

                if (myViewModel.model.DayBonus == null)
                    myViewModel.model.DayBonus = 0;

                using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
                {

                    Customer pro = dbcontext.Customers.Where(p => p.Code == myViewModel.model.Code).FirstOrDefault();
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
                    //    List<Loan> l = dbcontext.Loans.Where(p => p.IDCus == myViewModel.model.ID).ToList();

                    //    foreach (Loan temp in l)
                    //    {
                    //        temp.Date = myViewModel.model.StartDate.AddDays(i);
                    //        temp.IDCus = myViewModel.model.ID;
                    //        temp.Status = 0;
                    //        i++;
                    //        dbcontext.SaveChanges();
                    //    }
                    //}

                    dbcontext.SaveChanges();

                    if (update == 1)
                    {
                        pro = dbcontext.Customers.Where(p => p.Code == pro.Code).FirstOrDefault();

                        int day = Int32.Parse(pro.Loan.ToString()) / Int32.Parse(pro.Price.ToString());

                        for (int s = 1; s <= day; s++)
                        {
                            Loan temp = new Loan();
                            temp.Date = pro.StartDate.AddDays(s);
                            temp.IDCus = pro.ID;
                            temp.Status = 0;
                            dbcontext.Loans.Add(temp);
                            dbcontext.SaveChanges();
                        }
                        dbcontext.SaveChanges();
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

        public ActionResult Detail(int id)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                Customer model = dbcontext.Customers.FirstOrDefault(p => p.ID == id);
                List<Loan> list = dbcontext.Loans.Where(p => p.IDCus == id).ToList();
                ViewData["Loan"] = list;

                return View(model);
            }
            

        }

        [HttpPost]
        public JsonResult GetCusDetail(string code)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                var codeList = dbcontext.Customers.Where(i => i.Code == code).ToList();

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
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                dbcontext.Loans.Add(model);
                dbcontext.SaveChanges();

                return RedirectToAction("LoadCustomer", "Home");
            }
            
        }

        [HttpGet]
        public ActionResult UpdateLoan(int loanid, string songaydong, int idcus)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
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

                    item = dbcontext.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();

                    if (item.Type == true)
                        return Json(new { success = false, message = "Ngày đóng không hợp lệ" },
                            JsonRequestBehavior.AllowGet);

                    Const.timetemp = item.Date.ToShortDateString();

                    item.Status = item.Status + 1;

                    if (item.Status >= 2)
                        item.Status = 0;

                    if (item.Status == 1)
                    {
                        csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        List<Loan> lstSongaydatra = dbcontext.Loans.Where(p => p.IDCus == idcus && p.Status == 1 && p.Type == false).ToList();
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
                        csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        List<Loan> lstSongaydatra1 = dbcontext.Loans.Where(p => p.IDCus == csCustomer.ID && p.Status == 1 && p.Type == false).ToList();
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
                        item = dbcontext.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();
                        Const.timetemp = item.Date.ToShortDateString();

                        if (item.Type == true)
                            return Json(new { success = false, message = "Ngày đóng không hợp lệ" },
                                JsonRequestBehavior.AllowGet);

                        
                        item.Status = item.Status + 1;

                        if (item.Status >= 2)
                            item.Status = 0;

                        if (item.Status == 1)
                        {
                            csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();

                            List<Loan> lstSongaydatra2 = dbcontext.Loans.Where(p => p.IDCus == csCustomer.ID && p.Status == 1 && p.Type == false).ToList();
                            songaydatra = lstSongaydatra2.Count;
                            songaydatra++;
                            csCustomer.AmountPaid = songaydatra * csCustomer.Price;
                            csCustomer.RemainingAmount = csCustomer.Loan.Value - csCustomer.AmountPaid.Value;
                            t = 1;
                            csCustomer.DayPaids = songaydatra;

                            WriteHistory(csCustomer, 0, loanid);
                            dbcontext.SaveChanges();
                        }
                        else
                        {
                            csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                            List<Loan> lstSongaydatra3 = dbcontext.Loans.Where(p => p.IDCus == csCustomer.ID && p.Status == 1 && p.Type == false).ToList();
                            songaydatra = lstSongaydatra3.Count;
                            songaydatra--;
                            csCustomer.AmountPaid = songaydatra * csCustomer.Price;
                            csCustomer.RemainingAmount = csCustomer.Loan.Value - csCustomer.AmountPaid.Value;
                            t = 0;
                            csCustomer.DayPaids = songaydatra;

                            WriteHistory(csCustomer, 0, loanid);
                            dbcontext.SaveChanges();
                        }
                        loanid++;
                        ct = csCustomer.Price * songay;
                        amount = csCustomer.AmountPaid ?? 0;
                        remainingamount = csCustomer.RemainingAmount.Value;

                        Helper.Helper.UpdateLoanCustomer(csCustomer);
                    }
                }

                dbcontext.SaveChanges();

                return Json(new { success = true, oldval = loanid, status = t, songay = songay, amount = amount, remainingamount = remainingamount, ct = ct },
               JsonRequestBehavior.AllowGet);
            }
            

        }

        [HttpGet]
        public ActionResult UpdateNodung(int loanid, string songaydong, int idcus)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
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

                    Customer csCustomer = new Customer();

                    item = dbcontext.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();
                    Const.timetemp = item.Date.ToShortDateString();

                    item.Status = item.Status + 1;

                    if (item.Status >= 2)
                        item.Status = 0;

                    if (item.Status == 1)
                    {
                        csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        t = 1;
                        WriteHistory(csCustomer, 0, loanid);
                    }
                    else
                    {
                        csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                        t = 0;
                        WriteHistory(csCustomer, 0, loanid);
                    }

                    dbcontext.SaveChanges();

                }
                else
                {
                    for (int i = 0; i < songay; i++)
                    {
                        Loan item = new Loan();

                        Customer csCustomer = new Customer();
                        item = dbcontext.Loans.Where(p => p.ID == loanid && p.IDCus == idcus).FirstOrDefault();
                        Const.timetemp = item.Date.ToShortDateString();
                        item.Status = item.Status + 1;

                        if (item.Status >= 2)
                            item.Status = 0;

                        if (item.Status == 1)
                        {
                            csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                            t = 1;
                            WriteHistory(csCustomer, 0, loanid);
                        }
                        else
                        {
                            csCustomer = dbcontext.Customers.Where(p => p.ID == item.IDCus).FirstOrDefault();
                            t = 0;
                            WriteHistory(csCustomer, 0, loanid);
                        }
                        loanid++;
                        dbcontext.SaveChanges();

                    }
                }

                return Json(new { success = true, oldval = loanid, status = t, songay = songay },
                    JsonRequestBehavior.AllowGet);
            }
           
        }

        public ActionResult Reset(int id)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                Customer cs = dbcontext.Customers.FirstOrDefault(p => p.ID == id);

                cs.Description = "End";
                Const.timetemp = DateTime.Now.Date.ToShortDateString();
                WriteHistory(cs, -1, -1);
                dbcontext.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult RemoveItem(int proId)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                Customer ep = dbcontext.Customers.Where(p => p.ID == proId).FirstOrDefault();
                List<Loan> lstLoans = dbcontext.Loans.Where(p => p.IDCus == proId).ToList();

                dbcontext.Customers.Remove(ep);

                foreach (var item in lstLoans)
                    dbcontext.Loans.Remove(item);

                dbcontext.SaveChanges();

                ViewBag.Delete = true;
                return RedirectToAction("LoadCustomer", "Home");
            }
            
        }

        [HttpPost]
        public JsonResult DeleteCustomer(int id)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                try
                {
                    Customer cus = dbcontext.Customers.Where(o => o.ID == id).FirstOrDefault();
                    List<Loan> lstLoans = dbcontext.Loans.Where(p => p.IDCus == id).ToList();
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
                        temp.OldCode = cus.Code;
                        dbcontext.Customers.Add(temp);

                        foreach (var item in lstLoans)
                        {
                            dbcontext.Loans.Remove(item);
                        }

                        dbcontext.Customers.Remove(cus);
                        dbcontext.SaveChanges();
                        result["status"] = "success";
                    }

                }
                catch (Exception ex)
                {
                    result["status"] = "error";
                    result["message"] = ex.Message;
                }

                return Json(result);
            }
            
        }

        [HttpPost]
        public JsonResult AddLoan(CustomLoan l)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                int money = Int32.Parse(l.Money);

                Customer cs = dbcontext.Customers.Where(p => p.ID == l.IDCus).FirstOrDefault();

                Loan model = new Loan();
                model.Date = l.Date;
                model.Status = 1;
                model.IDCus = l.IDCus;
                model.Type = true;
                model.money = money;

                dbcontext.Loans.Add(model);

                Const.timetemp = DateTime.Now.ToString();
                dbcontext.SaveChanges();
                WriteHistory(cs, money, model.ID);

                return Json(new { amountpaid = cs.AmountPaid, remainingamount = cs.RemainingAmount },
                    JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult ResetDatetime(int type, string message, DateTime datetime)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                if (!string.IsNullOrEmpty(message))
                {
                    Message msg = new Message();
                    msg.Message1 = message;
                    msg.Date = datetime;
                    msg.type = 1;
                    dbcontext.Messages.Add(msg);
                    dbcontext.SaveChanges();

                }
            }

            

            return Json(type);
        }
        public void WriteHistory(Customer p, int money, int loanid)
        {
            using (CamdoAnhTuEntities1 dbcontext = new CamdoAnhTuEntities1())
            {
                var cookie = Request.Cookies["Chonngaylam"];
                StringBuilder str = new StringBuilder();
                int type = 0;

                if (cookie.Value == null || string.IsNullOrEmpty(cookie.Value))
                    throw new Exception("chua chọn ngày làm");

                if (loanid != -1)
                {
                    var checkhs = dbcontext.histories.Where(s => s.loanid == loanid).FirstOrDefault();
                    if (checkhs == null)
                    {
                        var checkLoan = dbcontext.Loans.Where(s => s.ID == loanid).FirstOrDefault();

                        if (checkLoan.Status == 1)
                        {
                            type = 0;
                            history hs = new history();
                            str.Append("Xóa đóng tiền cho ngày: " + Const.timetemp);
                            hs.CustomerId = p.ID;
                            hs.CustomerCode = p.Code;
                            hs.Detail = str.ToString();
                            hs.Ngaydongtien = cookie.Value != null ? DateTime.Parse(cookie.Value) : throw new Exception("Chưa chọn ngày làm");
                            hs.price = money == 0 ? p.Price : money;
                            hs.status = type;
                            hs.loanid = loanid;
                            dbcontext.histories.Add(hs);
                        }
                        else
                        {
                            type = 1;
                            history hs = new history();
                            str.Append("Đóng tiền cho ngày: " + Const.timetemp);
                            hs.CustomerId = p.ID;
                            hs.CustomerCode = p.Code;
                            hs.Detail = str.ToString();
                            hs.Ngaydongtien = cookie.Value != null ? DateTime.Parse(cookie.Value) : throw new Exception("Chưa chọn ngày làm");
                            hs.price = money == 0 ? p.Price : money;
                            hs.status = type;
                            hs.loanid = loanid;
                            dbcontext.histories.Add(hs);
                        }
                    }
                    else
                    {
                        int oldtype = checkhs.status.Value;

                        if (oldtype == 1) // xóa dong tien
                        {
                            str.Append("Xóa đóng tiền cho ngày: " + Const.timetemp);
                            checkhs.Ngaydongtien = cookie.Value != null ? DateTime.Parse(cookie.Value) : throw new Exception("Chưa chọn ngày làm");
                            type = 0;
                            checkhs.status = type;
                            checkhs.Detail = str.ToString();
                        }
                        else // đóng tien
                        {
                            str.Append("Đóng tiền cho ngày: " + Const.timetemp);
                            type = 1;
                            checkhs.Ngaydongtien = cookie.Value != null ? DateTime.Parse(cookie.Value) : throw new Exception("Chưa chọn ngày làm");
                            checkhs.status = type;
                            checkhs.Detail = str.ToString();
                        }

                    }
                }
                else if (loanid == -1)
                {
                    history hs = new history();
                    str.Append("Kết thúc dây nợ ngày : " + Const.timetemp);
                    hs.CustomerId = p.ID;
                    hs.Detail = str.ToString();
                    hs.CustomerCode = p.Code;
                    hs.Ngaydongtien = cookie.Value != null ? DateTime.Parse(cookie.Value) : throw new Exception("Chưa chọn ngày làm");
                    hs.price = money == 0 ? p.Price : money;
                    hs.status = 0;
                    hs.loanid = -1;
                    dbcontext.histories.Add(hs);
                }

                dbcontext.SaveChanges();
            }
            

        }

        public ActionResult ChonNgay(DateTime? chonngaylamSubmit)
        {
            HttpCookie cookie = new HttpCookie("Chonngaylam");
           
            if (chonngaylamSubmit.HasValue)
            {
                cookie.Expires = DateTime.Now.AddDays(1);
                cookie.Value = chonngaylamSubmit.Value.ToString(); ;
                HttpContext.Response.Cookies.Add(cookie);
            }

            //chonngaylam = DateTime.Parse(chonngaylamVal);
            return RedirectToAction("Index");
        }
    }
}