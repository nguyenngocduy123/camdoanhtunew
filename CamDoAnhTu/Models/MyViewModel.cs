using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CamDoAnhTu.Models
{
    public class MyViewModel
    {
        public int SelectedLoaiGiayTo { get; set; }
        public SelectList ListLoaiGiayTo { get; set; }
        public Customer model { get; set; }
        public HttpPostedFileBase fuMain { get; set; }
        public int sokhachbiloai { get; set; }
        public int constTA { get; set; }

        public MyViewModel()
        {
            model = new Customer();
        }
    }
}