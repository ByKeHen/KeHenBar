using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kehenbar.model
{
    public class FormModel
    {
        public string action { get; set; }
        public string table { get; set; }
        public string where { get; set; }
        public string field { get; set; }

        public string order { get; set; }
    }
}
