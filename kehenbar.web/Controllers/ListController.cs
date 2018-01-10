using kehenbar.model;
using kehenbar.Template;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class ListController : Controller
    {
        //
        // GET: /List/
        public void Index(string id)
        {
            string page = Request["page"] + "";
            string userid = Session["UserId"] + "";
            TempMain main = new TempMain("/list.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string myWhere = string.Empty;
                if (!string.IsNullOrEmpty(id))
                {
                    int _id = Convert.ToInt32(id);
                    switch (_id)
                    {
                        case 1:
                            //未结贴
                            myWhere = " where=(forums.state:1) ";
                            break;
                        case 2:
                            //未结贴
                            myWhere = " where=(forums.state:2) ";
                            break;
                        case 3:
                            //未结贴
                            myWhere = " where=(forums.isjing:1) ";
                            break;
                        case 4:
                            //未结贴
                            myWhere = " where=(forums.users_id:" + userid + ") ";
                            break;
                        default:
                            myWhere = "";
                            break;
                    }
                }
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myWhere]", myWhere);
                #endregion
            };
            main.ParseHtml(page);
        }
    }
}
