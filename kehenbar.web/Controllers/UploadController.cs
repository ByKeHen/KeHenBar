using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using kehenbar.common;

namespace kehenbar.web.Controllers
{
    [IsLoginMember]
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        public string Image() 
        {
            kehenbar.web.Models.Upload ul = new Models.Upload();
            string forder = Request.QueryString["f"] + ""; //二级目录
            forder = string.IsNullOrEmpty(forder) ? "/uploads/imgs/" : "/uploads/imgs/" + forder + "/";
            
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                string newName = Guid.NewGuid().ToString();
                string filePath = Server.MapPath(forder);
                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                file.SaveAs(filePath + newName + ext);
                ul.code = 0;
                ul.data.src = forder + newName + ext;
                ul.data.title = newName + ext;
            }
            catch (Exception ex)
            {
                ul.code = 1;
                ul.msg = ex.Message;
                ul.data = null;
                Logs.WriteLog("用户【" + Session["UserId"] + "】上传图片失败",2);
            }

            string json = JsonConvert.SerializeObject(ul);
            return json;
        }
        
       
        /// <summary>
        /// 上传图片 - 复制直接上传
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PasteImage(HttpPostedFileBase upload)
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
    }
}
