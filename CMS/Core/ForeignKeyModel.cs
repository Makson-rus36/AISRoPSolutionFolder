using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core
{
    public class ForeignKeyModel
    {
        public string name { get; set; }
        public string nameForeignTable { get; set; }
        public string nameForeignColumn { get; set; }
    }
}
