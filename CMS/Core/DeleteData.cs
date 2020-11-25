using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core
{
    public class DeleteData
    {
        public void DeleteDataOnDB(string tableName, string condition)
        {
            try
            {
                LoggerHelper.logger.startLog("Запрос на удаление принят: формирование");
                string sql = String.Format("delete from {0} where {1}", tableName, condition);
                LoggerHelper.logger.startLog("Запрос на удаление принят: исполнение: "+sql);
                InteractionsDB interactions = new InteractionsDB();
                interactions.DbExecuteNoReturn(sql);
            }
            catch (Exception exception)
            {
                LoggerHelper.logger.startLog(String.Format("Во время выполенения запроса произошла ошибка. \n" +
                                                           "---------\n" +
                                                           "Сообщение: {0}\n" +
                                                           "Подробно: {1}\n" +
                                                           "Трассировка стека: {2}\n" +
                                                           "---------", exception.Message, exception.InnerException, exception.StackTrace));
            }
        }
    }
}
