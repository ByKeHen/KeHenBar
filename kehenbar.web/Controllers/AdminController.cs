using kehenbar.model;
using kehenbar.Template;
using Newtonsoft.Json;
using kehenbar.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        [IsLogin]
        public void Index()
        {
            TempMain main = new TempMain("/admin/index.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string id = Request["topMenuId"];
                if (string.IsNullOrEmpty(id))
                {
                    if (string.IsNullOrEmpty(Session["topMenuId"] + ""))
                    {
                        id = "21";
                    }
                    else
                    {
                        id = Session["topMenuId"] + "";
                    }
                }
                else
                {
                    Session["topMenuId"] = id;
                }

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[topMenuId]", id);
                #endregion
            };

            main.ParseHtml();
        }

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录系统
        /// </summary>
        /// <param name="dengluming"></param>
        /// <param name="mima"></param>
        /// <returns></returns>
        public string LoginAc(string dengluming, string mima)
        {
            string sql = "select * from users where loginname=@dengluming and password = @mima";
            SqlParameter[] parm = { 
                                  new SqlParameter("@dengluming",dengluming),
                                  new SqlParameter("@mima",MD5.md5(mima))
                                  };
            DataTable userTable = SqlHelper.Query(sql, parm).Tables[0];

            if (userTable != null && userTable.Rows.Count >0)
            {
                Session["UserId"] = userTable.Rows[0]["id"] + "";
                Session["UserName"] = userTable.Rows[0]["name"] + "";

                return "success";
            }
            else
            {
                return "登录失败";
            }
        }
    }
}
