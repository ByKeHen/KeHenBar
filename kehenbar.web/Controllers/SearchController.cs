using kehenbar.DataBase;
using kehenbar.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public void Index()
        {
            TempMain main = new TempMain("/search.html");
            string page = Request["page"] + "";
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数

                string value = Request["value"] + "";
                value = SqlFunction.SqlFilter(value);

                
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_value]", value);
                #endregion
            };

            main.ParseHtml(page);
        }
    }
}
