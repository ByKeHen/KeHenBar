using myBlog.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myBlog.web.Controllers
{
    public class TopController : Controller
    {
        //
        // GET: /Top/
        myBlogEntities db = new myBlogEntities();
        public ActionResult Index()
        {
            List<mb_categorys> cateList = db.mb_categorys.Where(o => o.cshow == 1).ToList();

            return View(cateList);
        }
    }
}
