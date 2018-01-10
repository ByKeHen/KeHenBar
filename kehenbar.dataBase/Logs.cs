using kehenbar.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kehenbar.DataBase
{
    public class Logs
    {
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="neirong">日志内容</param>
        /// <param name="jibie">1、普通日志 2、异常日志</param>
        public static void WriteLog(string neirong, int jibie)
        {
            FormModel fm = new FormModel();
            fm.action = "insert";
            fm.table = "sys_logs";
            fm.field = JsonConvert.SerializeObject(new
            {
                jibie = jibie,
                neirong = neirong,
                adddate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            fm.where = "";

            Thread myThread = new Thread(Run);
            myThread.Start(fm);

        }

        private static void Run(object o)
        {
            FormModel fm = (FormModel)o;
            try
            {
                new DbEntity().Index(JsonConvert.SerializeObject(fm));
            }
            catch { }
        }
    }
}
