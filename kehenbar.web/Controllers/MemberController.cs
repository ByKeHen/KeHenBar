using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kehenbar.model;
using kehenbar.Template;
using kehenbar.web.Models;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;
using kehenbar.DataBase;
using kehenbar.common;

namespace kehenbar.web.Controllers
{
    public class MemberController : Controller
    {
        //
        // GET: /Member/

        //用户中心
        [IsLoginMember]
        public void Index()
        {
            string page = Request["page"] + "";
            TempMain main = new TempMain("/user/index.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string userId = Session["UserId"] + "";
                DataTable table = SqlHelper.Query("select count(1) from forums where users_id=" + userId);
                string allcount = table.Rows[0][0].ToString();

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_allcount]", allcount)
                    .Replace("[myparm_userid]", userId);
                #endregion
            };
            main.ParseHtml(page);
        }

        //设置
        [IsLoginMember]
        public void Set()
        {
            TempMain main = new TempMain("/user/set.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string userId = Session["UserId"] + "";
                
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_userid]", userId);
                #endregion
            };
            main.ParseHtml();
        }

        [IsLoginMember]
        public string XiuGai(string username, int sex, string city, string sign)
        {
            string userid = Session["UserId"] + "";
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.table = "users";
            fm.where = "id=" + userid;
            fm.field = JsonConvert.SerializeObject(new {
                name = username,
                sex = sex,
                city = city,
                jianjie = sign.Trim()
            });

            string resultJson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultJson;
        }

        [IsLoginMember]
        public string XiuGaimm(string nowpass, string pass, string repass)
        {
            nowpass = SqlFunction.SqlFilter(nowpass);
            string userid = Session["UserId"] + "";
            string count = SqlHelper.Query(string.Join(" ", new string[]{
                "select count(1) from users",
                "where id="+userid+" and password='"+MD5.md5(nowpass)+"'"
            })).Rows[0][0] + "";

            if (int.Parse(count)>0)
            {
                FormModel fm = new FormModel();
                fm.action = "update";
                fm.table = "users";
                fm.where = "id=" + userid + ",password=" + MD5.md5(nowpass);
                fm.field = JsonConvert.SerializeObject(new {
                    password = MD5.md5(pass)
                });

                string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
                return resultjson;
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = "网络错误"
                });
            }
        }

        [IsLoginMember]
        public string XiuGaitx(string path)
        {
            string userid = Session["UserId"] + "";
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.table = "users";
            fm.where = "id="+userid;
            fm.field = JsonConvert.SerializeObject(new {
                face = path
            });

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        //主页
        public void Home(string id)
        {
            TempMain main = new TempMain("/user/home.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                id = string.IsNullOrEmpty(id) ? "-1" : SqlFunction.SqlFilter(id);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_userid]", id);
                #endregion
            };
            main.ParseHtml();
        }
        //消息
        [IsLoginMember]
        public void Message()
        {
            string page = Request["page"] + "";
            TempMain main = new TempMain("/user/message.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string userId = Session["UserId"] + "";
                
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_userid]", userId);
                #endregion
            };
            main.ParseHtml(page);
        }

        //找回密码
        public void Forget()
        {
            TempMain main = new TempMain("/user/forget.html");

            main.ParseHtml();
        }

        //邮箱激活
        [IsLoginMember]
        public void Activate()
        {
            TempMain main = new TempMain("/user/activate.html");

            main.ParseHtml();
        }

        [IsLoginMember]
        public void Logout()
        {
            Session["UserId"] = null;
            Response.Redirect("/");
        }

        public void Reg()
        {
            TempMain main = new TempMain("/user/reg.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数

                Random rdm = new Random();
                int leftv = rdm.Next(1, 10);
                int rightv = rdm.Next(1, 10);
                Session["VerCode"] = leftv + rightv;
                
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[VerCode]", leftv + " + " +rightv+" = ?");
                #endregion
            };

            main.ParseHtml();
        }

        public void Login()
        {
            TempMain main = new TempMain("/user/login.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数

                Random rdm = new Random();
                int leftv = rdm.Next(1, 10);
                int rightv = rdm.Next(1, 10);
                Session["VerCode"] = leftv + rightv;

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[VerCode]", leftv + " + " + rightv + " = ?");
                #endregion
            };

            main.ParseHtml();
        }

        [IsLoginMember]
        public string ShanChuxx(int mid)
        {
            string userid = Session["UserId"] + "";
            FormModel fm = new FormModel();
            fm.action = "delete";
            fm.field = "";
            fm.table = "usersmsg";
            fm.where = "usersmsg.id=" + mid + ",usersmsg.users_id=" + userid;

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        [IsLoginMember]
        public string QingKongxx()
        {
            string userid = Session["UserId"] + "";
            FormModel fm = new FormModel();
            fm.action = "delete";
            fm.field = "";
            fm.table = "usersmsg";
            fm.where = "usersmsg.users_id=" + userid;

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public void LoginFast()
        {
            TempMain main = new TempMain("/user/loginform.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数

                Random rdm = new Random();
                int leftv = rdm.Next(1, 10);
                int rightv = rdm.Next(1, 10);
                Session["VerCode"] = leftv + rightv;

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[VerCode]", leftv + " + " + rightv + " = ?");
                #endregion
            };

            main.ParseHtml();
        }

        public string LoginSave(string email, string pass, string vercode)
        {
            if (vercode != Session["VerCode"] + "")
            {
                return "验证码不正确";
            }

            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.order = "";
            fm.table = "users";
            fm.where = "loginname=" + email + ",password=" + MD5.md5(pass);

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            DataTable resultTable = JsonConvert.DeserializeObject<DataTable>(resultjson);
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                Session["UserId"] = resultTable.Rows[0]["usersid"] + "";
                Session["UserName"] = resultTable.Rows[0]["usersname"] + "";
                return "success";
            }
            else
            {
                return "用户名或者密码错误";
            }
        }

        public string RegSave(string email, string rname, string pass, string repass, string vercode)
        {
            if (pass != repass) {
                return "两次输入的密码不一致";
            }
            if (vercode != Session["VerCode"] + "")
            {
                return "验证码不正确";
            }

            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.order = "";
            fm.table = "users";
            fm.where = "loginname=" + email;

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            DataTable resultTable = JsonConvert.DeserializeObject<DataTable>(resultjson);
            if (resultTable != null && resultTable.Rows.Count > 0)
            {
                return "登录名已存在";
            }

            fm = new FormModel();
            fm.action = "insert";
            fm.field = JsonConvert.SerializeObject(new { loginname = email, password = MD5.md5(pass), name = rname, registerdate =DateTime.Now.ToString()});
            fm.order = "";
            fm.table = "users";
            fm.where = "";

            ResultModel rm = JsonConvert.DeserializeObject<ResultModel>(new DbEntity().Index(JsonConvert.SerializeObject(fm)));
            if (rm.code == 0)
            {
                return rm.msg;
            }
            else {
                return rm.msg;
            }
        }

        public void LoginAndRegHtml()
        {
            string userid = Session["UserId"] + "";
            string result1 = string.Join("", new string[]{
                "<a class='unlogin' href='/member/login'>",
                    "<i class='iconfont icon-touxiang'></i>",
                "</a>",
                "<span>",
                    "<a href='/member/login'>登入</a>",
                    "<a href='/member/reg'>注册</a>",
                "</span>   ",
            });

            string result2 = string.Join("", new string[] { 
                "<a class='avatar' href='/member/index'>",
                    "<img src='#userface#'>",
                    "<cite>#username#</cite>",
                    "<i>#vip#</i>",
                "</a>",
                "<div class='nav'>",
                    "<a href='/member/set'><i class='iconfont icon-shezhi'></i>设置</a>",
                    "<a href='/member/logout'><i class='iconfont icon-tuichu' style='top: 0; font-size: 22px;'></i>退了</a>",
                "</div>"
            });

            if (string.IsNullOrEmpty(userid))
            {
                Response.Write("document.write(\"" + result1 + "\")");
            }
            else
            {
                FormModel fm = new FormModel();
                fm.action = "select";
                fm.field = "";
                fm.order = "";
                fm.table = "users";
                fm.where = "id=" + userid;
                string resultJson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
                DataTable table = JsonConvert.DeserializeObject<DataTable>(resultJson);
                result2 = result2.Replace("#userface#", table.Rows[0]["usersface"] + "")
                    .Replace("#vip#", "VIP1")
                    .Replace("#username#", table.Rows[0]["usersname"] + "");

                Response.Write("document.write(\"" + result2 + "\")");
            }
        }
	}
}