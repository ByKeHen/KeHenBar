using kehenbar.DataBase;
using kehenbar.model;
using kehenbar.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class CustomController : Controller
    {
        //自定义控制器
        // GET: /Custom/

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        public void Index(int id)
        {
            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.table = "sys_buttons";
            fm.where = "sys_buttons.id=" + id;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            JArray ja = JArray.Parse(resultjson);
            string btnTempPath = ja[0]["sys_buttonstemplatepath"] + "";
            string btnParm = ja[0]["sys_buttonscanshuliebiao"] + "";
            
            TempMain main = new TempMain(btnTempPath);
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                
                Dictionary<string, string> buttoncanshudic = new TempFun().GetFields(btnParm.Trim());
                foreach (var kv in buttoncanshudic)
                {
                    string parmKey = "[" + kv.Key + "]";
                    tm.content = tm.content.Replace(parmKey, Request[kv.Key] + "");
                }
            };

            main.ParseHtml();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public string delete()
        {
            string tablename = Request["__tablename"] + "";
            string rowid = Request["__rowid"] + "";

            if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(rowid))
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = "tablename和rowid参数不全"
                });
            }

            FormModel fm = new FormModel();
            fm.action = "delete";
            fm.field = "";
            fm.table = tablename;
            fm.where = tablename + ".id=" + rowid;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string Save() 
        {
            string tablename = Request["__tablename"] + "";
            string rowid = Request["__rowid"] + "";

            if (string.IsNullOrEmpty(tablename) || string.IsNullOrEmpty(rowid))
            {
                return JsonConvert.SerializeObject(new { 
                    code=1,
                    msg="tablename和rowid参数不全"
                });
            }

            List<string> sqllist = new List<string>();
            List<kehenbar.web.Models.sys_database_clumn> columnlist = new DataBaseController().GetColumnsByTableName(tablename);
            foreach (var item in columnlist)
	        {
                if (Request[item.ccode] != null)
                {

                    sqllist.Add("\""+item.ccode+"\":\""+Request[item.ccode] + "\"");
                }
	        }

            string sqlfiled = string.Join(",", sqllist.ToArray());

            FormModel fm = new FormModel();
            fm.action = "update";
            fm.field = "{" + sqlfiled + "}";
            fm.table = tablename;
            fm.where = tablename + ".id=" + rowid;

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        public string Insert()
        {
            string tablename = Request["__tablename"] + "";

            if (string.IsNullOrEmpty(tablename))
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = "必须要有tablename参数"
                });
            }

            List<string> sqllist = new List<string>();
            List<kehenbar.web.Models.sys_database_clumn> columnlist = new DataBaseController().GetColumnsByTableName(tablename);
            foreach (var item in columnlist)
            {
                if (Request[item.ccode] != null)
                {

                    sqllist.Add("\"" + item.ccode + "\":\"" + Request[item.ccode] + "\"");
                }
            }

            string sqlfiled = string.Join(",", sqllist.ToArray());

            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.field = "{" + sqlfiled + "}";
            fm.table = tablename;
            fm.where = "";

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }
    }
}
