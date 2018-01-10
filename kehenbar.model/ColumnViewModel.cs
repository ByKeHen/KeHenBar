using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kehenbar.model
{
    public class ColumnViewModel
    {
        public string table { get; set; }
        public string column { get; set; }

        public string colname { get; set; }
        public string yanshi { get; set; }
        public List<ColumnViewVal> keyval { get; set; }
    }

    public class ColumnViewVal
    {
        public string jian { get; set; }
        public string zhi { get; set; }
    }
}
