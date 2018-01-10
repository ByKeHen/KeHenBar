using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kehenbar.common
{
    public class WebConfigHelper
    {
        /// <summary>
        /// 写入web.config
        /// </summary>
        /// <param name="item">appSettings等</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void WriteConfig(string item, string key, string value)
        {
            if (item == "")
            {
                item = "appSettings";
            }
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
            AppSettingsSection appSection = (AppSettingsSection)config.GetSection(item);
            if (appSection.Settings[key] == null)
            {
                appSection.Settings.Add(key, value);
                config.Save();
            }
            else
            {
                appSection.Settings.Remove(key);
                appSection.Settings.Add(key, value);
                config.Save();
            }
        }

        /// <summary>
        /// web.config中是否存在key
        /// </summary>
        /// <param name="item">appSettings等</param>
        /// <param name="key">键</param>
        public static bool HaveConfigKey(string item, string key)
        {
            if (item == "")
            {
                item = "appSettings";
            }
            Configuration config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(System.Web.HttpContext.Current.Request.ApplicationPath);
            AppSettingsSection appSection = (AppSettingsSection)config.GetSection(item);
            if (appSection.Settings[key] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        } 
    }
}
