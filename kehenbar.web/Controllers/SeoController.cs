using myBlog.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myBlog.web.Controllers
{
    public class SeoController : Controller
    {
        //
        // GET: /Seo/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 推送文章
        /// </summary>
        /// <returns></returns>
        public string Tswz(string id)
        {
            string[] url = new string[] { "http://www.ppkanshu.com/article/info/" + id };

            string res = SeoHelper.PostUrl(url);

            return res;
        }
    }
}
