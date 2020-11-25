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
using WindowAddData;
using WindowEditData;

namespace TabItemEnterprises
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UserControl
    {

        class subdivision
        {
            public string fullname { get; set; }
            public string shortname { get; set; }
            public string dative_name { get; set; }
            public string genetive_name { get; set; }
            public Int32  code_enterprise { get; set; }
            public ForeignKeyModel code_maindivision { get; set; }
        }

        public class enterprises
        {
            public string fullname { get; set; }
            public string shortname { get; set; }

        }

        public MainWindow()
        {
            InitializeComponent();
            loadData();
        }

        private void loadData()
        {
            InteractionsDB interactions = new InteractionsDB();
            DataGridEnterprise.ItemsSource = interactions.DbExecuteWithReturn("select code,fullname, shortname from enterprises");
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
           enterprises ent = new enterprises();
           ent.fullname = "";
           ent.shortname = "";
           //ent.shortname = new ForeignKeyModel(){name = "", nameForeignColumn = "_type_name", nameForeignTable = "type_premises" };
           WindowAddData.MainWindow aMainWindow = new WindowAddData.MainWindow(ent, new List<string>(){"Полное название", "Краткое название"});
           if(aMainWindow.ShowDialog() == true)
               loadData();
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = DataGridEnterprise.SelectedCells;
            enterprises ent = new enterprises();
            ent.fullname = ((DataRowView)obj[0].Item).Row.ItemArray[1].ToString();
            //WARNING! DO NOT UNCOMMIT
            //ent.shortname = new ForeignKeyModel() { name = ((DataRowView)obj[0].Item).Row.ItemArray[2].ToString(), nameForeignColumn = "_type_name", nameForeignTable = "type_premises" };
            ent.shortname = ((DataRowView)obj[0].Item).Row.ItemArray[2].ToString(); 
            int id = int.Parse(((DataRowView)obj[0].Item).Row.ItemArray[0].ToString());
            string condition = " code = " + id;
            WindowEdit windowEdit = new WindowEdit(ent, new List<string>() { "Полное название", "Краткое название" }, condition);
            if(windowEdit.ShowDialog() == true)
                loadData();
            
        }

        private void BtnDel_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGridEnterprise.SelectedCells.Count != 0)
            {
                var obj = DataGridEnterprise.SelectedCells;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("Code", typeof(int)));
                dataTable.Columns.Add(new DataColumn("Код", typeof(int)));
                dataTable.Columns.Add(new DataColumn("Краткое название", typeof(string)));
                dataTable.Columns.Add(new DataColumn("Полное название", typeof(string)));
                DataRow dataRow = dataTable.NewRow();
                dataRow["Code"]= int.Parse(((DataRowView)obj[0].Item).Row.ItemArray[0].ToString());
                dataRow["Код"]= int.Parse(((DataRowView)obj[0].Item).Row.ItemArray[0].ToString());
                dataRow["Полное название"] = ((DataRowView)obj[0].Item).Row.ItemArray[1].ToString();
                dataRow["Краткое название"] = ((DataRowView)obj[0].Item).Row.ItemArray[2].ToString();
                dataTable.Rows.Add(dataRow);
                dataTable.TableName = "enterprises";
                WindowDeleteData.MainWindow windowDeleteData = new WindowDeleteData.MainWindow(dataTable);
                windowDeleteData.ShowDialog();
                loadData();
            }
        }

        private void BtnAddDivision_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnEditDivision_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnDelDivision_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DataGridEnterprise_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
           
        }
    }
}
