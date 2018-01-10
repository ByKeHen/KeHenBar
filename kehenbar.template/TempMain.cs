using kehenbar.DataBase;
using kehenbar.model;
using kehenbar.Template.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kehenbar.Template
{
    public class TempMain
    {
        #region 注册事件
        public delegate void TempFuncEventHandler(Object sender, TempFuncEventArgs e);
        public delegate void BeforeFuncEventHandler(Object sender, BeforeFuncEventArgs e);
        public delegate void EndFuncEventHandler(Object sender, EndFuncEventArgs e);
        public event TempFuncEventHandler TempFunc;
        public event BeforeFuncEventHandler BeforeFunc;
        public event EndFuncEventHandler EndFunc;

        public class TempFuncEventArgs : EventArgs
        {
            public readonly string content;
            public TempFuncEventArgs(string content){
                this.content = content;
            }
        }
        public class BeforeFuncEventArgs : EventArgs
        {
            public readonly string content;
            public BeforeFuncEventArgs(string content)
            {
                this.content = content;
            }
        }
        public class EndFuncEventArgs : EventArgs
        {
            public readonly string content;
            public EndFuncEventArgs(string content)
            {
                this.content = content;
            }
        }

        protected virtual void OnTempFunc(TempFuncEventArgs e)
        {
            if (TempFunc != null)
            {
                TempFunc(this, e);
            }
        }
        protected virtual void OnBeforeFunc(BeforeFuncEventArgs e)
        {
            if (BeforeFunc != null)
            {
                BeforeFunc(this, e);
            }
        }
        protected virtual void OnEndFunc(EndFuncEventArgs e)
        {
            if (EndFunc != null)
            {
                EndFunc(this, e);
            }
        }

        #endregion

        public string content = string.Empty;

        TempFun tempFun;
        TempConfig config;

        public TempMain() {
            tempFun = new TempFun();
            config = new TempConfig();
        }

        public TempMain(string filename)
        {
            tempFun = new TempFun();
            config = new TempConfig();
            content = tempFun.GetTemplate(config.templatePath + "html"+filename);
        }

        /// <summary>
        /// 解析辅助模板
        /// </summary>
        private void ParseTopAndFoot()
        {
            string labelRule = @"{kehenbar:template([\s\S]*?)}([\s\S]*?)";
            Regex r = new Regex(labelRule);
            MatchCollection mc = r.Matches(content);
            foreach (Match item in mc)
            {                
                Dictionary<string, string> dic = tempFun.GetFields(item.Groups[1].ToString().Trim());
                foreach (var it in dic)
                {
                    if (it.Key.ToLower().Trim() == "src")
                    {
                        string key = item.Groups[0].ToString();
                        string val = it.Value.Replace("\"", "");
                        content = content.Replace(key, tempFun.GetTemplate(config.templatePath +"html/"+ val));
                    }
                }
            }
        }

        /// <summary>
        /// 解析全局标签
        /// </summary>
        private void ParseGlobal()
        {
            content = content.Replace("{kehenbar:path}", config.path)      //程序跟目录
                .Replace("{kehenbar:templatepath}", config.templatePath)   //模版目录
                .Replace("[sys_userid]", System.Web.HttpContext.Current.Session["UserId"]+"");   //用户ID
           
        }

        /// <summary>
        /// 解析列表
        /// </summary>
        private void ParseList(int page,int lev=1)
        {
            string levstr = "";
            int data_row_count = 1;
            if(lev>0){levstr = lev.ToString();}

            string labelRule = @"{kehenbar:"+levstr+@"list([\s\S]*?)}([\s\S]*?){/kehenbar:"+levstr+"list}";
            string labelRuleField = @"\["+levstr+@"([\s\S]+?):([\s\S]+?)\]";
            string labelRulePage = @"\[kehenbar:page([\s\S]*?)\]";
            Regex r = new Regex(labelRule);
            MatchCollection mc = r.Matches(content);            

            foreach (Match item in mc)
            {
                #region 获取数据源
                Dictionary<string, string> dic = tempFun.GetFields(item.Groups[1].ToString().Trim());
                string key = item.Groups[2].ToString(); //html模版内容
                string keyHtml = string.Empty;          //html具体内容
                bool isParse = false; //是否禁止解析
                string tablename = string.Empty;
                string pageID = string.Empty;   //分页控件ID
                string pageTable = string.Empty;
                int numCount = 10000;              //获取多少条数据

                string sql_len = string.Empty;
                string sql_table = string.Empty;
                string sql_where = " where 1=1 ";
                string sql_order = string.Empty;

                foreach (var it in dic)
                {
                    string val = it.Value.Replace("\"", "");//条件值
                    if (it.Key.ToLower().Trim() == "len")
                    {
                        numCount = int.Parse(val);
                        sql_len = " top " + numCount+" ";
                    }
                    else if (it.Key.ToLower().Trim() == "table")
                    {
                        tablename = val;
                        List<string> columns = new List<string>();//字段
                        List<string> tables = new List<string>(); //表

                        #region 生成字段
                        DataTable table = SqlHelper.Query("select top 1 * from sys_database where tcode='"+SqlFunction.SqlFilter(val)+"'");
                        if (table==null || table.Rows.Count<=0) { return; };

                        DataTable cols = SqlHelper.Query("select * from sys_database_clumn where sys_database_id=" + table.Rows[0]["id"]);

                        columns.Add(table.Rows[0]["tcode"] + "." + "id '" + table.Rows[0]["tcode"] + "id'");
                        foreach (DataRow c in cols.Rows)
                        {
                            columns.Add(table.Rows[0]["tcode"] + ".[" + c["ccode"] + "] '" + table.Rows[0]["tcode"] + c["ccode"] + "'");
                        }
                        if (Convert.ToInt32( table.Rows[0]["ttype"] )== 2)
                        {
                            columns.Add(table.Rows[0]["tcode"] + "." + "pid '" + table.Rows[0]["tcode"] + "pid'");
                        }
                        if (!string.IsNullOrEmpty(table.Rows[0]["toutkey"]+""))
                        {
                            string[] outkeys = (table.Rows[0]["toutkey"] + "").Split(',');
                            for (int i = 0; i < outkeys.Length; i++)
                            {

                                tables.Add(outkeys[i]);
                                string _tcode = outkeys[i];
                                cols = SqlHelper.Query(string.Join(" ", new string[]{
                                     "select c.* "
                                    ,"from sys_database t"
                                    ,"    inner join sys_database_clumn c on t.id = c.sys_database_id"
                                    ,"where t.tcode = '"+_tcode+"'"
                                }));
                                columns.Add(outkeys[i] + "." + "id '" + outkeys[i] + "id'");
                                foreach (DataRow c in cols.Rows)
                                {
                                    columns.Add(outkeys[i] + "." + c["ccode"] + " '" + outkeys[i] + c["ccode"] + "'");
                                }
                            }
                            
                        }
                        #endregion

                        string columnstr = string.Join(",", columns.ToArray());

                        sql_table += columnstr + " from " + val;
                        pageTable += " from " + val;
                        foreach (var tb in tables)
                        {
                            sql_table += " left join " + tb + " on " + tb + ".id = " + val + "." + tb + "_id";
                            pageTable += " left join " + tb + " on " + tb + ".id = " + val + "." + tb + "_id";
                        }
                    }
                    else if (it.Key.ToLower().Trim() == "where")
                    {
                        //where=[istop:1,isjing:1]
                        string[] wheres = val.Replace("(", "").Replace(")", "").Split(',');
                        foreach (var w in wheres)
                        {
                            string andleft = w.Split(':')[0].Trim();
                            string andright = w.Split(':')[1].Trim();
                            if (string.IsNullOrEmpty(andleft) || string.IsNullOrEmpty(andright)) continue;
                            sql_where += " and " + andleft + "='" + andright+"'";
                        }

                    }
                    else if (it.Key.ToLower().Trim() == "wherelike")
                    {
                        //where=[istop:1,isjing:1]
                        string[] wheres = val.Replace("(", "").Replace(")", "").Split(',');
                        foreach (var w in wheres)
                        {
                            string andleft = w.Split(':')[0].Trim();
                            string andright = w.Split(':')[1].Trim();
                            if (string.IsNullOrEmpty(andleft) || string.IsNullOrEmpty(andright)) continue;
                            sql_where += " and " + andleft + " like '%" + andright + "%' ";
                        }

                    }
                    else if (it.Key.ToLower().Trim() == "order")
                    {
                        string[] sql_order_arr = new string[] { };
                        if (val.Contains(','))
                        {
                            sql_order += " order by ";
                            sql_order_arr=val.Split(',');
                            foreach (var soa in sql_order_arr)
                            {
                                sql_order += tablename + ".[" + soa + "] asc,";
                            }
                            sql_order = sql_order.Trim(',');
                        }
                        else
                        {
                            sql_order += " order by " + tablename + ".[" + val + "] asc";
                        }
                    }
                    else if (it.Key.ToLower().Trim() == "orderdesc")
                    {
                        string[] sql_order_arr = new string[] { };
                        if (val.Contains(','))
                        {
                            sql_order += " order by ";
                            sql_order_arr = val.Split(',');
                            foreach (var soa in sql_order_arr)
                            {
                                sql_order += tablename + ".[" + soa + "] desc,";
                            }
                            sql_order = sql_order.Trim(',');
                        }
                        else
                        {
                            sql_order += " order by " + tablename + ".[" + val + "] desc";
                        }
                    }
                    else if (it.Key.ToLower().Trim() == "page"){
                        pageID = val;
                    }
                    else if (it.Key.ToLower().Trim() == "parse") {
                        isParse = val == "no";
                    }
                }
                #endregion

                #region 替换内容
                if (isParse) { continue; }

                string sqlpage = string.Empty;
                string pageCss = string.Empty;
                string pageHtml = string.Empty;
                string allcount = "0"; //总条数
                int allPage = 0;//总页数
                if(!string.IsNullOrEmpty(pageID)){
                    sqlpage = " and " + tablename + ".id not in(select top " + numCount * (page - 1) + " id from " + tablename + sql_where + sql_order + " )";

                    allcount = SqlHelper.Query("select count(1) " + pageTable + sql_where).Rows[0][0]+"";
                    allPage = int.Parse(allcount) % numCount == 0 ? int.Parse(allcount) / numCount : int.Parse(allcount) / numCount + 1;
                    #region 分页模版
                    pageCss = string.Join("", new string[] { 
                         "<style type='text/css'>"
                        ,"    .pagebar {margin-top:10px}"
                        ,"    .pagebar span {display:inline-block;width:40px;height:30px;border:1px solid #999; border-radius:2px;}"
                        ,"    .pagebar span a{display:block;font-size:12px;text-align:center;line-height:30px}"
                        ,"</style>"
                    });

                    int prev = page - 1 < 1 ? 1 : page - 1;
                    int next = page + 1 > allPage ? page : page + 1;

                    pageHtml += "<div class='pagebar'>";
                    pageHtml += "    <span><a href='?page=" + prev + "'>上一页</a></span>";

                    for (int i = page; i <= (page+3); i++)
                    {
                        if (i > allPage) { break; }
                        
                        if(i==page){
                            if (i > 1)
                            {
                                pageHtml += "    <span><a href='?page=" + (i - 1) + "'>" + (i - 1) + "</a></span>";
                            }
                            pageHtml += "    <span><a style='background-color:#009688;color:#fff' href='?page=" + i + "'>" + i + "</a></span>";
                        }
                        else
                        {
                            pageHtml += "    <span><a href='?page=" + i + "'>" + i + "</a></span>";                            
                        }                        
                    }

                    pageHtml += "    <span><a href='?page=" + next + "'>下一页</a></span>";
                    pageHtml += "</div>";
                   
                    #endregion

                    #region 替换分页模版
                    Regex pager = new Regex(labelRulePage);
                    MatchCollection pagemc = pager.Matches(content);
                    foreach (Match pageitem in pagemc)
                    {
                        Dictionary<string, string> pagedic = tempFun.GetFields(pageitem.Groups[1].ToString().Trim());
                        foreach (var paged in pagedic)
                        {
                            string pageval = paged.Value.ToLower().Trim();
                            if (paged.Key.ToLower().Trim() == "for")
                            {
                                if (pageval == pageID.ToLower())
                                {
                                    content = content.Replace(pageitem.Value, pageCss+pageHtml);
                                    break;
                                }
                            }
                        }
                    }
                    #endregion

                    data_row_count = 1 + (page - 1) * numCount;
                }
                else
                {
                    data_row_count = 1;
                }

                string sqlselect = "select " + sql_len + sql_table + sql_where + sqlpage + sql_order;
                DataTable listTable = SqlHelper.Query(sqlselect);
                Regex rfield = new Regex(labelRuleField);
                MatchCollection mcfield = rfield.Matches(key);
                string contentHtml = string.Empty;
                foreach (DataRow row in listTable.Rows)
                {
                    string rowContentHtml = key;
                    foreach (Match field in mcfield)
                    {
                        string fContent = field.Groups[0].ToString();
                        string fkey = field.Groups[1].ToString();                       
                        string fval = field.Groups[2].ToString();

                        if (fkey == "list" && fval == "i") { continue; }
                        try
                        {
                            rowContentHtml = rowContentHtml.Replace(fContent, SqlFunction.UnSqlFilter(row[fkey + fval] + ""));
                        }
                        catch{}
                        
                    }
                    contentHtml += rowContentHtml.Replace("[list:i]", data_row_count + "");
                    contentHtml = ParseButton(contentHtml, row, tablename);//解析按钮
                    data_row_count++;
                }
                content = content.Replace(item.Value, contentHtml);
                #endregion
            }

            #region 递归处理
            if (mc.Count <= 0)
            {
                if (lev != 0)
                {
                    ParseList(page, 0);
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (lev == 0)
                {
                    return;
                }
                else
                {
                    lev =  lev+1;
                    ParseList(page, lev);
                }
            }
            #endregion
        }

        /// <summary>
        /// 解析IF判断
        /// </summary>
        private void ParseIf()
        { 
            string labelRule = @"{kehenbar:if([\s\S]*?)}([\s\S]*?){/kehenbar:if}";
            Regex r = new Regex(labelRule);
            MatchCollection mc = r.Matches(content);
            foreach (Match item in mc)
            {
                string htmlcontent = item.Groups[2].ToString();//if标签包裹的html内容
                Dictionary<string, FieldForIf> dic = tempFun.GetFieldsForIf(item.Groups[1].ToString().Trim());
                bool result = true;

                #region 判断条件 多个条件按照 且 的关系来执行
                foreach (var it in dic)
                {
                    switch (it.Key)
                    {
                        case "=":
                            if (it.Value.rightval == "\"\"")
                            {
                                if (it.Value.leftval != "") result = false;
                            }
                            else
                            {
                                if (it.Value.leftval != it.Value.rightval) result = false;
                            }
                            
                            break;
                        case ">":
                            if (TempFun.Isnum(it.Value.leftval) && TempFun.Isnum(it.Value.rightval))
                            {
                                if ( int.Parse(it.Value.leftval) <= int.Parse(it.Value.rightval))
                                    result = false;
                            }
                            
                            break;
                        case "<":
                            if (TempFun.Isnum(it.Value.leftval) && TempFun.Isnum(it.Value.rightval))
                            {
                                if (int.Parse(it.Value.leftval) >= int.Parse(it.Value.rightval))
                                    result = false;
                            }
                            break;
                        case "!=":
                            if (it.Value.rightval == "\"\"")
                            {
                                if (it.Value.leftval == "") result = false;
                            }
                            else
                            {
                                if (it.Value.leftval == it.Value.rightval) result = false;
                            }
                            break;
                    }
                }
                #endregion

                if (!htmlcontent.Contains("{else}"))
                {
                    if (result)
                    {
                        content = content.Replace(item.Value, htmlcontent);
                    }
                    else
                    {
                        content = content.Replace(item.Value, "");
                    }
                }
                else
                {
                    string[] htmlcontent_else = Regex.Split(htmlcontent, "{else}");
                    if (result)
                    {
                        content = content.Replace(item.Value, htmlcontent_else[0]);
                    }
                    else
                    {
                        content = content.Replace(item.Value, htmlcontent_else[1]);
                    }
                }
            }
        }

        /// <summary>
        /// 解析按钮
        /// </summary>
        private string ParseButton(string _content,DataRow datarow,string tablename)
        {

            string labelRule = @"{kehenbar:button([\s\S]*?)}";
            Regex r = new Regex(labelRule);
            MatchCollection mc = r.Matches(_content);

            #region 获取数据源
            List<string> buttonIdList = new List<string>();
            foreach (Match item in mc)
            {
                Dictionary<string, string> dic = tempFun.GetFields(item.Groups[1].ToString().Trim());                
                foreach (var it in dic)
                {
                    if (it.Key.ToLower().Trim() == "id")
                    {
                        buttonIdList.Add(it.Value.ToString());
                    }
                }
            }
            string buttonIdWhere = string.Join("','", buttonIdList.ToArray());
            DataTable buttonTable = SqlHelper.Query("select * from sys_buttons where id in('" + buttonIdWhere + "')");
            #endregion

            #region 解析标签
            int forcount = 0;
            foreach (Match item in mc)
            {
                forcount++;
                Dictionary<string, string> dic = tempFun.GetFields(item.Groups[1].ToString().Trim());
                string buttonid = string.Empty;
                DataRow buttonrow = null;
                foreach (var it in dic)
                {
                    if (it.Key.ToLower().Trim() == "id")
                    {
                        buttonid = it.Value.ToString();
                        buttonrow = buttonTable.Select("id=" + buttonid).FirstOrDefault();
                    }
                }

                if (buttonrow == null) { continue; }

                string buttonClass = "class=\"layui-btn " + buttonrow["chicun"] + " " + buttonrow["yanse"] + "\"";
                string buttonFilter = Guid.NewGuid().ToString();
                string buttonType = "lay-submit";
                string buttonHtml = string.Join(" ", new string[]{
                    "<button",buttonClass,buttonType,"lay-filter=\""+buttonFilter+"\"",">" ,
                        buttonrow["bname"] +"",
                    "</button>"
                });

                int opentype = Convert.ToInt32( buttonrow["bopentype"]);

                string kuan = "80%";
                string gao = "80%";
                if ("1" == buttonrow["tankuangchicun"] + "")
                {
                    kuan = buttonrow["baifenbi_k"] + "%";
                    gao = buttonrow["baifenbi_g"] + "%";
                }
                else
                {
                    kuan = buttonrow["xiangsu_k"] + "px";
                    gao = buttonrow["xiangsu_g"] + "px";
                }

                //mp_triggerid=sys_triggerid mp_triggerid=sys_triggerid
                string buttoncanshu = buttonrow["canshuliebiao"] + "";
                string urlparm = "?";
                Dictionary<string, string> buttoncanshudic = tempFun.GetFields(buttoncanshu.Trim());
                foreach (var kv in buttoncanshudic)
                {
                    try
                    {
                        urlparm += kv.Key + "=" + datarow[kv.Value] + "&";
                    }
                    catch
                    {
                        urlparm += kv.Key + "=" + kv.Value + "&";
                    }                    
                }
                urlparm = urlparm.Trim('&');

                string ajaxSendStr = string.Empty;
                if ("4"==buttonrow["benent"]+"")
                {
                    //查询
                    if (1 == opentype) {
                        ajaxSendStr = "window.location.href = '/custom/index/" + buttonrow["id"] + urlparm + "'";
                    }
                    else if (2 == opentype)
                    {
                        ajaxSendStr = "window.open( '/custom/index/" + buttonrow["id"] + urlparm + "')";
                    }
                    else if (3 == opentype) {
                        ajaxSendStr = string.Join(" ", new string[] { 
                            "layer.open({",
                                "type:2",
                                ",title:'"+buttonrow["bname"]+"'",
                                ",area:['"+kuan+"','"+gao+"']",
                                ",content:'/custom/index/"+buttonrow["id"]+urlparm+"'",
                            "})"
                        });
                    }
                }
                else if ("2" == buttonrow["benent"] + "")
                {
                    //删除
                    ajaxSendStr = string.Join(" ", new string[] { 
                        "layer.confirm('确定要删除这条数据吗？',function(){",
                            "kehenbar.buttonFuncDelete(data.field,function(res){",
                                "res = JSON.parse(res);",
                                "layer.alert(res.msg, function (index) {",
                                    "layer.close(index);",
                                    "return false;",
                                "})",
                            "})",
                        "},function(index){",
                            "layer.close(index);",
                            "return false;",
                        "})"                        
                    });
                }
                string buttonAction = string.Join(" ", new string[] { 
                    "form.on('submit("+buttonFilter+")',function(data){",
                        ajaxSendStr,";",
                        "return false;",
                    "})"
                });
                string buttonScript = string.Join(" ", new string[]{                
                    "<script type=\"text/javascript\">",
                        "layui.use(['form', 'kehenbar'], function () {",
                            "var form = layui.form(), kehenbar = layui.kehenbar, $ = layui.jquery;",
                            buttonAction,";",
                        "})",
                    "</script>"
                });

                string tablenameandrowid = "";
                if (mc.Count == forcount) {
                    tablenameandrowid = string.Join("", new string[] { 
                        "<input type=\"hidden\" name=\"__tablename\" value=\""+tablename+"\"/>",
                        "<input type=\"hidden\" name=\"__rowid\" value=\""+datarow[0]+"\"/>"
                    });
                }
                _content = _content.Replace(item.Value, buttonHtml + buttonScript + tablenameandrowid);
            }
            return _content;
            #endregion
        }

        private void HtmlInit(string _page)
        {
            int page = string.IsNullOrEmpty(_page) ? 1 : int.Parse(_page);
            BeforeFuncEventArgs before = new BeforeFuncEventArgs(content);
            OnBeforeFunc(before);

            ParseTopAndFoot();

            TempFuncEventArgs e = new TempFuncEventArgs(content);
            OnTempFunc(e);

            ParseGlobal();
            ParseList(page);

            ParseContent();
            ParseCount();

            ParseIf();
            //ParseButton();
            EndFuncEventArgs end = new EndFuncEventArgs(content);
            OnEndFunc(end);
        }

        /// <summary>
        /// 解析单篇文章
        /// </summary>
        public void ParseContent()
        {
            string labelRule = @"{kehenbar:content([\s\S]*?)}([\s\S]*?){/kehenbar:content}";
            string labelRuleField = @"\[([\s\S]+?):([\s\S]+?)\]";
            Regex r = new Regex(labelRule);
            MatchCollection mc = r.Matches(content);

            foreach (Match item in mc)
            {
                #region 获取数据源
                Dictionary<string, string> dic = tempFun.GetFields(item.Groups[1].ToString().Trim());
                string key = item.Groups[2].ToString(); //html模版内容
                string keyHtml = string.Empty;          //html具体内容
                
                string tablename = string.Empty;
                string sql_table = string.Empty;
                string sql_where = " where 1=1 ";

                foreach (var it in dic)
                {
                    string val = it.Value.Replace("\"", "");//条件值
                    if (it.Key.ToLower().Trim() == "table")
                    {
                        tablename = val;
                        List<string> columns = new List<string>();//字段
                        List<string> tables = new List<string>(); //表

                        #region 生成字段
                        DataTable table = SqlHelper.Query("select top 1 * from sys_database where tcode='" + SqlFunction.SqlFilter(val) + "'");
                        if (table == null || table.Rows.Count <= 0) { return; };

                        DataTable cols = SqlHelper.Query("select * from sys_database_clumn where sys_database_id=" + table.Rows[0]["id"]);

                        columns.Add(table.Rows[0]["tcode"] + "." + "id '" + table.Rows[0]["tcode"] + "id'");
                        foreach (DataRow c in cols.Rows)
                        {
                            columns.Add(table.Rows[0]["tcode"] + ".[" + c["ccode"] + "] '" + table.Rows[0]["tcode"] + c["ccode"] + "'");
                        }
                        if (Convert.ToInt32(table.Rows[0]["ttype"]) == 2)
                        {
                            columns.Add(table.Rows[0]["tcode"] + "." + "pid '" + table.Rows[0]["tcode"] + "pid'");
                        }
                        if (!string.IsNullOrEmpty(table.Rows[0]["toutkey"] + ""))
                        {
                            string[] outkeys = (table.Rows[0]["toutkey"] + "").Split(',');
                            for (int i = 0; i < outkeys.Length; i++)
                            {
                                tables.Add(outkeys[i]);
                                string _tcode = outkeys[i];
                                cols = SqlHelper.Query(string.Join(" ", new string[]{
                                     "select c.* "
                                    ,"from sys_database t"
                                    ,"    inner join sys_database_clumn c on t.id = c.sys_database_id"
                                    ,"where t.tcode = '"+_tcode+"'"
                                }));
                                columns.Add(outkeys[i] + "." + "id '" + outkeys[i] + "id'");
                                foreach (DataRow c in cols.Rows)
                                {
                                    columns.Add(outkeys[i] + "." + c["ccode"] + " '" + outkeys[i] + c["ccode"] + "'");
                                }
                            }
                        }
                        #endregion

                        string columnstr = string.Join(",", columns.ToArray());

                        sql_table += columnstr + " from " + val;
                        foreach (var tb in tables)
                        {
                            sql_table += " left join " + tb + " on " + tb + ".id = " + val + "." + tb + "_id";
                        }
                    }
                    else if (it.Key.ToLower().Trim() == "where")
                    {
                        //where=[istop:1,isjing:1]
                        string[] wheres = val.Replace("(", "").Replace(")", "").Split(',');
                        foreach (var w in wheres)
                        {
                            string andleft = w.Split(':')[0].Trim();
                            string andright = w.Split(':')[1].Trim();
                            if (string.IsNullOrEmpty(andleft) || string.IsNullOrEmpty(andright)) continue;
                            sql_where += " and " + andleft + "='" + andright + "'";
                        }

                    }
                    else if (it.Key.ToLower().Trim() == "wherelike")
                    {
                        //where=[istop:1,isjing:1]
                        string[] wheres = val.Replace("(", "").Replace(")", "").Split(',');
                        foreach (var w in wheres)
                        {
                            string andleft = w.Split(':')[0].Trim();
                            string andright = w.Split(':')[1].Trim();
                            if (string.IsNullOrEmpty(andleft) || string.IsNullOrEmpty(andright)) continue;
                            sql_where += " and " + andleft + " like '%" + andright + "%' ";
                        }

                    }
                }
                #endregion

                #region 替换内容

                string sqlselect = "select top 1 " + sql_table + sql_where;
                DataTable listTable = SqlHelper.Query(sqlselect);
                new DbEntity().TriggerRun("sel", tablename, JsonConvert.SerializeObject(listTable)); //执行触发器

                Regex rfield = new Regex(labelRuleField);
                MatchCollection mcfield = rfield.Matches(key);
                string contentHtml = key;
                foreach (Match field in mcfield)
                {
                    string fContent = field.Groups[0].ToString();
                    string fkey = field.Groups[1].ToString();
                    string fval = field.Groups[2].ToString();

                    try
                    {
                        contentHtml = contentHtml.Replace(fContent, SqlFunction.UnSqlFilter(listTable.Rows[0][fkey + fval] + ""));
                    }
                    catch { }

                }
                
                content = content.Replace(item.Value, contentHtml);
                #endregion
            }
        }

        /// <summary>
        /// 表中是否存在这条数据
        /// </summary>
        public void ParseCount()
        {
            string labelRule = @"{kehenbar:count([\s\S]*?)}([\s\S]*?){/kehenbar:count}";
            Regex r = new Regex(labelRule);
            MatchCollection mc = r.Matches(content);
            Dictionary<string, List<string>> tcdic = new Dictionary<string, List<string>>();
            
            
            foreach (Match item in mc)
            {
                string tablename = string.Empty;
                List<string> tablenameList = new List<string>();
                Dictionary<string, string> sqlwhereList = new Dictionary<string, string>();

                #region 统计表和字段
                string htmlcontent = item.Groups[2].ToString();//标签包裹的html内容
                Dictionary<string, string> dic = tempFun.GetFields(item.Groups[1].ToString().Trim());

                foreach (var it in dic)
                {
                    string val = it.Value.ToLower().Trim();
                    if (it.Key.ToLower().Trim() == "table")
                    {
                        tablename = SqlFunction.SqlFilter(val);

                        string tableoutkeys = SqlHelper.Query(string.Join(" ", new string[] { 
                            "select toutkey from sys_database where tcode = '"+tablename+"'"
                        })).Rows[0][0]+"";

                        if (!string.IsNullOrEmpty(tableoutkeys))
                        {
                            tablenameList.AddRange(tableoutkeys.Split(',').ToList());
                        }
                    }                    
                    else if (it.Key.ToLower().Trim() == "where")
                    {
                        string tablewhere = val.Trim('(').Trim(')');
                        string[] tablewherearr = tablewhere.Split(',');

                        foreach (var twarr in tablewherearr)
                        {
                            if (twarr != null && twarr != "")
                            {
                                string sqlwhereleft = twarr.Split(':')[0];
                                string sqlwhereright = twarr.Split(':')[1];

                                sqlwhereList.Add(sqlwhereleft, sqlwhereright);
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                #endregion

                #region 替换内容

                string sqlselect = string.Empty;
                sqlselect += " select count(1) from "+tablename ;
                foreach (var sqltb in tablenameList)
                {
                    sqlselect += " inner join " + sqltb + " on " + sqltb + ".id=" + tablename + "." + sqltb+"_id ";         
                }
                sqlselect += " where 1=1 ";
                foreach (KeyValuePair<string, string> sqlwher in sqlwhereList)
                {
                    string leftsql = sqlwher.Key.Trim().ToLower();
                    string rightsql = sqlwher.Value.Trim().ToLower();
                    if (leftsql.IndexOf('.') > 0)
                    {
                        leftsql = leftsql.Replace(".", ".[") + "]";
                    }
                    else
                    {
                        leftsql = "[" + leftsql + "]";
                    }
                    rightsql = SqlFunction.SqlFilter(rightsql);
                    rightsql = string.IsNullOrEmpty(rightsql) ? "''" : rightsql;

                    sqlselect += " and " + leftsql + "=" + rightsql;
                }
                string resultcount = SqlHelper.Query(sqlselect).Rows[0][0] + "";
                htmlcontent = htmlcontent.Replace("[count:num]", resultcount);
                content = content.Replace(item.Value, htmlcontent);
                #endregion
            }
        }


        /// <summary>
        /// 初始化页面 分页
        /// </summary>
        /// <param name="_page"></param>
        public void ParseHtml(string _page)
        {
            HtmlInit(_page);
            tempFun.Echo(content);
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        public void ParseHtml()
        {
            HtmlInit("1");
            tempFun.Echo(content);
        }

        /// <summary>
        /// 返回初始化页面html
        /// </summary>
        /// <returns></returns>
        public string ReturnHtml()
        {
            HtmlInit("1");
            return content;
        }
    }
}
