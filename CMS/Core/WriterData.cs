using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CMS.Core
{
    public class WriteMode
    {
        public const string UPDATEMODE = "update";
        public const string INSERTMODE = "insert";
    }

    public class WriterData
    {
        public void WriteInDb(object obj, string writeMode, [AllowNull] string condition)
        {
            switch (writeMode)
            {
                case WriteMode.INSERTMODE:
                    {
                        var sqlQuery = "insert into {0}({1}) values ({2})";
                        var mType = obj.GetType();
                        var nameTable = mType.Name;
                        var tableField = new List<string>();
                        var tableValues = new List<string>();
                        foreach (var prop in mType.GetProperties())
                        {
                            try
                            {


                                switch (prop.PropertyType.ToString())
                                {
                                    case "System.String":
                                        tableField.Add(prop.Name);
                                        tableValues.Add("'" + prop.GetValue(obj) + "'");
                                        break;

                                    case "System.DateTime":
                                        tableField.Add(prop.Name);
                                        tableValues.Add("'" + ((DateTime)prop.GetValue(obj)).ToString("yyyy-MM-dd") + "'");
                                        break;
                                    case "System.Int32":
                                        tableField.Add(prop.Name);
                                        tableValues.Add(prop.GetValue(obj).ToString());
                                        break;
                                    case "CMS.Core.ForeignKeyModel":
                                        tableField.Add(prop.Name);
                                        tableValues.Add(((ForeignKeyModel)prop.GetValue(obj)).name);
                                        break;
                                    default:
                                        Console.WriteLine(prop.PropertyType.ToString());
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                LoggerHelper.logger.startLog(String.Format("Во время создания запроса произошла ошибка. \n" +
                                                                           "---------\n" +
                                                                           "Сообщение: {0}\n" +
                                                                           "Подробно: {1}\n" +
                                                                           "Трассировка стека: {2}\n" +
                                                                           "---------", exception.Message, exception.InnerException, exception.StackTrace));

                            }
                        }

                        var values = string.Join(", ", tableValues);
                        var field = string.Join(", ", tableField);
                        Console.WriteLine(string.Format("Name table: {0}", mType.Name));
                        sqlQuery = string.Format(sqlQuery, nameTable, field, values);
                        InteractionsDB interactionsDb = new InteractionsDB();
                        interactionsDb.DbExecuteNoReturn(sqlQuery);
                        break;
                    }
                case WriteMode.UPDATEMODE:
                    {
                     
                        try
                        {
                            var sqlQuery = "update {0} set  {1}";
                            if (condition != null)
                                sqlQuery += " where " + condition;
                            var mType = obj.GetType();
                            var nameTable = mType.Name;
                            var dictionaryValues = new Dictionary<string, string>();
                            foreach (var prop in mType.GetProperties())
                                switch (prop.PropertyType.ToString())
                                {
                                    case "System.String":
                                        dictionaryValues[prop.Name] = "'" + prop.GetValue(obj) + "'";
                                        break;

                                    case "System.DateTime":
                                        dictionaryValues[prop.Name] =
                                            "'" + ((DateTime) prop.GetValue(obj)).ToString("yyyy-MM-dd") + "'";
                                        break;
                                    case "System.Int32":
                                        dictionaryValues[prop.Name] = prop.GetValue(obj).ToString();
                                        break;
                                    case "CMS.Core.ForeignKeyModel":
                                        dictionaryValues[prop.Name] = ((ForeignKeyModel) prop.GetValue(obj)).name;
                                        break;
                                    default:
                                        Console.WriteLine(prop.PropertyType.ToString());
                                        break;
                                }

                            var listSetValues = new List<string>();
                            foreach (var VARIABLE in dictionaryValues)
                                listSetValues.Add(string.Format("{0} = {1}", VARIABLE.Key, VARIABLE.Value));
                            sqlQuery = string.Format(sqlQuery, nameTable, string.Join(", ", listSetValues));
                            Console.WriteLine(string.Format("Name table: {0}", mType.Name));
                            InteractionsDB interactionsDb = new InteractionsDB();
                            interactionsDb.DbExecuteNoReturn(sqlQuery);
                        }
                        catch (Exception exception)
                        {
                            LoggerHelper.logger.startLog(String.Format("Во время создания запроса произошла ошибка. \n" +
                                                                       "---------\n" +
                                                                       "Сообщение: {0}\n" +
                                                                       "Подробно: {1}\n" +
                                                                       "Трассировка стека: {2}\n" +
                                                                       "---------", exception.Message, exception.InnerException, exception.StackTrace));
                        }
                        break;
                    }
            }
        }
    }
}
