using PowerSystemLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Entity
{
    public class AjaxPost
    {
        public List<int> IDList { get; set; }

        public Role Role { get; set; }

        public string Code { get; set; }
    }
}
