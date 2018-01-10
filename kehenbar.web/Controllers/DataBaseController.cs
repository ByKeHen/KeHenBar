using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Data;
using kehenbar.web.Models;
using kehenbar.Template;
using kehenbar.model;
using kehenbar.DataBase;
using Newtonsoft.Json.Linq;

namespace kehenbar.web.Controllers
{
    [IsLogin]
    public class DataBaseController : Controller
    {
        public void Index(string page)
        {
            TempMain main = new TempMain("/admin/database/index.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[topMenuId]", getTopMenuId());
            };

            main.ParseHtml(page);
        }

        #region 维护列表
        public void EditDataTable()
        {
            TempMain main = new TempMain("/admin/database/edit_datatable.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[topMenuId]", getTopMenuId());
            };
            
            main.ParseHtml();
        }

        public string EditDataTableSave(string biaobianhao, string zdbianhao)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.field = JsonConvert.SerializeObject(new { waijianzhi=0});
            fm.table = "sys_database_clumn";
            fm.where = "sys_database.tcode=" + biaobianhao;

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            JObject jsono = JObject.Parse(resultjson);
            if ("0" == jsono["code"] + "")
            {
                fm.action = "update";
                fm.field = JsonConvert.SerializeObject(new { waijianzhi = 1 });
                fm.table = "sys_database_clumn";
                fm.where = "sys_database.tcode=" + biaobianhao + ",sys_database_clumn.ccode=" + zdbianhao;

                resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            }

            return resultjson;
        }

        public string EditDataTable_UpdateShowlb(string biaobianhao, string zdbianhao, int showlb)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.field = JsonConvert.SerializeObject(new { showlb = showlb });
            fm.table = "sys_database_clumn";
            fm.where = "sys_database.tcode=" + biaobianhao + ",sys_database_clumn.ccode=" + zdbianhao;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string EditDataTable_SaveOrderLb(string biaobianhao, string zdbianhao, int orderlb)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.field = JsonConvert.SerializeObject(new { orderlb = orderlb });
            fm.table = "sys_database_clumn";
            fm.where = "sys_database.tcode=" + biaobianhao + ",sys_database_clumn.ccode=" + zdbianhao;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string EditDataTable_MakeLink(string biaobianhao, string zdbianhao, int lianjielx)
        {
            List<string> optionlist = new List<string>();
            //lianjielx 1列表 2表单
            if (lianjielx == 1)
            {
                DataTable table = SqlHelper.Query(string.Join(" ", new string[]{
                    "select tname,tcode,toutkey ",
                    "from dbo.sys_database",
                    "where ISNULL(toutkey,'')<>''"
                }));

                foreach (DataRow item in table.Rows)
                {
                    string toutkeys = item["toutkey"]+"";
                    List<string> toutkeylist = toutkeys.Split(',').ToList();
                    if (toutkeylist.Contains(biaobianhao))
                    {
                        optionlist.Add("<option value=\"/database/datalist/" + item["tcode"] + "/10/1?" + biaobianhao + "=#p1#\">" + item["tname"] + "</option>");
                        continue;
                    }
                }

            }
            else
            {
                optionlist.Add("<option value=\"/database/dataedit/" + biaobianhao + "/#p1#\">编辑页</option>");
                optionlist.Add("<option value=\"/database/datashow/" + biaobianhao + "/#p1#\">查看页</option>");
            }

            return string.Join("", optionlist.ToArray());
        }

        public string EditDataTable_SaveLink(string biaobianhao, string zdbianhao, string linkkx)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.field = JsonConvert.SerializeObject(new { columnlink = linkkx });
            fm.table = "sys_database_clumn";
            fm.where = "sys_database.tcode=" + biaobianhao + ",sys_database_clumn.ccode=" + zdbianhao;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string EditDataTable_DelLink(string biaobianhao, string zdbianhao)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.field = JsonConvert.SerializeObject(new { columnlink = "" });
            fm.table = "sys_database_clumn";
            fm.where = "sys_database.tcode=" + biaobianhao + ",sys_database_clumn.ccode=" + zdbianhao;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string EditDataTable_GetLink(string biaobianhao, string zdbianhao)
        {
            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.table = "sys_database_clumn";
            fm.where = "sys_database.tcode=" + biaobianhao + ",sys_database_clumn.ccode=" + zdbianhao;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            JArray ja = JArray.Parse(resultjson);

            return JsonConvert.SerializeObject(new {
                columnlink = ja[0]["sys_database_clumncolumnlink"] + ""
            });
        }
        #endregion

        #region 触发器
        public void Trigger()
        {
            TempMain main = new TempMain("/admin/trigger/index.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[topMenuId]", getTopMenuId());
            };

            main.ParseHtml();
        }

        public string Trigger_YanzhengSQL(string sql)
        {
            string resultjson = SqlHelper.ValidateSQL(sql);
            return resultjson;
        }

        public string Trigger_BaoCunSQL(string tablename, string triggerenent, string tiggername, string sqlcontent) 
        { 
            string resultjson = Trigger_YanzhengSQL(sqlcontent);
            JObject jo = JObject.Parse(resultjson);
            if ("0" != jo["code"] + "")
            {

                return resultjson;
            }
            else
            {
                string sqlinsert = string.Join(" ", new string[]{
                    "insert into sys_trigger(sys_database_id,event,tiggername,sqlcontent)values",
                    "(@tablename,@triggerenent,@tiggername,@sqlcontent)"
                });

                try
                {
                    SqlParameter[] sqlparm = {
                                             new SqlParameter("@tablename",tablename),
                                             new SqlParameter("@triggerenent",triggerenent),
                                             new SqlParameter("@tiggername",tiggername),
                                             new SqlParameter("@sqlcontent",sqlcontent),
                                             };

                    SqlHelper.ExecuteSql(sqlinsert, sqlparm);
                    return JsonConvert.SerializeObject(new
                    {
                        code=0,
                        msg = "添加成功"
                    });
                }
                catch (Exception ex)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        code = 1,
                        msg = ex.Message
                    });
                }
            }
        }

        public string Trigger_ZhiXingSQL(string sql)
        {
            string resultjson = Trigger_YanzhengSQL(sql);
            JObject jo = JObject.Parse(resultjson);
            if("0" != jo["code"]+""){

                return resultjson;
            }
            else
            {
                #region 执行sql
                string sqltemp = sql.Trim().Substring(0, 10).ToLower();
                if (sqltemp.Contains("select"))
                {
                    try
                    {
                        DataTable dt = SqlHelper.Query(sql);
                        return JsonConvert.SerializeObject(new
                        {
                            code = 0,
                            data = JsonConvert.SerializeObject(dt),
                            datacount = dt.Rows.Count
                        });
                    }
                    catch (Exception ex)
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            code = 1,
                            msg=ex.Message
                        });
                    }
                    
                }
                else
                {
                    try
                    {
                        int resultcount = SqlHelper.ExecuteSql(sql);
                        return JsonConvert.SerializeObject(new
                        {
                            code = 2,
                            data = JsonConvert.SerializeObject(new { }),
                            datacount = resultcount
                        });
                    }
                    catch (Exception ex)
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            code = 1,
                            msg = ex.Message
                        });
                    }

                }
                #endregion
            }
        }
        #endregion

        #region 按钮管理
        public void ButtonIndex()
        {
            TempMain main = new TempMain("/admin/button/index.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[topMenuId]", getTopMenuId());
            };
            main.ParseHtml();
        }

        public string ButtonSave(string biaoming,string chicun,string yanse, string mingcheng, string shijian, string dakaifangshi, string tankuangchicun, string baifenbi_k, string baifenbi_g, string xiangsu_k, string xiangsu_g, string mobanlujing, string canshu)
        {
            #region sql
            string sqlinsert = string.Join(" ", new string[]{
                "insert into sys_buttons(",
                    "sys_database_id",
                    ",bname",
                    ",chicun",
                    ",yanse",
                    ",benent",
                    ",bopentype",
                    ",tankuangchicun",
                    ",baifenbi_k",
                    ",baifenbi_g",
                    ",xiangsu_k",
                    ",xiangsu_g",
                    ",templatepath",
                    ",canshuliebiao",
                    ",anniuhtml)",
                "values(",
                    "'"+biaoming+"',",
                    "'"+mingcheng+"',",
                    "'"+chicun+"',",
                    "'"+yanse+"',",
                    shijian+",",
                    dakaifangshi+",",
                    tankuangchicun+",",
                    baifenbi_k+",",
                    baifenbi_g+",",
                    xiangsu_k+",",
                    xiangsu_g+",",
                    "'"+mobanlujing+"',",
                    "'"+canshu+"',",
                    "''",
                ")",
                ";select @@identity"
            });
            #endregion

            try
            {
                string buttonId = SqlHelper.GetSingle(sqlinsert).ToString();
                string buttonHtml = "{kehenbar:button id=" + buttonId + "}";
                return JsonConvert.SerializeObject(new
                {
                    code=0,
                    html = buttonHtml
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = ex.Message
                });
            }
        }
        #endregion

        [ValidateInput(false)]
        public void AjaxLoad(string content)
        {
            TempMain main = new TempMain();
            main.BeforeFunc += delegate(Object sender, TempMain.BeforeFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = content.Replace("kehenbar:tlist", "kehenbar:list")
                    .Replace("kehenbar:tif", "kehenbar:if");
            };
            string result = main.ReturnHtml();
            Response.Write("<textarea>" + result + "</textarea>");
        }

        public void TableClumnEdit(string id)
        {
            TempMain main = new TempMain("/admin/database/edit_table_columns.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_tcode]", id);
            };
            main.ParseHtml();
        }

        public void Addoutkey(string id)
        {
            id = SqlFunction.SqlFilter(id);

            string toukeystr = SqlHelper.Query("select toutkey from sys_database where tcode='" + id + "'").Rows[0][0] + "";

            TempMain main = new TempMain("/admin/database/add_table_outkey.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_tcode]", id);
                tm.content = tm.content.Replace("[myparm_toukeystr]", toukeystr);
            };
            main.ParseHtml();
        }

        public DataTable GetTableByCode(string tcode)
        {
            tcode = SqlFunction.SqlFilter(tcode);
            DataTable dt = SqlHelper.Query("select * from sys_database where tcode = '" + tcode + "'");
            return dt;
        }

        public string ClumnDel(string id, string biaobh)
        {
            id = SqlFunction.SqlFilter(id);
            biaobh = SqlFunction.SqlFilter(biaobh);
            ColumnModel clumn = new DbEntity().GetTableColumn(biaobh).Find(o => o.ccode == id);
            if (clumn == null)
            {
                return "没找到这个字段";
            }
            else
            {
                string sqlDel = string.Join(" ", new string[]{
                     "delete from sys_database_clumn where sys_database_id =("
	                ,"   select id from sys_database"
	                ,"   where tcode = '"+biaobh+"'"
                    ,")and ccode = '"+id+"';"

                    ,"if exists (select * from syscolumns where name='"+id+"' and id=object_id('"+biaobh+"'))"
                    ,"alter table "+biaobh+" drop column "+id
                });


                int result = SqlHelper.ExecuteSql(sqlDel);
                if (result > 0)
                {
                    return "删除成功";
                }
                else
                {
                    return "删除失败";
                }

            }
        }

        public string ClumnAdd(string mingcheng, string bianhao, string leibie, int changdu, string miaoshu, string biaobianhao, int shunxu, int tonghang, int kuan)
        {
            DataTable table = GetTableByCode(biaobianhao);
            if (table == null || table.Rows.Count <= 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = "没有找到表"
                });
            }
            ColumnModel clumn = new DbEntity().GetTableColumn(biaobianhao).Find(o => o.ccode == bianhao);
            if (clumn != null)
            {
                return JsonConvert.SerializeObject(new
                {
                    code = 1,
                    msg = "没有找到字段"
                });
            }

            kehenbar.web.Models.sys_database_clumn column = new Models.sys_database_clumn();
            column.ccode = bianhao;
            column.clength = changdu;
            column.cmark = miaoshu;
            column.cname = mingcheng;
            column.ctype = leibie;
            column.order = shunxu;
            column.kuan = kuan;
            column.tonghang = tonghang;
            column.sys_database_id = Convert.ToInt32(table.Rows[0]["id"]);
            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.table = "sys_database_clumn";
            fm.where = "";
            fm.field = JsonConvert.SerializeObject(column);

            string result = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            ResultModel rm = JsonConvert.DeserializeObject<ResultModel>(result);

            if (rm.code == 0)
            {
                string sql = string.Join(" ", new string[] { 
                    "alter table "+table.Rows[0]["tcode"],
                    "add ["+column.ccode+"] "+GetTableColumnType(column,column.ctype)
                });

                SqlHelper.ExecuteSql(sql);

                fm = new FormModel();
                fm.action = "select";
                fm.table = "sys_database_clumn";
                fm.where = "tcode=" + table.Rows[0]["tcode"] + ",ccode=" + column.ccode;
                fm.field = "";
                string jsonResult = new DbEntity().Index(JsonConvert.SerializeObject(fm));
                return jsonResult;
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

        public string TableSave(string biaoming, string biaobianhao, string biaoshuoming, int leibie)
        {
            if (string.IsNullOrEmpty(biaoming.Trim()) || string.IsNullOrEmpty(biaobianhao.Trim()))
            {
                return "";
            }

            DataTable table = SqlHelper.Query("select * from sys_database where tcode='" + biaobianhao + "'");
            if (table != null && table.Rows.Count > 0)
            {
                return "have";
            }

            string insertField = JsonConvert.SerializeObject(new { tname = biaoming, tcode = biaobianhao, ttype = leibie, tmark = biaoshuoming });

            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.field = insertField;
            fm.table = "sys_database";
            fm.where = "";
            string resultJson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            ResultModel rm = JsonConvert.DeserializeObject<ResultModel>(resultJson);

            if (rm.code == 0)
            {
                return "success";
            }
            else
            {
                return "error";
            }
        }

        public string ColumnShowAdd(ConfigKeyVal config)
        {
            if (config == null)
            {
                return "网络错误";
            }

            int yanshiid = Convert.ToInt32(config.yanshi);
            string sql = "select * from sys_columnshow where id=" + yanshiid;
            DataTable dtyanshi = SqlHelper.Query(sql);

            string yanshinr = dtyanshi.Rows[0]["content"] + ""; //html模板
            List<string> htmlTemp = new List<string>();

            if (yanshiid == 1 || yanshiid == 3)
            {
                yanshinr = yanshinr.Replace("@name", config.column);
                foreach (Keyval item in config.keyval)
                {
                    htmlTemp.Add(yanshinr.Replace("@title", item.zhi).Replace("@value", item.jian));
                }
            }
            else if (yanshiid == 2)
            {
                yanshinr = yanshinr.Replace("@name", config.column);
                htmlTemp.Add(yanshinr);
            }
            else if (yanshiid == 4)
            {
                string value = string.Empty;
                foreach (Keyval item in config.keyval)
                {
                    value += "<option value=\"" + item.jian + "\">" + item.zhi + "</option>";
                }

                yanshinr = yanshinr.Replace("@name", config.column).Replace("@value", value);
                htmlTemp.Add(yanshinr);
            }

            string sqlkeys = "[key],[val],[table],[column]"; //参数列表
            string sqlinsert = "delete from sys_columnshowVal where [table]='" + config.table + "' and [column]='" + config.column + "';";

            foreach (string item in htmlTemp)
            {
                string sqlvals = string.Join("','", new string[]{
                     config.colname
                    ,item
                    ,config.table
                    ,config.column
                });

                sqlinsert += "insert into sys_columnshowVal(" + sqlkeys + ")values('" + sqlvals + "');";
            }

            int result = SqlHelper.ExecuteSql(sqlinsert);
            if (result > 0)
            {
                string showtypeJson = JsonConvert.SerializeObject(config);
                FormModel fm = new FormModel();
                fm.action = "update";
                fm.field = JsonConvert.SerializeObject(new { showtype = showtypeJson });
                fm.table = "sys_database_clumn";
                fm.where = "tcode=" + config.table + ",ccode=" + config.column;
                string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
                ResultModel rm = JsonConvert.DeserializeObject<ResultModel>(resultjson);
                if (rm.code == 0)
                {
                    return "成功了";
                }
                else
                {
                    return "失败了";
                }
            }
            else
            {
                return "失败了";
            }
        }

        public string OutkeyDel(string tcode, string toutkey)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.table = "sys_database";
            fm.where = "tcode=" + tcode;
            fm.field = JsonConvert.SerializeObject(new
            {
                toutkey = toutkey
            });

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string OutkeyAdd(string tcode, string toutkey)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.table = "sys_database";
            fm.where = "tcode=" + tcode;
            fm.field = JsonConvert.SerializeObject(new
            {
                toutkey = toutkey
            });

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string TableCreate(string id)
        {
            id = SqlFunction.SqlFilter(id);
            DataTable table = SqlHelper.Query("select * from sys_database where tcode='" + id + "'");

            if (table == null || table.Rows.Count <= 0)
            {
                return "error";
            }

            List<kehenbar.web.Models.sys_database_clumn> columns = GetColumnsByTableName(id);
            List<string> sqlList = new List<string>();
            foreach (var item in columns)
            {
                string type = string.Empty;
                type = GetTableColumnType(item, type);
                sqlList.Add("[" + item.ccode + "] " + type);
            }
            string sqlColumns = string.Join(",", sqlList.ToArray());

            //导航表 自动加pid字段
            string p1 = string.Empty;
            if (Convert.ToInt32(table.Rows[0]["ttype"]) == 2)
            {
                p1 = "pid int not null,";
            }

            string sql = string.Join(" ", new string[]{
                "IF EXISTS  (SELECT  * FROM dbo.SysObjects WHERE ID = object_id(N'["+table.Rows[0]["tcode"]+"]') AND OBJECTPROPERTY(ID, 'IsTable') = 1) "
                ,"  drop table "+table.Rows[0]["tcode"]
                
                ,"create table "+table.Rows[0]["tcode"]
                ,"("
                ,"    id int identity(1,1) primary key not null,"
                ,     p1
                ,     sqlColumns
                ,")"
            });

            try
            {
                SqlHelper.ExecuteSql(sql, new SqlParameter[] { });
                return "success";
            }
            catch (Exception)
            {

                return "error";
            }
        }

        private static string GetTableColumnType(kehenbar.web.Models.sys_database_clumn item, string type)
        {
            switch (item.ctype)
            {
                case "edit":
                    type = "varchar(max)";
                    break;
                case "text":
                    type = "varchar(max)";
                    break;
                case "varchar":
                    type = "varchar(" + item.clength + ")";
                    break;
                case "int":
                    type = "int";
                    break;
                case "decimal":
                    type = "decimal(18,2)";
                    break;
                case "datetime":
                    type = "datetime";
                    break;
                case "image":
                    type = "varchar(500)";
                    break;
                default:
                    type = "varchar(100)";
                    break;
            }
            return type;
        }

        public void ColumnShow(string t, string c)
        {
            t = SqlFunction.SqlFilter(t);
            c = SqlFunction.SqlFilter(c);

            TempMain main = new TempMain("/admin/database/get_table_column_show.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                FormModel fm = new FormModel();
                fm.action = "select";
                fm.field = "";
                fm.table = "sys_database_clumn";
                fm.where = "tcode=" + t + ",ccode=" + c;
                string result = new DbEntity().Index(JsonConvert.SerializeObject(fm));
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(result);
                string showtype = string.Empty;
                if (dt != null && dt.Rows.Count > 0)
                {
                    showtype = dt.Rows[0]["sys_database_clumnshowtype"] + "";
                }

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_tcode]", t)
                    .Replace("[myparm_ccode]", c)
                    .Replace("[myparm_showtype]", showtype);
                #endregion
            };

            main.ParseHtml();
        }

        public string Image(string erjimulu, string biaoming)
        {
            biaoming = SqlFunction.SqlFilter(biaoming);
            kehenbar.web.Models.Upload ul = new Models.Upload();
            string forder = erjimulu; //二级目录
            forder = string.IsNullOrEmpty(forder) ? "/uploads/imgs/" : "/uploads/imgs/" + forder + "/";

            DataTable table = SqlHelper.Query("select * from sys_database where tcode='" + biaoming + "'");
            if (table == null || table.Rows.Count <= 0)
            {
                return "";
            }
            List<kehenbar.web.Models.sys_database_clumn> columns = GetColumnsByTableName(biaoming);
            List<string> parmList = new List<string>();
            string labelID = string.Empty;
            foreach (var item in columns)
            {
                if (item.ctype == "image")
                {
                    parmList.Add("file_" + item.ccode);
                }
            }

            HttpPostedFileBase file = null;
            foreach (var item in parmList)
            {
                file = Request.Files[item];

                labelID = item;
                if (file != null)
                {
                    break;
                }
            }

            string ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            string newName = Guid.NewGuid().ToString();
            string filePath = Server.MapPath(forder);
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            try
            {
                file.SaveAs(filePath + newName + ext);
                ul.code = 0;
                ul.data.src = forder + newName + ext;
                ul.data.title = labelID;
            }
            catch (Exception ex)
            {
                ul.code = 1;
                ul.msg = ex.Message;
                ul.data = null;
            }

            string json = JsonConvert.SerializeObject(ul);
            return json;
        }

        public void GetTableForm(string id)
        {
            id = string.IsNullOrEmpty(id) ? id : SqlFunction.SqlFilter(id);

            DataTable dtcolsVal = SqlHelper.Query("select * from sys_columnshowVal where [table] = '" + id + "'");
            DataTable table = GetTableInfoByTcode(id);

            TempMain main = new TempMain("/admin/database/get_table_form.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string editFieldId = string.Empty;
                string tableFormDaoHangHtml = GetTableFormDaoHangHtml(null, table, dtcolsVal, out editFieldId);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_tablename]", table.Rows[0]["sys_databasetcode"] + "")
                    .Replace("[myparm_formDaohangHtml]", tableFormDaoHangHtml)
                    .Replace("[myparm_editClumnName]", editFieldId);
                #endregion
            };

            main.ParseHtml();
        }

        public void GetTableFormLayout(string id)
        {
            id = string.IsNullOrEmpty(id) ? id : SqlFunction.SqlFilter(id);

            DataTable dtcolsVal = SqlHelper.Query("select * from sys_columnshowVal where [table] = '" + id + "'");
            DataTable table = GetTableInfoByTcode(id);

            TempMain main = new TempMain("/admin/database/get_table_form_layout.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数

                string editFieldId = string.Empty;
                string tableFormDaoHangHtml = GetTableFormDaoHangHtml(null, table, dtcolsVal, out editFieldId);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_tablename]", table.Rows[0]["sys_databasetcode"] + "")
                    .Replace("[myparm_formDaohangHtml]", tableFormDaoHangHtml)
                    .Replace("[topMenuId]", getTopMenuId())
                    .Replace("[myparm_editClumnName]", editFieldId);
                #endregion
            };

            main.ParseHtml();
        }

        public void GetTableFormDaoHang(string t, string tablename, string id)
        {
            tablename = SqlFunction.SqlFilter(tablename);

            id = string.IsNullOrEmpty(id) ? id : SqlFunction.SqlFilter(id);
            string pid = "";
            DataTable table = GetTableInfoByTcode(tablename);

            DataTable dtcolsVal = SqlHelper.Query("select * from sys_columnshowVal where [table] = '" + tablename + "'");

            if (t.Equals("zj"))
            {

                //同级菜单
                pid = id;
            }
            else if (t.Equals("tj") && id != null)
            {
                //子级菜单
                string sql = "select pid from " + tablename + " where id=" + id;
                pid = SqlHelper.Query(sql).Rows[0][0] + "";
            }

            if (id == null) { pid = "0"; }

            TempMain main = new TempMain("/admin/database/get_table_form_daohang.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string editFieldId = string.Empty;
                string tableFormDaoHangHtml = GetTableFormDaoHangHtml(null, table, dtcolsVal, out editFieldId);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_pid]", pid)
                    .Replace("[myparm_tablename]", table.Rows[0]["sys_databasetcode"] + "")
                    .Replace("[myparm_formDaohangHtml]", tableFormDaoHangHtml)
                    .Replace("[myparm_editClumnName]", editFieldId);
                #endregion
            };

            main.ParseHtml();
        }

        private DataTable GetTableInfoByTcode(string tablename)
        {
            tablename = SqlFunction.SqlFilter(tablename);
            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.table = "sys_database_clumn";
            fm.where = "tcode=" + tablename;
            fm.order = "order";

            string resultJson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            DataTable result = JsonConvert.DeserializeObject<DataTable>(resultJson);
            return result;
        }

        public string GetTableFormDaoHangHtml(DataTable source, DataTable dt, DataTable dtcolsVal, out string editFieldId)
        {
            string resultHtml = string.Empty;
            string tableoutkeys = dt.Rows[0]["sys_databasetoutkey"] + ""; //外键            

            editFieldId = "empty";
            int rowcount = 0;
            foreach (DataRow item in dt.Rows)
            {
                #region 保单数据行头部
                string istonghang = item["sys_database_clumntonghang"] + "";
                string columnwidth = item["sys_database_clumnkuan"] + ""; //宽
                columnwidth = string.IsNullOrEmpty(columnwidth) ? "200px" : columnwidth + "px";
                rowcount++;
                if (istonghang == "1")
                {
                    resultHtml += "<div class=\"layui-inline\">";
                }
                else
                {
                    if (rowcount == 1) { 
                        resultHtml += "<div class=\"layui-form-item\">";
                        resultHtml += "<div class=\"layui-inline\">";
                    }
                    else
                    {
                        resultHtml += "</div><div class=\"layui-form-item\">";
                        resultHtml += "<div class=\"layui-inline\">";
                    }
                }
                #endregion

                #region 生成表单数据行
                //自定义显示方式
                if (!string.IsNullOrEmpty(item["sys_database_clumnshowtype"] + ""))
                {
                    resultHtml += "<label class=\"layui-form-label\">" + item["sys_database_clumncname"] + "</label>";
                    resultHtml += "<div class=\"layui-input-block\" style=\"width:" + columnwidth + "\">";
                    foreach (DataRow row in dtcolsVal.Rows)
                    {
                        if (row["column"].ToString().Equals(item["sys_database_clumnccode"] + ""))
                        {
                            resultHtml += row["val"];
                            string selectedVal = "0";
                            string columncode = item["sys_database_clumnccode"] + "";
                            if (source != null && source.Rows.Count > 0)
                            {
                                selectedVal = source.Rows[0][columncode] + "";
                            }
                            resultHtml += "<input type='hidden' name='__selectedVal' value='" + selectedVal + "'>";
                        }
                    }
                    resultHtml +=    "</div>";
                    resultHtml += "</div>";

                    continue;
                }

                //外键表显示方式
                if (!string.IsNullOrEmpty(tableoutkeys))
                {
                    bool have = false;
                    string tableoutkey_column = item["sys_database_clumnccode"] + "";
                    foreach (var toutkey in tableoutkeys.Split(','))
                    {
                        string _toutkey = toutkey.ToLower().Trim() + "_id";
                        string _tableoutkey_column = tableoutkey_column.ToLower().Trim();
                        if (_toutkey.Equals(_tableoutkey_column))
                        {
                            have = true;
                            string _resultHtml = "";

                            Dictionary<string, string> outkeyvalues = new DbFunction().GetOutKeyValues(toutkey);
                            
                            _resultHtml += "<label class=\"layui-form-label\">" + item["sys_database_clumncname"] + "</label>";
                            _resultHtml += "<div class=\"layui-input-block\"  style=\"width:" + columnwidth + "\">";
                            _resultHtml +=   "<select name=\"" + item["sys_database_clumnccode"] + "\">";
                            foreach (KeyValuePair<string,string> vk in outkeyvalues)
                            {                                
                                {
                                    _resultHtml += "<option value=\"" + vk.Key + "\">" + vk.Value + "</option>";
                                }
                            }
                            _resultHtml +=   "</select>";

                            string selectedVal = "0";
                            string columncode = item["sys_database_clumnccode"] + "";
                            if (source != null && source.Rows.Count > 0)
                            {
                                selectedVal = source.Rows[0][columncode] + "";
                            }
                            else
                            {
                                if (_tableoutkey_column == "users_id")
                                {
                                    selectedVal = Session["UserId"] + "";
                                }
                            }
                            _resultHtml += "<input type='hidden' name='__selectedVal' value='" + selectedVal + "'>";
                            _resultHtml += "</div>";                            
                            resultHtml += _resultHtml;                            
                        }
                    }
                    if (have) {

                        resultHtml += "</div>";
                        continue; 
                    }
                }

                //其他字段显示方式
                if (item["sys_database_clumnctype"] + "" == "varchar")
                {
                    resultHtml += string.Join(" ", new string[]{

                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-block\"  style=\"width:" + columnwidth + "\">"
                            ,"<input type=\"text\" name=\""+item["sys_database_clumnccode"]+"\" class=\"layui-input\" value=\"#value#\">"
                        ,"</div>"

                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                else if (item["sys_database_clumnctype"] + "" == "text")
                {
                    resultHtml += string.Join(" ", new string[]{

                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-block\" style=\"width:" + columnwidth + "\">"
                            ,"<textarea name=\""+item["sys_database_clumnccode"]+"\" placeholder=\"请输入内容\" class=\"layui-textarea\">#value#</textarea>"
                        ,"</div>"
                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                else if (item["sys_database_clumnctype"] + "" == "edit")
                {
                    editFieldId = item["sys_database_clumnccode"] + "";
                    resultHtml += string.Join(" ", new string[]{
                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-block\" style=\"width:" + columnwidth + "\">"
                            ,"<!-- 加载编辑器的容器 -->"
                            ,"<textarea id=\"container\" name=\""+item["sys_database_clumnccode"]+"\" type=\"text/plain\" >#value#"
                            ,"</textarea>"
                        ,"</div>"
                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                else if (item["sys_database_clumnctype"] + "" == "int")
                {
                    resultHtml += string.Join(" ", new string[]{
                        
                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-block\" style=\"width:" + columnwidth + "\">"
                            ,"<input type=\"number\" lay-verfiy=\"number\" name=\""+item["sys_database_clumnccode"]+"\" class=\"layui-input\" value=\"#value#\">"
                        ,"</div>"

                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                else if (item["sys_database_clumnctype"] + "" == "decimal")
                {
                    resultHtml += string.Join(" ", new string[]{
                        
                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-block\" style=\"width:" + columnwidth + "\">"
                            ,"<input type=\"number\" lay-verfiy=\"number\" name=\""+item["sys_database_clumnccode"]+"\" class=\"layui-input\" value=\"#value#\">"
                        ,"</div>"

                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                else if (item["sys_database_clumnctype"] + "" == "datetime")
                {
                    resultHtml += string.Join(" ", new string[]{
                        
                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-block\" style=\"width:" + columnwidth + "\">"
                            ,"<input type=\"text\" name=\""+item["sys_database_clumnccode"]+"\" class=\"layui-input\" onclick=\"layui.laydate({ elem: this, istime: true, format: 'YYYY-MM-DD hh:mm:ss' })\" value=\"#value#\">"
                        ,"</div>"

                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                else if (item["sys_database_clumnctype"] + "" == "image")
                {
                    resultHtml += string.Join(" ", new string[]{
                        
                         "<label class=\"layui-form-label\">"+item["sys_database_clumncname"]+"</label>"
                        ,"<div class=\"layui-input-inline\">"
                            ,"<input type=\"file\" name=\"file_"+item["sys_database_clumnccode"]+"\" class=\"layui-upload-file\">"
                            ,"<input type=\"hidden\" name=\""+item["sys_database_clumnccode"]+"\" id=\"fileid_"+item["sys_database_clumnccode"]+"\" value=\"#value#\"/>"
                        ,"</div>"
                        ,"<div class=\"layui-form-mid layui-word-aux\">"
                            ,"<img src=\"#value#\" style=\" height: 50px; position: absolute; top: -5px;\" id=\"tip_"+item["sys_database_clumnccode"]+"\" />"
                        ,"</div>"

                    });
                    if (source != null && source.Rows.Count > 0)
                    {
                        resultHtml = resultHtml.Replace("#value#", source.Rows[0][item["sys_database_clumnccode"] + ""] + "");
                    }
                    else
                    {
                        resultHtml = resultHtml.Replace("#value#", "");
                    }
                }
                #endregion

                #region 表单数据行尾部
                if (istonghang=="1"){
                    resultHtml += "</div>";
                    if (rowcount == dt.Rows.Count)
                    {
                        resultHtml += "</div>";
                    }
                }
                else
                {
                    resultHtml += "</div>";
                }
                #endregion
            }
            return resultHtml;
        }

        [ValidateInput(false)]
        public string TableInsert()
        {
            string tablename = SqlFunction.SqlFilter(Request["tablename"] + "");
            if (string.IsNullOrEmpty(tablename))
            {
                return "网络错误";
            }
            DataTable table = GetTableInfoByTcode(tablename);
            List<string> sqlkeylist = new List<string>();
            List<string> sqlvallist = new List<string>();
            foreach (DataRow item in table.Rows)
            {
                string itemcode = item["sys_database_clumnccode"] + "";
                sqlkeylist.Add(itemcode);
                sqlvallist.Add("'" + SqlFunction.SqlFilter(Request[itemcode]) + "'");
            }

            /*-----------------------
             *table type =2 的 情况
             */
            string pid = Request["pid"];
            if (!string.IsNullOrEmpty(pid))
            {
                pid = SqlFunction.SqlFilter(pid);
                sqlkeylist.Add("pid");
                sqlvallist.Add(pid);
            }
            //-----------------------
            string sqlkey = string.Join("],[", sqlkeylist.ToArray());
            string sqlval = string.Join(",", sqlvallist.ToArray());
            string sql = "insert into " + tablename + "([" + sqlkey + "]) values(" + sqlval + ")";
            try
            {
                SqlHelper.ExecuteSql(sql, new SqlParameter[] { });
                return "保存成功";
            }
            catch (Exception)
            {
                return "网络错误";
            }
        }

        [ValidateInput(false)]
        public string TableEditSave()
        {
            string tablename = SqlFunction.SqlFilter(Request["tablename"] + "");
            string rowId = SqlFunction.SqlFilter(Request["rowId"] + "");
            if (string.IsNullOrEmpty(tablename))
            {
                return "网络错误";
            }
            List<kehenbar.web.Models.sys_database_clumn> columnList = GetColumnsByTableName(tablename);
            List<string> sqlkeylist = new List<string>();

            foreach (var item in columnList)
            {
                string setparm = "[" + item.ccode + "]='" + Request[item.ccode] + "'";

                sqlkeylist.Add(setparm);
            }

            string sqlkey = string.Join(",", sqlkeylist.ToArray());
            string sql = "update " + tablename + " set " + sqlkey + " where id=" + rowId;
            try
            {
                SqlHelper.ExecuteSql(sql, new SqlParameter[] { });
                return "保存成功";
            }
            catch (Exception)
            {
                return "网络错误";
            }
        }

        public void DataList(string id, int page, int pagesize)
        {
            id = SqlFunction.SqlFilter(id);

            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.table = "sys_database";
            fm.where = "tcode=" + id;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            DataTable table = JsonConvert.DeserializeObject<DataTable>(resultjson);
            if (table != null && table.Rows.Count > 0)
            {
                int tableType = Convert.ToInt32(table.Rows[0]["sys_databasettype"]);
                if (tableType == 1 || tableType == 3)
                {
                    //数据表
                    PuTongList(id, page, pagesize);
                }
                else
                {
                    //导航表
                    DaoHangList(id);
                }
            }
        }

        private void DaoHangList(string id)
        {
            string sqlcolumns = string.Join(" ", new string[]{
                 "select ccode,ctype,cname"
                ,"from sys_database_clumn c"
                ,"inner join sys_database t on c.sys_database_id = t.id"
                ,"where t.tcode = @tcode"
            });
            SqlParameter[] paramCol = { new SqlParameter("@tcode", id) };
            DataTable dtColumns = SqlHelper.Query(sqlcolumns, paramCol).Tables[0];
            List<string> sqlkeys = new List<string>();
            List<string> colValues = new List<string>();
            foreach (DataRow item in dtColumns.Rows)
            {
                string itemCtype = (item[1] + "").Trim().ToLower();
                if (!itemCtype.Equals("int") && !itemCtype.Equals("varchar")) { continue; }

                sqlkeys.Add(item[0] + "");
                colValues.Add(item[2] + "");
            }
            sqlkeys.Add("pid");
            colValues.Add("父级ID");

            string sqlkey = string.Join("],[", sqlkeys.ToArray());
            string sql = string.Join(" ", new string[] { 
                 "select id,[" + sqlkey +"] "
                ,"from " + id                
                ,"order by id desc"
            });


            FormModel fm = new FormModel();
            fm.action = "select";
            fm.field = "";
            fm.table = "sys_database_clumn";
            fm.where = "tcode=" + id;
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            DataTable table = JsonConvert.DeserializeObject<DataTable>(resultjson);

            List<ConfigKeyVal> configList = new List<ConfigKeyVal>();
            foreach (DataRow item in table.Rows)
            {
                if (string.IsNullOrEmpty(item["sys_database_clumnshowtype"] + ""))
                {
                    continue;
                }
                configList.Add(JsonConvert.DeserializeObject<ConfigKeyVal>(item["sys_database_clumnshowtype"] + ""));
            }

            DataTable dt = SqlHelper.Query(sql);

            TempMain main = new TempMain("/admin/database/list_daohang.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                //列头
                string tableTrHeader = "<th>" + string.Join("</th><th>", colValues.ToArray()) + "</th>";
                string tableTrBody = GetDaoHangListHtml(dt, configList, sqlkeys, id, 1, 0);

                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_tableName]", id)
                    .Replace("[myparm_tableTrHeader]", tableTrHeader)
                    .Replace("[myparm_tableTrBody]", tableTrBody)
                    .Replace("[topMenuId]", getTopMenuId());

                #endregion
            };

            main.ParseHtml();
        }

        public string ColumnEdit(string biaobianhao, string bianhao, int paixu, int tonghang,int kuan)
        {
            FormModel fm = new FormModel();
            fm.action = "update";
            fm.table = "sys_database_clumn";
            fm.where = "tcode=" + biaobianhao + ",ccode=" + bianhao;
            fm.field = JsonConvert.SerializeObject(new
            {
                order = paixu,
                tonghang = tonghang,
                kuan = kuan
            });
            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        public string DataDel(string bm, int id)
        {
            FormModel fm = new FormModel();
            fm.action = "delete";
            fm.field = "";
            fm.table = bm;
            fm.where = bm+".id=" + id;

            string resultjson = new DbEntity().Index(JsonConvert.SerializeObject(fm));
            return resultjson;
        }

        private void PuTongList(string id, int page, int pagesize)
        {
            #region 生成查询条件
            List<string> sqlwhereList = new List<string>();
            DataTable table = SqlHelper.Query(string.Join(" ", new string[]{
                "select tcode,toutkey ",
                "from dbo.sys_database",
                "where tcode=@tcode"
            }), new SqlParameter("@tcode",id)).Tables[0];
            if(table != null && table.Rows.Count > 0){
                string toutkeys = table.Rows[0]["toutkey"] + "";
                List<string> toutkeylist = toutkeys.Split(',').ToList();
                foreach (var item in toutkeylist)
                {
                    string urlparm = Request.QueryString[item] + "";
                    if (!string.IsNullOrEmpty(urlparm))
                    {
                        sqlwhereList.Add(item + "_id=" + urlparm);
                    }
                }
            }
            string sqlwherestr = string.Empty;
            if (sqlwhereList.Count > 0) {
                sqlwherestr = string.Join(" and ", sqlwhereList.ToArray()) +" and ";
            }
            #endregion

            #region 拼接字段
            string sqlcolumns = string.Join(" ", new string[]{
                 "select ccode,ctype,cname,showlb,orderlb,columnlink"
                ,"from sys_database_clumn c"
                ,"inner join sys_database t on c.sys_database_id = t.id"
                ,"where t.tcode = @tcode"
            });
            SqlParameter[] paramCol = { new SqlParameter("@tcode", id) };
            DataTable dtColumns = SqlHelper.Query(sqlcolumns, paramCol).Tables[0];
            List<string> sqlkeys = new List<string>();
            List<string> colValues = new List<string>();
            List<string> columnlinkList = new List<string>();
            #endregion

            #region 处理字段
            DataView dtColumnsDV = dtColumns.DefaultView;
            dtColumnsDV.Sort = "orderlb asc";
            dtColumns = dtColumnsDV.ToTable();

            columnlinkList.Add("");//数据列比这里的字段多一列ID，所以为了列对应，这里加一个空。
            foreach (DataRow item in dtColumns.Rows)
            {
                if (item["showlb"]+"" == "1")
                {
                    sqlkeys.Add(item[0] + "");
                    colValues.Add(item[2] + "");
                    columnlinkList.Add(item["columnlink"] + "");

                    if (sqlkeys.Count > 6) { break; } //列表显示6个字段就可以了，多了显示不开。            
                }
            }
            #endregion

            #region 加载数据
            string sqlkey = string.Join("],[", sqlkeys.ToArray());
            string sql = string.Join(" ", new string[] { 
                 "select top " + pagesize + " id,[" + sqlkey +"]"
                ,"from " + id
                ,"where "+sqlwherestr+" id not in "
                ,"(select top " + (page - 1) * pagesize + " id "
                    ,"from " + id
                    ,"where "+sqlwherestr +" 1=1 "
                    ,"order by id desc) "
                ,"order by id desc"
            });
            #endregion

            #region 分页
            int allCount = Convert.ToInt32(SqlHelper.Query("select count(1) from " + id).Rows[0][0]);
            int allPage = allCount % pagesize == 0 ? allCount / pagesize : allCount / pagesize + 1;

            string pagebar = string.Empty;
            int pageprev = 1;
            int pagenext = 1;
            pageprev = page - 1 <= 0 ? page : page - 1;
            pagenext = page + 1 > allPage ? allPage : page + 1;

            pagebar += "<div class='pagebar'>";
            pagebar += "<span><a href='/database/datalist/" + id + "/" + pagesize + "/" + pageprev + "'>上一页</a></span>";
            for (int i = (page-3); i < allPage; i++)
            {
                if (i < 0) { i = 0; }
                if (i > ((page +3))) { break; }
                string nowpage = string.Empty;
                if ((i + 1) == page)
                {
                    pagebar += "<span><a style='background-color:#009688;color:#fff' href='/database/datalist/" + id + "/" + pagesize + "/" + (i + 1) + "'>" + (i + 1) + "</a></span>";
                }
                else
                {
                    pagebar += "<span><a href='/database/datalist/" + id + "/" + pagesize + "/" + (i + 1) + "'>" + (i + 1) + "</a></span>";
                }
            }
            pagebar += "<span><a href='/database/datalist/" + id + "/" + pagesize + "/" + pagenext + "'>下一页</a></span>";
            pagebar += "</div>";
            #endregion

            #region 解析模版
            DataTable dt = SqlHelper.Query(sql);
            dt = new DbFunction().DeserializeColumnVal(id, dt, columnlinkList);
            TempMain main = new TempMain("/admin/database/list.html");
            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                string tableTrHeader = "<th>" + string.Join("</th><th>", colValues.ToArray()) + "</th>";

                TempMain tm = (TempMain)sender;
                tm.content = tm.content.Replace("[myparm_tableName]", id)
                    .Replace("[myparm_columnVals]", tableTrHeader)
                    .Replace("[myparm_dt]", JsonConvert.SerializeObject(dt))
                    .Replace("[myparm_pagebar]", pagebar)
                    .Replace("[myparm_rowIndex]", pagesize * (page - 1) + "")
                    .Replace("[topMenuId]", getTopMenuId());
            };
            main.ParseHtml();
            #endregion
        }

        public string GetDaoHangListHtml(DataTable dataTable, List<ConfigKeyVal> configlist, List<string> columnKeys, string tableName, int leval, int pid)
        {

            string resultHtml = string.Empty;
            foreach (DataRow row in dataTable.Select("leval=" + leval + " and pid=" + pid))
            {
                resultHtml += "<tr>";

                #region 生成列
                int columncount = 0;
                foreach (var item in columnKeys)
                {
                    columncount++;
                    if (configlist.Count == 0)
                    {
                        string rowitem = row[item] + "";
                        resultHtml += "<td title=\"" + rowitem + "\">";
                        if (columncount == 1)
                        {
                            for (int i = 1; i < leval; i++)
                            {
                                resultHtml += "<img src='/imgs/dh/02.gif' style='position: relative;top: -5px;'/><img src='/imgs/dh/kong.gif'/>";
                            }
                        }
                        resultHtml += (rowitem.Length > 10 ? rowitem.Substring(0, 10) + ".." : rowitem) + "</td>  ";
                    }
                    else
                    {
                        bool ishave = false;
                        foreach (var item1 in configlist)
                        {
                            if (item1.column == item)
                            {
                                string val = row[item] + "";
                                foreach (var item2 in item1.keyval)
                                {
                                    if (item2.jian == val)
                                    {
                                        string item2zhi = item2.zhi.Length > 10 ? item2.zhi.Substring(0, 10) + ".." : item2.zhi;
                                        resultHtml += "<td title=\"" + item2.zhi + "\">" + item2zhi + "</td>";
                                        break;
                                    }
                                }
                                ishave = true;
                                break;
                            }
                        }
                        if (!ishave)
                        {
                            string rowitem = row[item] + "";
                            resultHtml += "<td title=\"" + rowitem + "\">";
                            if (columncount == 1)
                            {
                                for (int i = 1; i < leval; i++)
                                {
                                    resultHtml += "<img src='/imgs/dh/02.gif' style='position: relative;top: -5px;'/><img src='/imgs/dh/kong.gif'/>";
                                }
                            }
                            resultHtml += (rowitem.Length > 10 ? rowitem.Substring(0, 10) + ".." : rowitem) + "</td>  ";
                        }
                    }
                }
                #endregion

                #region 生成按钮
                resultHtml += string.Join("", new string[]{
                        
                        "   <td>"
                        ,"    <input type='hidden' name='biaoming' value='"+tableName+"'/>"                   
                        ,"    <button class='layui-btn layui-btn-mini' lay-click lay-filter='addChildmenu' data='"+row["id"]+"'>添加子菜单</button>"
                        ,"    <button class='layui-btn layui-btn-mini' lay-click lay-filter='edit' data='"+row["id"]+"'>编辑</button>"
                        ,"    <button class='layui-btn layui-btn-mini layui-btn-danger' lay-click lay-filter='del' data='"+row["id"]+"'>删除</button>"
                        ,"  </td>"
                        ,"</tr>"
                    });
                #endregion

                leval = leval + 1;
                pid = Convert.ToInt32(row["id"]);

                int rowcount = dataTable.Select("leval=" + leval + " and pid=" + pid).Length;
                if (rowcount <= 0)
                {
                    leval = leval - 1;
                    pid = Convert.ToInt32(row["pid"]);
                    continue;
                }
                resultHtml += GetDaoHangListHtml(dataTable, configlist, columnKeys, tableName, leval, pid);
                leval = leval - 1;
                pid = Convert.ToInt32(row["pid"]);
            }

            return resultHtml;
        }

        public void DataEdit(int id, string bm)
        {
            string tablename = SqlFunction.SqlFilter(bm);
            string columns = GetColumns(tablename).Replace(",", "],[");

            string sql = "select id,[" + columns + "] from " + tablename + " where id=" + id;
            DataTable dt = SqlHelper.Query(sql);
            DataTable dtcolsVal = SqlHelper.Query("select * from sys_columnshowVal where [table] = '" + bm + "'");

            List<kehenbar.web.Models.sys_database_clumn> columnList = GetColumnsByTableName(tablename);

            DataTable table = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(columnList));

            TempMain main = new TempMain("/admin/database/edit_table_form.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                string editFieldId = string.Empty;
                string tableFormDaoHangHtml = GetTableFormDaoHangHtml(dt, table, dtcolsVal, out editFieldId);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_id]", id + "")
                    .Replace("[myparm_tablename]", table.Rows[0]["sys_databasetcode"] + "")
                    .Replace("[myparm_formDaohangHtml]", tableFormDaoHangHtml)
                    .Replace("[myparm_editClumnName]", editFieldId)
                    .Replace("[topMenuId]", getTopMenuId());
            };

            main.ParseHtml();
        }

        public void DataShow(int id, string bm)
        {
            string tablename = SqlFunction.SqlFilter(bm);
            string columns = GetColumns(tablename).Replace(",", "],[");

            string sql = "select id,[" + columns + "] from " + tablename + " where id=" + id;
            DataTable dt = SqlHelper.Query(sql);
            DataTable dtcolsVal = SqlHelper.Query("select * from sys_columnshowVal where [table] = '" + bm + "'");

            List<kehenbar.web.Models.sys_database_clumn> columnList = GetColumnsByTableName(tablename);

            DataTable table = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(columnList));

            TempMain main = new TempMain("/admin/database/show_table_form.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                string editFieldId = string.Empty;
                string tableFormDaoHangHtml = GetTableFormDaoHangHtml(dt, table, dtcolsVal, out editFieldId);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_id]", id + "")
                    .Replace("[myparm_tablename]", table.Rows[0]["sys_databasetcode"] + "")
                    .Replace("[myparm_formDaohangHtml]", tableFormDaoHangHtml)
                    .Replace("[myparm_editClumnName]", editFieldId)
                    .Replace("[topMenuId]", getTopMenuId());
            };

            main.ParseHtml();
        }

        public void dataDaoHangEdit(int id, string bm)
        {
            string tablename = SqlFunction.SqlFilter(bm);
            string columns = GetColumns(tablename).Replace(",", "],[");

            string sql = "select id,pid,[" + columns + "] from " + tablename + " where id=" + id;
            DataTable dt = SqlHelper.Query(sql);
            DataTable dtcolsVal = SqlHelper.Query("select * from sys_columnshowVal where [table] = '" + bm + "'");
            List<kehenbar.web.Models.sys_database_clumn> columnList = new List<kehenbar.web.Models.sys_database_clumn>();
            kehenbar.web.Models.sys_database_clumn c = new kehenbar.web.Models.sys_database_clumn();
            c.ccode = "pid";
            c.cname = "上级ID";
            c.ctype = "int";
            c.showtype = "";
            c.order = 1;
            columnList.Add(c);
            columnList.AddRange(GetColumnsByTableName(tablename));

            DataTable table = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(columnList));

            TempMain main = new TempMain("/admin/database/edit_table_form_daohang.html");

            main.TempFunc += delegate(Object sender, TempMain.TempFuncEventArgs e)
            {
                #region 解析自定义函数
                string editFieldId = string.Empty;
                string tableFormDaoHangHtml = GetTableFormDaoHangHtml(dt, table, dtcolsVal, out editFieldId);
                TempMain tm = (TempMain)sender;
                tm.content = tm.content
                    .Replace("[myparm_id]", id + "")
                    .Replace("[myparm_tablename]", table.Rows[table.Rows.Count - 1]["sys_databasetcode"] + "")
                    .Replace("[myparm_formDaohangHtml]", tableFormDaoHangHtml)
                    .Replace("[myparm_editClumnName]", editFieldId)
                    .Replace("[topMenuId]", getTopMenuId());

                #endregion
            };

            main.ParseHtml();
        }

        public string dataDaoHangDel(int id, string bm)
        {
            bm = SqlFunction.SqlFilter(bm);

            DataTable source = SqlHelper.Query("select id,pid from " + bm);
            List<int> idlist = GetIdListByPid(id, bm, source);
            idlist.Add(id);
            string sql = "delete from " + bm + " where id in(" + string.Join(",", idlist.ToArray()) + ")";
            int result = SqlHelper.ExecuteSql(sql);
            if (result > 0)
            {
                return "删除成功";
            }
            else
            {
                return "删除失败";
            }
        }

        private List<int> GetIdListByPid(int id, string bm, DataTable source)
        {
            List<int> ids = new List<int>();
            DataRow[] rows = source.Select("pid=" + id);
            foreach (DataRow item in rows)
            {
                int _id = Convert.ToInt32(item["id"] + "");
                ids.Add(_id);

                DataRow[] rows1 = source.Select("pid=" + _id);
                if (rows1.Length > 0)
                {
                    ids.AddRange(GetIdListByPid(_id, bm, source));
                }
            }

            return ids;
        }

        public string GetColumns(string tname)
        {
            List<kehenbar.web.Models.sys_database_clumn> columns = GetColumnsByTableName(tname);
            List<string> columnList = new List<string>();
            foreach (var item in columns)
            {
                columnList.Add(item.ccode);
            }
            return string.Join(",", columnList.ToArray());
        }

        public List<kehenbar.web.Models.sys_database_clumn> GetColumnsByTableName(string tname)
        {
            DataTable table = GetTableInfoByTcode(tname);

            List<kehenbar.web.Models.sys_database_clumn> columns = new List<Models.sys_database_clumn>();
            foreach (DataRow item in table.Rows)
            {
                kehenbar.web.Models.sys_database_clumn column = new Models.sys_database_clumn();
                column.id = Convert.ToInt32(item["sys_database_clumnid"] + "");
                column.sys_database_id = Convert.ToInt32(item["sys_database_clumnsys_database_id"] + "");
                column.cname = item["sys_database_clumncname"] + "";
                column.ccode = item["sys_database_clumnccode"] + "";
                column.clength = Convert.ToInt32(item["sys_database_clumnclength"] + "");
                column.cmark = item["sys_database_clumncmark"] + "";
                column.ctype = item["sys_database_clumnctype"] + "";
                column.order = Convert.ToInt32(item["sys_database_clumnorder"] + "");
                column.show = Convert.ToInt32(item["sys_database_clumnshow"] + "");
                column.showtype = item["sys_database_clumnshowtype"] + "";

                column.sys_database_clumnid = Convert.ToInt32(item["sys_database_clumnid"] + "");
                column.sys_database_clumnsys_database_id = Convert.ToInt32(item["sys_database_clumnsys_database_id"] + "");
                column.sys_database_clumncname = item["sys_database_clumncname"] + "";
                column.sys_database_clumnccode = item["sys_database_clumnccode"] + "";
                column.sys_database_clumnclength = Convert.ToInt32(item["sys_database_clumnclength"] + "");
                column.sys_database_clumncmark = item["sys_database_clumncmark"] + "";
                column.sys_database_clumnctype = item["sys_database_clumnctype"] + "";
                column.sys_database_clumnorder = Convert.ToInt32(item["sys_database_clumnorder"] + "");
                column.sys_database_clumnshow = Convert.ToInt32(item["sys_database_clumnshow"] + "");
                column.sys_database_clumnshowtype = item["sys_database_clumnshowtype"] + "";
                column.sys_database_clumntonghang = item["sys_database_clumntonghang"] + "";
                column.sys_database_clumnkuan = item["sys_database_clumnkuan"] + "";
                column.sys_databaseid = Convert.ToInt32(item["sys_databaseid"] + "");
                column.sys_databasetcode = item["sys_databasetcode"] + "";
                column.sys_databasettype = Convert.ToInt32(item["sys_databasettype"] + "");
                column.sys_databasetname = item["sys_databasetname"] + "";
                column.sys_databasetoutkey = item["sys_databasetoutkey"] + "";
                columns.Add(column);
            }
            return columns;
        }

        public string getTopMenuId()
        {
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
            return id;
        }
    }
}