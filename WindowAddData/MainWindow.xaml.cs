using System;
using System.Collections.Generic;
using System.Data;
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
                    label.FontSize = 14;
                    label.Content = listDescription[i];
                    i++;
                    MainPanel.Children.Add(label);
                    switch (prop.PropertyType.ToString())
                    {
                        case "System.String":
                            TextBox textBox = new TextBox();
                            textBox.FontSize = 14;
                            MainPanel.Children.Add(textBox);
                            break;

                        case "System.DateTime":
                            DatePicker datePicker = new DatePicker();
                            datePicker.SelectedDate = DateTime.Today;
                            MainPanel.Children.Add(datePicker);
                            break;
                        case "System.Int32":
                            TextBox textBoxnum = new TextBox();
                            textBoxnum.FontSize = 14;
                            MainPanel.Children.Add(textBoxnum);
                            break;
                        case "CMS.Core.ForeignKeyModel":
                            ForeignKeyModel foreignKeyModel = (ForeignKeyModel)prop.GetValue(obj);
                            ComboBox comboBox = new ComboBox();
                            comboBox.FontSize = 14;
                            InteractionsDB interactionsDb = new InteractionsDB();
                            comboBox.ItemsSource =
                                interactionsDb.DbExecuteWithReturn(String.Format("select * from {0}", foreignKeyModel.nameForeignTable));
                            comboBox.DisplayMemberPath = foreignKeyModel.nameForeignColumn;
                            MainPanel.Children.Add(comboBox);
                            
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
                        if(obj.GetType().GetProperties()[i].GetValue(obj)==null)
                         obj.GetType().GetProperties()[i].SetValue(obj,((TextBox)VARIABLE).Text);
                        i++;
                        break;
                    case "ComboBox":
                        var value = ((DataRowView) ((ComboBox) VARIABLE).SelectedItem).Row.ItemArray[0].ToString();
                        ForeignKeyModel foreignKeyModel = new ForeignKeyModel()
                        {
                            name=value,
                            nameForeignColumn = ((ForeignKeyModel)obj.GetType().GetProperties()[i].GetValue(obj)).nameForeignColumn,
                            nameForeignTable = ((ForeignKeyModel)obj.GetType().GetProperties()[i].GetValue(obj)).nameForeignTable
                        };
                        obj.GetType().GetProperties()[i].SetValue(obj, foreignKeyModel);
                        break;
                }
            }
            WriterData writerData = new WriterData();
            writerData.WriteInDb(obj,WriteMode.INSERTMODE,null);
            this.DialogResult = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
