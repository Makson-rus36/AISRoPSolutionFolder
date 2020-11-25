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

namespace WindowDeleteData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dataTable;
        public MainWindow(DataTable dataTable)
        {
            InitializeComponent();
            this.dataTable = dataTable;
            dataGrid.ItemsSource=dataTable.DefaultView;
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DeleteData deleteData = new DeleteData();
                string col = dataTable.Columns[0].ColumnName;
                string data = dataTable.Rows[0].ItemArray[0].ToString();
                deleteData.DeleteDataOnDB(dataTable.TableName, col + " = " + data);
                DialogResult = true;
            }
            catch (Exception exception)
            {
               
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
