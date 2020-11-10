using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace CMS.Core
{
    public class Module
    {
        /// <summary>
        ///     Класс, предоставляющий основную структуру для модулей
        /// </summary>
        public string type { get; set; }
        public string nameModule { get; set; }
        public string descriptionModule { get; set; }
        
        public string executableModuleName { get; set; }
        
        public string mainNamespace { get; set; }
        public string loaderClass { get; set; }
        
        public string nameMethodStartingModule { get; set; }

        public string versionModule { get; set; }
        public string errorModule { get; set; }

        
    }

    public class TabItemModel
    {
#nullable enable
        public object? content { get; set; }
#nullable disable
        public string header { get; set; }
    }

    public class ModulesList
    {
        public static List<Module> Modules;
        public static List<Type> Windows;
    }
    public class ContentLoader 
    {
        public ContentLoader()
        {
            ModulesList.Modules = loadXMLInfo();
            LoadModule(ModulesList.Modules);
        }
        


        /// <summary>
        ///     Класс, предназначенный для загрузки модулей
        /// </summary>
        public List<TabItemModel> GetTabItems { get; } = new List<TabItemModel>();
        public List<Module> GetPluginWindows { get; }  = new List<Module>();

        /// <summary>
        ///     Функция, для инициализации модулей
        /// </summary>
        /// <param name="modules">Список объектов типа Module</param>
        private void LoadModule(IEnumerable<Module> modules)
        {
            LoggerHelper.logger.startLog("Инициализация модулей: начато");
            foreach (var module in modules)
                try
                {
                    LoggerHelper.logger.startLog(string.Format(
                        "Инициализация модулей: загрузка модуля [название]: {0}, \n" +
                        " [описание]: {1}, \n" +
                        " [версия]: {2}, \n" +
                        " [загрузочный класс]: {3}, \n" +
                        " [загрузочный метод]: {4}\n",
                        module.nameModule,
                        module.descriptionModule,
                        module.versionModule,
                        module.loaderClass,
                        module.nameMethodStartingModule));
                    
                    Assembly assembly = Assembly.LoadFrom("Modules//net5.0-windows//" + module.executableModuleName);
                    Type objType = assembly.GetType(module.mainNamespace+"." + module.loaderClass);
                    if (objType == null) throw new NullReferenceException();
                    var type =  module.type;
                    switch (type)
                    {
                        case "System.Windows.Controls.UserControl":
                        {
                            var tabItem = new TabItemModel();
                            tabItem.content = Activator.CreateInstance(objType);
                            tabItem.header = module.nameModule;
                            GetTabItems.Add(tabItem);
                            break;
                        }
                        
                        case "System.Windows.Window":
                        {
                            GetPluginWindows.Add(module);
                            break;
                        }
                        
                        default:
                        {
                           
                            throw new Exception("Неизвестный тип данных.");
                            break;
                        }
                            
                    }
                }
                catch (Exception exception)
                {
                    module.errorModule = "Инициализация модулей: ошибка. " +exception.Message;
                    LoggerHelper.logger.startLog(String.Format("Инициализация модулей: ошибка. \n" +
                                                               "---------\n" +
                                                               "Сообщение: {0}\n" +
                                                               "Подробно: {1}\n" +
                                                               "Трассировка стека: {2}\n" +
                                                               "---------", exception.Message, exception.InnerException, exception.StackTrace));
                }
            LoggerHelper.logger.startLog("Инициализация модулей: завершено");
        }

        

        /// <summary>
        ///     Метод, загружающий информацию из XML в список объектов типа Module
        /// </summary>
        /// <returns>Возвращает список объектов типа Module</returns>
        private List<Module> loadXMLInfo()
        {
            
            var modules = new List<Module>();
            try
            {
                LoggerHelper.logger.startLog("Чтение модулей: начато");
                var xmlDocument = new XmlDocument();
                LoggerHelper.logger.startLog("Чтение модулей: чтение XML");
                xmlDocument.Load("XMLFile1.xml");
                var xmlRoot = xmlDocument.DocumentElement;
                foreach (XmlNode xmlNode in xmlRoot)
                {
                    var module = new Module();
                    foreach (XmlNode xmlItemModule in xmlNode.ChildNodes)
                    {
                        switch (xmlItemModule.Name)
                        {
                            case "Type":
                            {
                                module.type = xmlItemModule.InnerText;
                                break;
                            }
                            case "Name":
                            {
                                module.nameModule = xmlItemModule.InnerText;
                                break;
                            }
                            case "Description":
                            {
                                module.descriptionModule = xmlItemModule.InnerText;
                                break;
                            }
                            case "ExecutableModuleName":
                            {
                                module.executableModuleName = xmlItemModule.InnerText;
                                break;
                            }
                            case "MainNamespace":
                            {
                                module.mainNamespace = xmlItemModule.InnerText;
                                break;
                            }
                            case "LoaderClass":
                            {
                                module.loaderClass = xmlItemModule.InnerText;
                                break;
                            }
                            case "LoaderMethod":
                            {
                                module.nameMethodStartingModule = xmlItemModule.InnerText;
                                break;
                            }
                            case "Version":
                            {
                                module.versionModule = xmlItemModule.InnerText;
                                break;
                            }
                            
                        }
                    }

                    modules.Add(module);
                }

                LoggerHelper.logger.startLog("Чтение модулей: завершено");
            }
            catch (Exception exception)
            {
                LoggerHelper.logger.startLog(String.Format("Чтение модулей: ошибка. \n" +
                                                           "---------\n" +
                                                           "Сообщение: {0}\n" +
                                                           "Подробно: {1}\n" +
                                                           "Трассировка стека: {2}\n" +
                                                           "---------", exception.Message, exception.InnerException, exception.StackTrace));
            }

            return modules;
        }

        
    }
}