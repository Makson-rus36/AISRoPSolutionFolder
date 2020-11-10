using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using CMS.Core;
using Module = CMS.Core.Module;

namespace AISRoP.UI
{
    
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            try
            {
                InitializeComponent();
                var list = loadXMLInfo();
                theListView.ItemsSource = ModulesList.Modules;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        
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
                        if (xmlItemModule.Name == "Name")
                            module.nameModule = xmlItemModule.InnerText;
                        if (xmlItemModule.Name == "Description")
                            module.descriptionModule=(xmlItemModule.InnerText);
                        if (xmlItemModule.Name == "LoaderClass")
                            module.loaderClass=(xmlItemModule.InnerText);
                        if (xmlItemModule.Name == "LoaderMethod")
                            module.nameMethodStartingModule=(xmlItemModule.InnerText);
                        if (xmlItemModule.Name == "Version")
                            module.versionModule=(xmlItemModule.InnerText);
                    }

                    modules.Add(module);
                }

                LoggerHelper.logger.startLog("Чтение модулей: завершено");
            }
            catch (Exception ex)
            {
                LoggerHelper.logger.startLog("Чтение модулей: ошибка. " + ex.Message);
            }

            return modules;
        }
        



    }
}