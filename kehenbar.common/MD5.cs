using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kehenbar.common
{
    public class MD5
    {
        /// <summary>
        /// 作为密码方式加密字符串
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string md5(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
        }
    }
}
