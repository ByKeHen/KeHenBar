using kehenbar.DataBase;
using kehenbar.model;
using kehenbar.Template;
using kehenbar.web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kehenbar.web.Controllers
{
    public class ContentController : Controller
    {
        //
        // GET: /Content/

        public void Index(int id)
        {

            string page = Request["page"] + "";
            TempMain main = new TempMain("/content.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数

                string commentcount = SqlHelper.Query("select count(1) from forumscomment where forums_id=" + id).Rows[0][0]+"";

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_id]", id.ToString())
                    .Replace("[myparm_userid]",Session["UserId"]+"")
                    .Replace("[myparm_commentcount]", commentcount);
                #endregion
            };
            main.ParseHtml(page);
        }

        [IsLoginMember]
        [ValidateInput(false)]
        public string Reply(int tieid,string content)
        {
            string userid = Session["UserId"] + "";

            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.field = JsonConvert.SerializeObject(new {
                users_id = userid,
                forums_id = tieid,
                content = content,
                addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                state = 1,
                zan = 0
            });
            fm.table = "forumscomment";
            fm.where = "";

            try
            {
                DataTable table = SqlHelper.Query(string.Join(" ", new string[]{
                    "select users_id,id,title from forums",
                    "where id="+tieid
                }));

                SendMessage(table.Rows[0][0]+"", string.Join(" ", new string[] { 
                    "<a href=\"/member/home/"+table.Rows[0][0]+""+"\">"+Session["UserName"]+"</a>",
                    "回复了您的帖子",
                    "<a href=\"/content/index/"+table.Rows[0][1]+""+"\">"+table.Rows[0][2]+""+"</a>"
                }));
            }
            catch{}
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        [IsLoginMember]
        public void SendMessage(string toUserId, string content)
        {
            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.table = "usersmsg";
            fm.where = "";
            fm.field = JsonConvert.SerializeObject(new
            {
                users_id = toUserId,
                content = content,
                addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            try
            {
                new DbEntity().Index(JsonConvert.SerializeObject(fm));
            }
            catch{}            
        }

        [IsLoginMember]
        public string DianZan(int tieid, int plid)
        {
            string userid = Session["UserId"] + "";

            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.where = "";
            fm.table = "forumsgg";
            fm.field = JsonConvert.SerializeObject(new
            {
                users_id = userid,
                forums_id = tieid,
                forumscomment_id = plid,
                addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));

            return resultjson;
        }

        [IsLoginMember]
        public string ShanChupl(int tieid,int plid)
        {
            string userid = Session["UserId"] + "";
            string fatieren = SqlHelper.Query(string.Join(" ",new string[]{
                "select u.id"
                ,"from users u"
                ,"inner join forums f on u.id = f.users_id"
                ,"inner join forumscomment fc on fc.forums_id = f.id"
                ,"where fc.id = "+plid
            })).Rows[0][0] + "";

            if (userid == fatieren)
            {
                //删除评论
                int result = SqlHelper.ExecuteSql(string.Join(" ", new string[]{
                    "delete from forumscomment where id="+plid
                }));                

                if (result > 0) {
                    return "success";
                }
                else
                {
                    return "失败";
                }
            }
            else
            {
                return "网络错误";
            }
        }

        [IsLoginMember]
        [ValidateInput(false)]
        public string ReplyPserson(int plid, string content)
        {
            string userid = Session["UserId"] + "";

            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.field = JsonConvert.SerializeObject(new
            {
                users_id = userid,
                forumscomment_id = plid,
                content = content,
                addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            fm.table = "forumscommentReplay";
            fm.where = "";

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        [IsLoginMember]
        public string shoucang(int tieid)
        {
            string userid = Session["UserId"] + "";
            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.where = "";
            fm.table = "forumscollec";
            fm.field = JsonConvert.SerializeObject(new {
                users_id = userid,
                forums_id = tieid,
                addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        [IsLoginMember]
        public string qxshoucang(int tieid)
        {
            string userid = Session["UserId"] + "";
            FormModel fm = new FormModel();
            fm.action = "delete";
            fm.where = "forumscollec.forums_id=" + tieid + ",forumscollec.users_id=" + userid;
            fm.table = "forumscollec";
            fm.field = "";
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        [IsLoginMember]
        public void Edit(int id)
        {
            TempMain main = new TempMain("/edit.html"); 
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                Random rdm = new Random();
                int leftv = rdm.Next(1, 10);
                int rightv = rdm.Next(1, 10);
                Session["VerCode"] = leftv + rightv;

                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_userid]", Session["UserId"] + "")
                    .Replace("[myparm_id]", id+"")
                    .Replace("[VerCode]", leftv + " + " + rightv + " = ?");
                #endregion
            };
            main.ParseHtml();
        }

        [IsLoginMember]
        public string CaiNa(int plid)
        {
            string userid = Session["UserId"] + "";
            DataTable table = SqlHelper.Query(string.Join(" ", new string[]{
                "select u.id,f.feiwen,f.id"
                ,"from users u"
                ,"inner join forums f on u.id = f.users_id"
                ,"inner join forumscomment fc on fc.forums_id = f.id"
                ,"where fc.id = "+plid
            }));
            if (table == null || table.Rows.Count <= 0) { return "没找到帖子信息"; }

            if (userid == table.Rows[0][0]+"")
            {
                string jiangli = table.Rows[0][1] + "";
                string tieid = table.Rows[0][2] + "";

                string huifuren = SqlHelper.Query(string.Join(" ",new string[]{
                    "select u.id"
                    ,"from users u"
                    ,"inner join forumscomment fc on fc.users_id=u.id"
                    ,"where fc.id="+plid
                })).Rows[0][0] + "";

                SqlHelper.ExecuteSql(string.Join(" ", new string[] { 
                    "update forums set state=2 where id="+tieid
                    ,";update forumscomment set state=2 where id="+plid
                    ,";update users set feiwen=feiwen+"+jiangli
                    ,"where id="+huifuren
                }));

                return "success";
            }
            else {
                return "网络错误";
            }
        }

        [IsLoginMember]
        [ValidateInput(false)]
        public string EditSave(string title, string content, int leibie, int experience, int vercode, int tieid)
        {
            ResultModel rm = new ResultModel();
            if (vercode + "" != Session["VerCode"] + "")
            {
                rm.code = 1;
                rm.msg = "验证码不正确";
                return JsonConvert.SerializeObject(rm);
            }

            FormModel fm = new FormModel();
            fm.action = "update";
            fm.where = "forums.id=" + tieid;
            fm.table = "forums";
            fm.field = JsonConvert.SerializeObject(new
            {
                forumstype_id = leibie,
                users_id = Session["UserId"] + "",
                title = title,
                content = content,
                feiwen = experience
            });

            string resultJson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultJson;
        }

        public void Doc(string id)
        {

            TempMain main = new TempMain("/doc.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数                

                id = string.IsNullOrEmpty(id) ? "1" : SqlFunction.SqlFilter(id);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_id]", id);
                #endregion
            };
            main.ParseHtml();
        }

        [IsLoginMember]
        public void Add()
        {
            TempMain main = new TempMain("/add.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                Random rdm = new Random();
                int leftv = rdm.Next(1, 10);
                int rightv = rdm.Next(1, 10);
                Session["VerCode"] = leftv + rightv;

                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_userid]", Session["UserId"] + "")
                    .Replace("[VerCode]", leftv + " + " + rightv + " = ?");
                #endregion
            };
            main.ParseHtml();
        }

        [IsLoginMember]
        [ValidateInput(false)]
        public string AddSave(string title, string content, int leibie, int experience, int vercode)
        {
            ResultModel rm = new ResultModel();
            if (vercode+"" != Session["VerCode"]+"")
            {
                rm.code = 1;
                rm.msg = "验证码不正确";
                return JsonConvert.SerializeObject(rm);
            }

            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.table = "forums";
            fm.where = "";
            fm.field = JsonConvert.SerializeObject(new
            {
                forumstype_id = leibie,
                users_id = Session["UserId"]+"",
                title = title,
                addtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                content = content,
                feiwen = experience,
                istop=0,
                isjing=0,
                state=1,
                seecount=0,
                writecount=0
            });

            string resultJson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultJson;
        }
    }
}
