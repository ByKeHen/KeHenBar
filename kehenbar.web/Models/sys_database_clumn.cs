using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kehenbar.web.Models
{
    public class sys_database_clumn
    {
        public int id { get; set; }
        public int sys_database_id { get; set; }
        public string cname { get; set; }
        public string ccode { get; set; }
        public int clength { get; set; }
        public string ctype { get; set; }
        public string cmark { get; set; }
        public int order { get; set; }
        public int show { get; set; }
        public string showtype { get; set; }
        public int kuan { get; set; }
        public int tonghang { get; set; }

        public int sys_database_clumnid { get; set; }
        public int sys_database_clumnsys_database_id { get; set; }
        public string sys_database_clumncname { get; set; }
        public string sys_database_clumnccode { get; set; }
        public int sys_database_clumnclength { get; set; }
        public string sys_database_clumnctype { get; set; }
        public string sys_database_clumncmark { get; set; }
        public int sys_database_clumnorder { get; set; }
        public int sys_database_clumnshow { get; set; }
        public string sys_database_clumnshowtype { get; set; }
        public string sys_database_clumntonghang { get; set; }
        public string sys_database_clumnkuan { get; set; }
        public int sys_databaseid { get; set; }
        public int sys_databasettype { get; set; }
        public string sys_databasetcode { get; set; }
        public string sys_databasetname { get; set; }
        public string sys_databasetoutkey { get; set; }
    }
}