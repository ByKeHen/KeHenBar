using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myBlog.web.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasteImage()
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            var filePhysicalPath = Server.MapPath("/uploads/imgs/" + fileName);//我把它保存在网站根目录的 upload 文件夹
            var data1 = Request.Form.ToString();
            var data = data1.Replace("%2f", "/").Replace("%3d", "=");
            byte[] bytes1 = Convert.FromBase64String(data);
            MemoryStream memStream1 = new MemoryStream(bytes1);
            Image a = new Bitmap(memStream1);

            a.Save(filePhysicalPath);
            var url = "/uploads/imgs/" + fileName;
            return Content(url);
        }

        public JsonResult Upload()
        {
            return Json(new { });
        }
    }
}
