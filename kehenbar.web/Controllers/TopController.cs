using kehenbar.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class TopController : Controller
    {
        //
        // GET: /Top/
        kehenbarEntities db = new kehenbarEntities();
        public ActionResult Index()
        {
            List<mb_categorys> cateList = db.mb_categorys.Where(o => o.cshow == 1).ToList();

            return View(cateList);
        }
    }
}
