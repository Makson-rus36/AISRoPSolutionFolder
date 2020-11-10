using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core
{
    internal interface IInteractDatabase
    {
#nullable enable
        public void DbExecuteNoReturn(object? sqlQuery);
        public object? DbExecuteWithReturn(object? sqlQuery);
#nullable disable
    }

    public class InteractionsDB : IInteractDatabase
    {
        public void DbExecuteNoReturn(object sqlQuery)
        {
            var connectDb = new ConnectDB();
            try
            {
                var sqlConnection = connectDb.GetSqlConnection;
                LoggerHelper.logger.startLog("Формирование команды к серверу: начато");
                var sqlCommand = new SqlCommand(sqlQuery.ToString(), sqlConnection);
                LoggerHelper.logger.startLog("Формирование команды к серверу: исполнение");
                var countAffectedRow = sqlCommand.ExecuteNonQuery();
                LoggerHelper.logger.startLog(string.Format(
                    "Формирование команды к серверу: исполнение (завершено успешно). Затронуто строк: {0}",
                    countAffectedRow.ToString()));
            }
            catch (Exception exception)
            {
                LoggerHelper.logger.startLog(string.Format(
                    "Во время формирования или исполнения команды к серверу БД возникла неисправимая ошибка. \n" +
                    "---------\n" +
                    "Сообщение: {0}\n" +
                    "Подробно: {1}\n" +
                    "Трассировка стека: {2}\n" +
                    "---------", exception.Message, exception.InnerException, exception.StackTrace));
            }
            finally
            {
                connectDb.Dispose();
            }
        }

        public object DbExecuteWithReturn(object sqlQuery)
        {
            return null;
        }
    }
}