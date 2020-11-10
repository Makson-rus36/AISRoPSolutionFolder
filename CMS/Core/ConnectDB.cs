using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Core
{

    public class ConnectDB
    {

        
        //protected string strCon = "Data Source=LAPTOP-314H63I2;Initial Catalog=DNP;Integrated Security=True";
        protected string strCon = "Data Source=DESKTOP-KB687EU;Initial Catalog=RegistrOfPremises;Integrated Security=True";
        private static SqlConnection sqlActiveConnection = null;

        public ConnectDB()
        {
            var contemp = createConnetion();
            if (contemp != null)
            {
                sqlActiveConnection = contemp;
                sqlActiveConnection.Open();
            }
        }
        private bool testConnection()
        {
            bool flag = false;
            LoggerHelper.logger.startLog("Соединение с базой данных: запрос к БД");
            try
            {
                SqlConnection sqlConnection = new SqlConnection(getConnectionString());
                sqlConnection.Open();
                LoggerHelper.logger.startLog("Соединение с базой данных: успешно");
                flag = true;
                sqlConnection.Close();
            }
            catch(Exception exception)
            {
                LoggerHelper.logger.startLog(String.Format("Соединение с базой данных: ошибка. \n" +
                                                           "---------\n" +
                                                           "Сообщение: {0}\n" +
                                                           "Подробно: {1}\n" +
                                                           "Трассировка стека: {2}\n" +
                                                           "---------", exception.Message, exception.InnerException, exception.StackTrace));
            }
            return flag;
        }

        private SqlConnection createConnetion()
        {
            if (testConnection())
            {
                return new SqlConnection(getConnectionString());
            }
            else
                return null;
        }

        private string getConnectionString()
        {
            return strCon;
        }

        public SqlConnection GetSqlConnection
        {
            get { return sqlActiveConnection; }
        }

        public void NewMethod()
        {

        }
        ~ConnectDB()
        {
            if (sqlActiveConnection!=null)
            {
                try
                {
                    LoggerHelper.logger.startLog("Закрытие активного соеденения с БД: начато");
                    sqlActiveConnection.Close();
                    LoggerHelper.logger.startLog("Закрытие активного соеденения с БД: завершено успешно");
                }
                catch (SqlException exception)
                {
                    LoggerHelper.logger.startLog(String.Format("Закрытие активного соеденения с БД: завершено c ошибками \n"+
                        "---------\n" +
                        "Сообщение: {0}\n" +
                        "Подробно: {1}\n" +
                        "Трассировка стека: {2}\n" +
                        "---------", exception.Message, exception.InnerException, exception.StackTrace));
                }
            }
        }
    }

    class MyClass2
    {
        
    }
}
