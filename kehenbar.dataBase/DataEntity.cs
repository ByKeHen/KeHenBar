using kehenbar.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kehenbar.DataBase
{
    public class DbEntity
    {
        public string Index(string data)
        {
            FormModel fm = JsonConvert.DeserializeObject<FormModel>(data);

            string action = fm.action.Trim().ToLower();
            string table = fm.table.Trim();
            string where = fm.where.Trim().ToLower();
            string fields = fm.field.Trim();
            string order = fm.order + "";

            /*
             * 
             *  在这里验证权限
             *  yanZhengQuanXian()
             * 
             */

            string result = string.Empty;
            switch (action)
            {
                case "insert":
                    result = Insert(table, where, fields);
                    break;
                case "delete":
                    result = Delete(table, where, fields);
                    break;
                case "update":
                    result = Update(table, where, fields);
                    break;
                case "select":
                    result = Select(table, where, fields, order);
                    break;
                default:
                    result = "action参数错误";
                    break;
            }

            return result;
        }


        public List<ColumnModel> GetTableColumn(string table)
        {
            string sql = string.Join(" ", new string[] {
                 "select c.cname,c.ccode,c.ctype,c.clength,t.tcode "
                ,"from sys_database_clumn c"
                ,"inner join sys_database t on c.sys_database_id = t.id"
                ,"where t.tcode='"+table+"'"
            });

            DataTable datatable = SqlHelper.Query(sql);
            List<ColumnModel> cmlist = new List<ColumnModel>();
            if (datatable != null && datatable.Rows.Count > 0)
            {
                ColumnModel cmo = new ColumnModel();
                cmo.ccode = "id";
                cmo.clength = 8;
                cmo.cname = "主键";
                cmo.ctype = "int";
                cmlist.Add(cmo);

                foreach (DataRow row in datatable.Rows)
                {
                    ColumnModel cm = new ColumnModel();
                    cm.ccode = row["ccode"].ToString();
                    cm.clength = Convert.ToInt32(row["clength"]);
                    cm.cname = row["cname"].ToString();
                    cm.ctype = row["ctype"].ToString();
                    cmlist.Add(cm);
                }
            }

            //string tablecode = datatable.Rows[0]["tcode"] + "";
            //string sql1 = string.Join(" ", new string[] { 
            //    "select count(1)" 
            //    ,"from syscolumns "
            //    ,"where name = 'pid' and id = object_id('"+tablecode+"')"
            //});

            return cmlist;
        }

        public void TriggerRun(string action,string tablename,string jsondata)
        {
            DataTable triggerTable = SqlHelper.Query(string.Join(" ",new string[]{
                "select *",                
                "from sys_trigger",
                "inner join sys_database on sys_trigger.sys_database_id = sys_database.id",
                "where sys_trigger.event = '"+action+"'",
                    "and sys_database.tcode='"+tablename+"'"
            }));

            string triggerStr = string.Empty;
            foreach (DataRow item in triggerTable.Rows)
            {
                string _tri = SqlFunction.UnSqlFilter(item["sqlcontent"] + "");
                if (string.IsNullOrEmpty(_tri.Trim())) { continue; }
                triggerStr += SqlFunction.UnSqlFilter(item["sqlcontent"] + ";GO;");
            }
            if (string.IsNullOrEmpty(triggerStr)) { return; }
            triggerStr = triggerStr.Trim(';');
            try
            {
                string userid = System.Web.HttpContext.Current.Session["UserId"] + "";
                if(!string.IsNullOrEmpty(userid))
                {
                    triggerStr = triggerStr.Replace("[sys_userid]", userid);
                }

                JArray jsonArrayData = JArray.Parse(jsondata);

                string labelRule = @"\[([\s\S]*?)\]";
                Regex r = new Regex(labelRule);
                MatchCollection mc = r.Matches(triggerStr);
                foreach (Match item in mc)
                {
                    string sqlparm = item.Groups[1].ToString();
                    if (jsonArrayData[0][sqlparm] == null) { continue; }

                    triggerStr = triggerStr.Replace(item.Value, jsonArrayData[0][sqlparm] + "");
                }

                string resultsql = SqlHelper.ExecuteSqlForGo(triggerStr);
                if (!string.IsNullOrEmpty(resultsql)) {
                    Logs.WriteLog(tablename + "执行触发器" + action + "时出错，产生异常的sql内容为[" + resultsql + "]", 2);
                }
            }
            catch(Exception e){
                Logs.WriteLog("运行触发器出现异常"+e.Message, 2);
            }
        }

        private string Select(string table, string where, string fields, string order = "")
        {
            table = SqlFunction.SqlFilter(table);

            List<string> outkeylist = GetTableOutKeys(table);
            string result = string.Empty;

            List<ColumnModel> cmlist = GetTableColumn(table);
            List<string> colList = new List<string>();
            foreach (ColumnModel item in cmlist)
            {
                colList.Add(table + ".[" + item.ccode + "] '" + table + item.ccode + "'");
            }
            foreach (var item in outkeylist)
            {
                cmlist = GetTableColumn(item);
                foreach (ColumnModel it in cmlist)
                {
                    colList.Add(item + ".[" + it.ccode + "] '" + item + it.ccode + "'");
                }
            }


            string sql_column = string.Join(",", colList.ToArray());

            string sql_from = " from " + table;
            foreach (var item in outkeylist)
            {
                sql_from += " left join " + item + " on " + item + ".id = " + table + "." + item + "_id ";
            }

            List<string> whereList = GetTableWhere(where);
            string sql_where = " 1=1 ";
            foreach (var item in whereList)
            {
                if (string.IsNullOrEmpty(item)) { continue; };
                string whereKey = item.Split('=')[0];
                string whereVal = item.Split('=')[1];
                sql_where += " and " + SqlFunction.SqlFilter(whereKey) + "='" + SqlFunction.SqlFilter(whereVal) + "'";
            }
            string sql_order = string.IsNullOrEmpty(SqlFunction.SqlFilter(order)) ? "" : SqlFunction.SqlFilter(order);

            string sql = string.Empty;
            if (string.IsNullOrEmpty(sql_order))
            {
                sql = " select " + sql_column + sql_from + " where " + sql_where;
            }
            else
            {
                sql = " select " + sql_column + sql_from + " where " + sql_where + " order by [" + sql_order + "] asc";
            }


            DataTable dt = SqlHelper.Query(sql);
            return JsonConvert.SerializeObject(dt);
        }

        private string Update(string table, string where, string fields)
        {
            table = SqlFunction.SqlFilter(table);

            List<string> outkeylist = GetTableOutKeys(table);
            string result = string.Empty;

            string sql_column = string.Empty;
            List<ColumnModel> cmlist = GetTableColumn(table);
            JObject jo = JObject.Parse(fields);
            foreach (var item in cmlist)
            {
                if (jo[item.ccode] != null)
                {
                    string yanzhengResult = YanZheng(item, jo[item.ccode] + "");
                    if (string.IsNullOrEmpty(yanzhengResult))
                    {
                        sql_column += "[" + item.ccode + "]" + "='" + SqlFunction.SqlFilter(jo[item.ccode] + "") + "',";
                    }
                    else
                    {
                        return yanzhengResult;
                    }
                }
            }
            sql_column = sql_column.Trim(',');
            string sql_from = " from " + table;
            foreach (var item in outkeylist)
            {
                sql_from += " inner join " + item + " on " + item + ".id = " + table + "." + item + "_id ";
            }

            List<string> whereList = GetTableWhere(where);
            string sql_where = " 1=1 ";
            foreach (var item in whereList)
            {
                string whereKey = item.Split('=')[0];
                string whereVal = item.Split('=')[1];
                sql_where += " and " + SqlFunction.SqlFilter(whereKey) + "='" + SqlFunction.SqlFilter(whereVal) + "'";
            }

            string sql = " update " + table + " set " + sql_column + sql_from + " where " + sql_where;
            int res = SqlHelper.ExecuteSql(sql);

            ResultModel rm = new ResultModel();
            if (res > 0)
            {
                rm.code = 0;
                rm.msg = "修改成功。";

                TriggerRun("upd",table,Select(table,where,""));
            }
            else
            {
                rm.code = 1;
                rm.msg = "网络错误。";
            }
            return JsonConvert.SerializeObject(rm);
        }

        private string Delete(string table, string where, string fields)
        {
            table = SqlFunction.SqlFilter(table);

            List<string> tablenameList = new List<string>();
            Dictionary<string, string> sqlwhereList = new Dictionary<string, string>();
            string tableoutkeys = SqlHelper.Query(string.Join(" ", new string[] { 
                "select toutkey from sys_database where tcode = '"+table+"'"
            })).Rows[0][0] + "";

            if (!string.IsNullOrEmpty(tableoutkeys))
            {
                tablenameList.AddRange(tableoutkeys.Split(',').ToList());
            }

            string[] tablewherearr = where.Split(',');

            foreach (var twarr in tablewherearr)
            {
                if (twarr != null && twarr != "")
                {
                    string sqlwhereleft = twarr.Split('=')[0];
                    string sqlwhereright = twarr.Split('=')[1];

                    sqlwhereList.Add(sqlwhereleft, sqlwhereright);
                }
            }


            string sqlselect = string.Empty;
            sqlselect += " delete " + table + " from " + table;
            foreach (var sqltb in tablenameList)
            {
                sqlselect += " left join " + sqltb + " on " + sqltb + ".id=" + table + "." + sqltb + "_id ";
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
            string resultjson = Select(table, where, "");
            int res = SqlHelper.ExecuteSql(sqlselect);

            ResultModel rm = new ResultModel();
            if (res > 0)
            {
                rm.code = 0;
                rm.msg = "删除成功。";

                TriggerRun("del", table, resultjson);
            }
            else
            {
                rm.code = 1;
                rm.msg = "网络错误。";
            }
            return JsonConvert.SerializeObject(rm);
        }

        private string Insert(string table, string where, string fields)
        {
            table = SqlFunction.SqlFilter(table);

            string result = string.Empty;
            string sql_column = string.Empty;
            List<ColumnModel> cmlist = GetTableColumn(table);
            JObject jo = JObject.Parse(fields);

            List<string> sql_keylist = new List<string>();
            List<string> sql_vallist = new List<string>();
            foreach (var item in cmlist)
            {
                if (item.ccode.Trim().ToLower().Equals("id")) { continue; }

                if (jo[item.ccode] != null)
                {
                    string yanzhengResult = YanZheng(item, jo[item.ccode] + "");
                    if (string.IsNullOrEmpty(yanzhengResult))
                    {
                        sql_keylist.Add(item.ccode);
                        sql_vallist.Add(SqlFunction.SqlFilter(jo[item.ccode] + ""));
                    }
                    else
                    {
                        return yanzhengResult;
                    }
                }
                else
                {
                    sql_keylist.Add(item.ccode);
                    sql_vallist.Add("");
                }
            }

            string sql_keys = string.Join("],[", sql_keylist.ToArray());
            string sql_vals = string.Join("','", sql_vallist.ToArray());
            string sql = " insert into " + table + "([" + sql_keys + "]) values ('" + sql_vals + "');select @@identity";
            string _res =SqlHelper.GetSingle(sql)+"";
            int res = string.IsNullOrEmpty(_res) ? 0 : int.Parse(_res);
            ResultModel rm = new ResultModel();
            if (res > 0)
            {
                rm.code = 0;
                rm.msg = "添加成功。";

                TriggerRun("add",table,Select(table, table + ".id=" + res, ""));
            }
            else
            {
                rm.code = 1;
                rm.msg = "网络错误。";
            }
            return JsonConvert.SerializeObject(rm);
        }

        private string YanZheng(ColumnModel item, string val)
        {
            string result = string.Empty;
            switch (item.ctype)
            {
                case "varchar":
                    if (val.Length > item.clength)
                    {
                        result = item.cname + "的内容不要超过" + item.clength + "个字。";
                    }
                    break;
                case "datetime":
                    try
                    {
                        DateTime.Parse(val);
                    }
                    catch
                    {
                        result = item.cname + "输入的日期格式不正确。";
                    }
                    break;
                case "int":
                    try
                    {
                        Convert.ToInt32(val);
                    }
                    catch
                    {
                        result = item.cname + "必须输入数字。";
                    }
                    break;
            }

            return result;
        }

        private List<string> GetTableOutKeys(string tablename)
        {
            List<string> result = new List<string>();
            DataTable dt = SqlHelper.Query("select toutkey from sys_database where tcode = '" + tablename + "'");
            if (dt != null && dt.Rows.Count > 0)
            {

                string outkeys = dt.Rows[0]["toutkey"] + "";
                if (string.IsNullOrEmpty(outkeys))
                {
                    return result;
                }
                else
                {
                    if (outkeys.Contains(","))
                    {
                        foreach (var item in outkeys.Split(','))
                        {
                            result.Add(item);
                        }
                        return result;
                    }
                    else
                    {
                        result.Add(outkeys);
                        return result;
                    }
                }
            }
            else
            {
                return result;
            }
        }

        private List<string> GetTableWhere(string where)
        {
            List<string> whereList = new List<string>();
            if (where.Contains(','))
            {
                foreach (var item in where.Split(','))
                {
                    whereList.Add(item.Trim());
                }
            }
            else
            {
                whereList.Add(where.Trim());
            }
            return whereList;
        }
    }
}
