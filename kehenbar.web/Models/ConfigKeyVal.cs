using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kehenbar.web.Models
{
    public class ConfigKeyVal
    {
        public string table { get; set; }
        public string column { get; set; }

        public string colname { get; set; }
        public string yanshi { get; set; }
        public List<Keyval> keyval { get; set; }
    }

    public class Keyval 
    {
        public string jian { get; set; }
        public string zhi { get; set; }
    }
}