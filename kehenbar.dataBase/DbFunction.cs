using kehenbar.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kehenbar.DataBase
{
    public class DbFunction
    {
        public DataTable DeserializeColumnVal(string tablecode, DataTable sourcetable, List<string> columnlinkList)
        {
            List<ColumnViewModel> columnsview = GetColumnView(tablecode);
            List<string> outkeys = GetOutKeys(tablecode);

            DataTable returntable = sourcetable.Clone();
            for (int i = 0; i < returntable.Columns.Count; i++)
            {
                returntable.Columns[i].DataType = typeof(string);
            }

            foreach (DataRow row in sourcetable.Rows)
            {
                returntable.ImportRow(row);
            }

            //解析连接
            for (int i = 0; i < sourcetable.Rows.Count; i++)
            {
                for (int j = 0; j < sourcetable.Columns.Count; j++)
                {
                    if (!string.IsNullOrEmpty(columnlinkList[j]))
                    {
                        string id = returntable.Rows[i]["id"] + "";
                        string columnLink = columnlinkList[j].Replace("#p1#", id);
                        string columnUrl = "<a style=\"color:#01AAED\" href=\"" + columnLink + "\">" + returntable.Rows[i][j] + "</a>";
                        returntable.Rows[i][j] = columnUrl;
                    }
                }
            }

            //解析自定义样式
            foreach (ColumnViewModel cvm in columnsview.FindAll(o => o.table == tablecode))
            {
                string column = cvm.column;
                if (sourcetable.Columns.Contains(column))
                {

                    for (int i = 0; i < sourcetable.Rows.Count; i++)
                    {
                        string oldval = sourcetable.Rows[i][column] + "";
                        string newval = cvm.keyval.Find(v => v.jian == oldval).zhi;
                        returntable.Rows[i][column] = newval;
                    }
                }                
            }

            //解析外键表
            foreach (string outkey in outkeys)
            {
                string column = outkey + "_id";
                if (sourcetable.Columns.Contains(column))
                {
                    for (int i = 0; i < sourcetable.Rows.Count; i++)
                    {
                        Dictionary<string,string> outkeyvalues = GetOutKeyValues(outkey);
                        if (outkeyvalues.Count>0)
                        {
                            string oldval = sourcetable.Rows[i][column] + "";
                            if (!outkeyvalues.ContainsKey(oldval)) continue;
                            string newval = outkeyvalues[oldval] + "";
                            returntable.Rows[i][column] = newval;
                        }
                    }
                }   
            }

            return returntable;
        }

        /// <summary>
        /// 获取字段显示方式
        /// </summary>
        /// <param name="tablecode"></param>
        /// <returns></returns>
        public List<ColumnViewModel> GetColumnView(string tablecode)
        {
            List<string> OutKeyList = GetOutKeys(tablecode);
            OutKeyList.Add(tablecode);
            string sqlwhere = string.Join("','", OutKeyList.ToArray());
            string sqlselect = string.Join(" ", new string[]{
                "select showtype",
                "from sys_database_clumn ",
                "left join sys_database on sys_database_clumn.sys_database_id=sys_database.id",
                "where sys_database.tcode in('"+sqlwhere+"')",
	            "    and ISNULL(showtype,'')<>''"
            });

            List<ColumnViewModel> columnViewModelList = new List<ColumnViewModel>();
            DataTable table = SqlHelper.Query(sqlselect);
            foreach (DataRow item in table.Rows)
            {
                ColumnViewModel columnviewmodel = JsonConvert.DeserializeObject<ColumnViewModel>(item[0] + "");
                columnViewModelList.Add(columnviewmodel);
            }

            return columnViewModelList;
        }

        /// <summary>
        /// 获取外键表的显示内容
        /// </summary>
        /// <param name="tablecode"></param>
        /// <returns></returns>
        public Dictionary<string,string> GetOutKeyValues(string tablecode) 
        {
            tablecode = SqlFunction.SqlFilter(tablecode);

            string sqlselect = string.Join(" ", new string[] { 
                "select ccode ",
                "from sys_database_clumn ",
                "left join sys_database on sys_database_clumn.sys_database_id = sys_database.id",
                "where sys_database.tcode = @tablecode",
	            "    and sys_database_clumn.waijianzhi = 1"
            });

            DataTable waijiantable = SqlHelper.Query(sqlselect, new SqlParameter("@tablecode", tablecode))
            .Tables[0];

            string waijian = "id";
            if (waijiantable != null && waijiantable.Rows.Count > 0)
            {
                waijian = waijiantable.Rows[0][0] + "";
            }

            string sql = string.Join(" ", new string[] { 
                 "select id 'key',"+waijian +" val"
                ,"from "+tablecode
                ,"order by id desc"
            });

            try
            {
                Dictionary<string,string> returndic = new Dictionary<string,string>();
                DataTable dt = SqlHelper.Query(sql);
                foreach (DataRow row in dt.Rows)
                {
                    returndic.Add(row["key"] + "", row["val"] + "");
                }
                return returndic;
            }
            catch
            {
                return new Dictionary<string,string>();
            }
        }

        /// <summary>
        /// 获取表外键
        /// </summary>
        /// <param name="tablecode"></param>
        /// <returns></returns>
        public List<string> GetOutKeys(string tablecode)
        {
            string outkeys = SqlHelper.Query(string.Join(" ", new string[]{
                    "select toutkey",
                    "from sys_database",
                    "where tcode = @tcode"
                }), new SqlParameter("@tcode",tablecode))
                .Tables[0].Rows[0][0] + "";

            if (string.IsNullOrEmpty(outkeys)) {
                return new List<string>();
            }
            else
            {
                return outkeys.Split(',').ToList();
            }
        }
    }
}
