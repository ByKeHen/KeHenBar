using kehenbar.model;
using kehenbar.Template;
using kehenbar.web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using kehenbar.common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public void Index()
        {

            bool havekey = WebConfigHelper.HaveConfigKey("", "ConnectionString");
            if (!havekey) {
                Create();
                return;
            }

            string page = Request["page"] + "";
            string shouyetemp = SqlHelper.GetSingle("select shouyetemp from siteconfig where id=1") + "";
            if (string.IsNullOrEmpty(shouyetemp.Trim()))
            {
                TempMain main = new TempMain("/index.html");
                main.ParseHtml(page);
            }
            else
            {
                TempMain main = new TempMain(shouyetemp);
                main.ParseHtml(page);
            }
        }

        public void Create()
        {
            TempMain main = new TempMain("/create/index.html");
            main.ParseHtml();
        }

        public void NotFound()
        {
            TempMain main = new TempMain("/404.html");
            main.ParseHtml(); 
        }

        public string createConnection(int shujukuleixing, string dizhi, string mingcheng, string yonghuming, string mima)
        {
            string constr = "server=" + dizhi + ";database=" + mingcheng + ";uid=" + yonghuming + ";pwd=" + mima;
            string returnjson = string.Empty;
            SqlConnection connection = new SqlConnection(constr);
            try
            {
                connection.Open();
                returnjson = JsonConvert.SerializeObject(new { 
                    code=0,
                    connection = constr,
                    dbname = mingcheng
                });
            }
            catch
            {
                returnjson = JsonConvert.SerializeObject(new
                {
                    code = 1,
                    connection = ""
                });
            }
            finally {
                connection.Close();
            }
            return returnjson;
        }

        public string createTable(string lianjie, string mokuai, string dbname)
        {
            string returnjson = string.Empty;

            string sqlcontent = TextHelper.GetContet("/data/sql.txt");
            JObject jobject = JObject.Parse(sqlcontent);
            if (jobject["code"] + "" == "0")
            {
                sqlcontent = "USE [" + dbname + "];GO;" + jobject["msg"] + "";
                string sql = SqlHelper.ExecuteSqlForGo(sqlcontent, lianjie);
                if (!string.IsNullOrEmpty(sql))
                {
                    return JsonConvert.SerializeObject(new
                    {
                        code = 1,
                        msg = "error"
                    });
                }
                else
                {
                    WebConfigHelper.WriteConfig("", "ConnectionString", lianjie);
                    return JsonConvert.SerializeObject(new
                    {
                        code = 0,
                        msg = "success"
                    });
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = "sql空"
                });
            }
        }
    }
}
