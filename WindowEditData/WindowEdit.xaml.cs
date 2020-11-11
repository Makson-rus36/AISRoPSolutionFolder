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

namespace WindowEditData
{
    /// <summary>
    /// Interaction logic for WindowEdit.xaml
    /// </summary>
    
        public partial class WindowEdit : Window
        {
            private object obj;
            private List<String> listDescription;
            private string condition;
            public WindowEdit(object obj, List<String> listDescription, string condition)
            {
                InitializeComponent();
                this.obj = obj;
                this.listDescription = listDescription;
                this.condition = condition;
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
                        
                        MainPanel.Children.Add(label);
                        switch (prop.PropertyType.ToString())
                        {
                            case "System.String":
                                TextBox textBox = new TextBox();
                                textBox.Text = prop.GetValue(obj).ToString();
                                MainPanel.Children.Add(textBox);
                                break;

                            case "System.DateTime":
                                DatePicker datePicker = new DatePicker();
                                datePicker.SelectedDate = (DateTime)prop.GetValue(obj);
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
                                DataTable dataTable = 
                                    interactionsDb.DbExecuteWithReturn(String.Format("select * from {0}", foreignKeyModel.nameForeignTable)).ToTable();
                                int index = 0;
                                foreach (DataRowView VARIABLE in comboBox.Items)
                                {
                                    if (VARIABLE.Row.ItemArray[0].ToString() == foreignKeyModel.name)
                                        comboBox.SelectedIndex = index;
                                    index++;
                                }
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

                    i++;
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
                            obj.GetType().GetProperties()[i].SetValue(obj, ((TextBox)VARIABLE).Text);
                            i++;
                            break;
                        case "ComboBox":
                            var value = ((DataRowView)((ComboBox)VARIABLE).SelectedItem).Row.ItemArray[0].ToString();
                            ForeignKeyModel foreignKeyModel = new ForeignKeyModel()
                            {
                                name = value,
                                nameForeignColumn = ((ForeignKeyModel)obj.GetType().GetProperties()[i].GetValue(obj)).nameForeignColumn,
                                nameForeignTable = ((ForeignKeyModel)obj.GetType().GetProperties()[i].GetValue(obj)).nameForeignTable
                            };
                            obj.GetType().GetProperties()[i].SetValue(obj, foreignKeyModel);
                            break;
                }
                }
                WriterData writerData = new WriterData();
                writerData.WriteInDb(obj, WriteMode.UPDATEMODE, condition);
                this.DialogResult = true;
            }
        }
}

