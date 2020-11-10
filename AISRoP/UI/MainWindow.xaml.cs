using System;
using System.Windows;
using CMS.Core;


namespace AISRoP.UI
{
    /// <summary>
    ///     Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
            new StartSystem(MainContentTable, MenuPlugin);
        }

        

        private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
        {
            new AboutProgram().Show();
        }

        private void MenuItemSettings_OnClick(object sender, RoutedEventArgs e)
        {
            (new SettingsWindow()).Show();
        }
    }
}