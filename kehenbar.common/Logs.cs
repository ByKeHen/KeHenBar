using kehenbar.DataBase;
using kehenbar.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kehenbar.common
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

            kehenbar.DataBase.Logs.WriteLog(neirong, jibie);
        }
    }
}
