using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core
{
    class DeleteData
    {
        public void DeleteDataOnDB(string tableName, string condition)
        {
            string sql = String.Format("delete from {0} where {1}",tableName, condition);

        }
    }
}
