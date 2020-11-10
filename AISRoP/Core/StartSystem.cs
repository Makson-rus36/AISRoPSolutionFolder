using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using CMS.Core;

namespace AISRoP
{
    public class StartSystem
    {
        private static ContentLoader _contentLoader;

        public StartSystem(ItemsControl tabControl, ItemsControl menuItem)
        {
            try
            {
                LoggerHelper.logger.startLog("Запуск систем");
                _contentLoader = new ContentLoader();
                var tabItems = _contentLoader.GetTabItems;
                var pluginWindows = _contentLoader.GetPluginWindows;
                foreach (var tabItem in tabItems)
                {
                    TabItem tabItem1 = new TabItem();
                    tabItem1.Content = tabItem.content;
                    tabItem1.Header = tabItem.header;
                    tabControl.Items.Add(tabItem1);
                }
                    
                foreach (var window in pluginWindows)
                {
                    var menuItemWindow = new MenuItem();
                    menuItemWindow.Header = window.nameModule;
                    
                    menuItemWindow.Click += (sender, args) =>
                    {
                        try
                        {
                            Process.Start(Environment.CurrentDirectory+ "//Modules//net5.0-windows//" + window.executableModuleName);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            
                        }
                    };
                    menuItem.Items.Add(menuItemWindow);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Произошла неисправимая ошибка. Подробности см. лог");
                LoggerHelper.logger.startLog(String.Format("Запуск систем: ошибка. \n" +
                                                           "---------\n" +
                                                           "Сообщение: {0}\n" +
                                                           "Подробно: {1}\n" +
                                                           "Трассировка стека: {2}\n" +
                                                           "---------", exception.Message, exception.InnerException, exception.StackTrace));
            }
        }

       
    }
}