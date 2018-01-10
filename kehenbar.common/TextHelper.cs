using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kehenbar.common
{
    public class TextHelper
    {
        public static string GetContet(string filepath)
        {             
            string tempath = System.Web.HttpContext.Current.Server.MapPath(filepath);
            if (!File.Exists(tempath))
            {

                return JsonConvert.SerializeObject(new { 
                    code=1,
                    msg="没找到文件"
                });
            }

            using (StreamReader sr = new StreamReader(tempath, System.Text.Encoding.Default))
            {
                string content = sr.ReadToEnd();
                sr.Close();

                return JsonConvert.SerializeObject(new
                {
                    code = 0,
                    msg = content
                });
            }
        }
    }
}
