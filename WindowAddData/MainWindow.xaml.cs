using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CMS.Core;

namespace WindowAddData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object obj;
        private List<String> listDescription;
        public MainWindow(object obj, List<String> listDescription)
        {
            InitializeComponent();
            this.obj = obj;
            this.listDescription = listDescription;
            CreateGUI();
        }

        private void CreateGUI()
        {
            var mType = obj.GetType();
            int i = 0;
            foreach (var prop in mType.GetProperties())
            {
                try
                {
                    
                    Label label = new Label();
                    label.Content = listDescription[i];
                    i++;
                    MainPanel.Children.Add(label);
                    switch (prop.PropertyType.ToString())
                    {
                        case "System.String":
                            TextBox textBox = new TextBox();
                            MainPanel.Children.Add(textBox);
                            break;

                        case "System.DateTime":
                            DatePicker datePicker = new DatePicker();
                            datePicker.SelectedDate = DateTime.Today;
                            MainPanel.Children.Add(datePicker);
                            break;
                        case "System.Int32":
                            
                            break;
                        case "CMS.Core.ForeignKeyModel":
                            ForeignKeyModel foreignKeyModel = (ForeignKeyModel)prop.GetValue(obj);
                            ComboBox comboBox = new ComboBox();
                            InteractionsDB interactionsDb = new InteractionsDB();
                            comboBox.ItemsSource =
                                interactionsDb.DbExecuteWithReturn(String.Format("select * from {0}", foreignKeyModel.nameForeignTable));
                            comboBox.DisplayMemberPath = foreignKeyModel.nameForeignColumn;
                            
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
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (var VARIABLE in MainPanel.Children)
            {
                Type mType = VARIABLE.GetType();
                switch (mType.Name)
                {
                    case "TextBox":
                        obj.GetType().GetProperties()[i].SetValue(obj,((TextBox)VARIABLE).Text);
                        i++;
                        break;
                }
            }
            WriterData writerData = new WriterData();
            writerData.WriteInDb(obj,WriteMode.INSERTMODE,null);
            this.DialogResult = true;
        }
    }
}
