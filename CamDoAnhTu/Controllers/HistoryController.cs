using CamDoAnhTu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CamDoAnhTu.Controllers
{
    public class HistoryController : Controller
    {
        // GET: History
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History(int? id, int? type, DateTime? datetime) // id = customerid
        {
            if (type.HasValue)
            {
                ViewBag.type = type.Value;
                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    var lstHistory = new List<history>();

                    if (id.HasValue) // xem theo khach hang
                    {
                        lstHistory = ctx.histories.Where(p => p.CustomerId == id.Value).ToList();
                    }
                    else
                    {
                        datetime = datetime.Value.Date;
                        StringBuilder str = new StringBuilder();

                        lstHistory = ctx.histories
                            .Where(p => p.Ngaydongtien.Value.Year == datetime.Value.Year
                                        && p.Ngaydongtien.Value.Month == datetime.Value.Month
                                        && p.Ngaydongtien.Value.Day == datetime.Value.Day
                                        && (p.Customer.type == type || type == -1) && p.status == 1).ToList();

                        decimal? sum = 0;

                        foreach (var item in lstHistory)
                        {
                            if (item.status == 1)
                                sum += item.price;

                        }

                        ViewBag.TongKet = $"Tổng: {sum.Value:N0}";
                        
                    }
                   
                    return View(lstHistory);
                }
            }
            else
            {
                ViewBag.type = -2;
                using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
                {
                    List<history> lstHistory = ctx.histories.Where(p => p.CustomerId == id).ToList();

                    return View(lstHistory);
                }
            }
        }
    }
}