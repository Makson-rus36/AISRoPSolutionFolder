using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core
{
    interface IInteractDatabase
    {
#nullable enable
        public void DbExecuteNoReturn(object? sqlQuery);
        public object? DbExecuteWithReturn(object? sqlQuery);
#nullable disable
    }
    class InteractionsDB:IInteractDatabase
    {
        public void DbExecuteNoReturn(object sqlQuery)
        {
            try
            {
                ConnectDB connectDb = new ConnectDB();
                SqlConnection sqlConnection = connectDb.GetSqlConnection;
                LoggerHelper.logger.startLog("Формирование команды к серверу: начато");
                SqlCommand sqlCommand = new SqlCommand(sqlQuery.ToString(), sqlConnection);
                if (sqlCommand != null)
                {
                    int countAffectedRow = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.logger.startLog(String.Format("Во время формирования или исполнения команды к серверу БД возникла неисправимая ошибка. \n" +
                                             "---------\n" +
                                             "Сообщение: {0}\n" +
                                             "Подробно: {1}\n" +
                                             "Трассировка стека: {2}\n" +
                                             "---------", exception.Message,exception.InnerException, exception.StackTrace));
            }
        }

        public object DbExecuteWithReturn(object sqlQuery)
        {
            return null;
        }
    }
}
