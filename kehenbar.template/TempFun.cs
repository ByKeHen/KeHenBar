using kehenbar.Template.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace kehenbar.Template
{
    public class TempFun
    {
        string content = string.Empty;
        TempConfig config;
        TempLanguage language;
        public TempFun()
        {
            config = new TempConfig();
            language = new TempLanguage();
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool Isnum(string o)
        {
            try
            {
                int.Parse(o);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }      

        /// <summary>
        /// 获取模版内容
        /// </summary>
        /// <param name="tempath">模版路径</param>
        /// <returns></returns>
        public string GetTemplate(string tempath)
        { 
            tempath = System.Web.HttpContext.Current.Server.MapPath(tempath);
            if (!File.Exists(tempath))
            {
                Alert(language.no_temp_file);
                return content = "";
                
            }

            using (StreamReader sr = new StreamReader(tempath,System.Text.Encoding.Default))
            {
                content = sr.ReadToEnd();
                sr.Close();
            }

            return content;
        }

        public Dictionary<string, string> GetFields(string str)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            
            string[] filedarr = str.Split(' ');
            foreach (var it in filedarr)
            {
                if (it.Contains('='))
                {
                    dic.Add(it.Split('=')[0], it.Split('=')[1]);
                }
            } 
                        
            return dic;
        }

        public Dictionary<string, FieldForIf> GetFieldsForIf(string str)
        {
            Dictionary<string, FieldForIf> dic = new Dictionary<string, FieldForIf>();

            string[] filedarr = str.Split(' ');
            foreach (var it in filedarr)
            {
                FieldForIf fieldforif = new FieldForIf();
                if (it.Contains('='))
                {
                    fieldforif.leftval = it.Split('=')[0];
                    fieldforif.rightval = it.Split('=')[1];

                    dic.Add("=", fieldforif);
                }
                else if (it.Contains("<>"))
                {
                    fieldforif.leftval = Regex.Split(it, "<>")[0];
                    fieldforif.rightval = Regex.Split(it, "<>")[1];

                    dic.Add("!=", fieldforif);
                }
                else if (it.Contains(">"))
                {
                    fieldforif.leftval = it.Split('>')[0];
                    fieldforif.rightval = it.Split('>')[1];

                    dic.Add(">", fieldforif);
                }
                else if (it.Contains("<"))
                {
                    fieldforif.leftval = it.Split('<')[0];
                    fieldforif.rightval = it.Split('<')[1];

                    dic.Add("<", fieldforif);
                }
            }

            return dic;
        }

        /// <summary>
        /// 弹框提示
        /// </summary>
        /// <param name="msg"></param>
        public void Alert(string msg)
        {
            Echo("<script type='text/javascript'>");
            Echo("alert('"+msg+"');");            
            Echo("</script>");

        }

        /// <summary>
        /// 输出信息
        /// </summary>
        /// <param name="msg"></param>
        public void Echo(string msg)
        {
            System.Web.HttpContext.Current.Response.Write(msg);
        }

        /// <summary>
        /// 输出信息并停止
        /// </summary>
        /// <param name="msg"></param>
        public void Die(string msg)
        {
            System.Web.HttpContext.Current.Response.Write(msg);
            System.Web.HttpContext.Current.Response.End();
        }
    }
}
