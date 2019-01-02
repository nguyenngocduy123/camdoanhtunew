using CamDoAnhTu.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;
using CamDoAnhTu.ViewModel;
using System;
using CamDoAnhTu.Helper;

namespace CamDoAnhTu.Controllers
{
    [SessionTimeout]
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult Index(int type, int typemsg)
        {
            ViewBag.type = type;

            //using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            //{
            //    var result = ctx.Messages.Where(p=> p.type == typemsg).ToList();
            //    return View(result);
            //}
            return View();
        }

        public ActionResult LoadData(DataTableAjaxPostModel model)
        {

            int pageSize = Convert.ToInt32(model.length);
            int skip = Convert.ToInt32(model.start);
            int recordsTotal = 0;
            int draw = model.draw;
            string sortBy = "ID";
            bool sortDir = true;

            if (model.order != null)
            {
                // in this example we just default sort on the 1st column
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
            }

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                var customerData = (from tempcustomer in ctx.Messages
                                    select tempcustomer);

                //Search    
                if (!string.IsNullOrEmpty(model.search.value))
                {
                    customerData = customerData.Where(m => m.Message1.Contains(model.search.value));
                }

                recordsTotal = customerData.Count();

                var data = customerData.OrderBy(sortBy, sortDir).Skip(skip).Take(pageSize).ToList();
                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
        }

        [HttpPost]
        public ActionResult DeleteMessage(int id)
        {

            using (CamdoAnhTuEntities1 ctx = new CamdoAnhTuEntities1())
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                var message = ctx.Messages.Where(p => p.Id == id).FirstOrDefault();

                if (message != null)
                {
                    ctx.Messages.Remove(message);
                    ctx.SaveChanges();
                }
                result["status"] = "success";
                return Json(result);
            }
        }
    }
}