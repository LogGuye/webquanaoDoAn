using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly QLCHQA db = new QLCHQA();
        // GET: SanPham
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var sp = db.SanPhams.FirstOrDefault(x => x.MaSP == id);
            if (sp == null) return HttpNotFound();
            return View(sp);
        }

    }

}


