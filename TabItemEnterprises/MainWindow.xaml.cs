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
        private int code;
        public class subdivision
        {
            public string fullname { get; set; }
            public string shortname { get; set; }
            public string dative_name { get; set; }
            public string genitive_name { get; set; }
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
            DataGridEnterprise.SelectedIndex = 0;
        }

        private void loadSubdivision(int code_ent)
        {
            InteractionsDB interactions = new InteractionsDB();
            DataGridDivision.ItemsSource =
                interactions.DbExecuteWithReturn("select code,fullname, shortname, " +
                                                 "(select case when code_maindivision IS NULL then 'предприятие' " +
                                                 "else (select s.fullname " +
                                                 "from subdivision as s " +
                                                 "where s.code = sm.code_maindivision)end)  as maindiv " +
                                                 "from subdivision as sm " +
                                                 "where code_enterprise = "+code_ent);
        }

        private DataView loadOneSubdivision(int code)
        {
            InteractionsDB interactions = new InteractionsDB();
            return interactions.DbExecuteWithReturn("select * from subdivision where code=" + code);
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
            var obj = DataGridEnterprise.SelectedCells;
            subdivision subdev = new subdivision();
            subdev.fullname = "";
            subdev.shortname = "";
            subdev.dative_name = "";
            subdev.genitive_name = "";
            subdev.code_enterprise = int.Parse(((DataRowView) obj[0].Item).Row.ItemArray[0].ToString());
            subdev.code_maindivision = new ForeignKeyModel(){name = "",nameForeignColumn = "fullname", nameForeignTable = "subdivision" };
            WindowAddData.MainWindow aMainWindow = new WindowAddData.MainWindow(subdev, 
                new List<string>() { "Полное название", 
                                     "Краткое название",
                "дательный падеж",
                "родительный падеж",
                "код предприятия",
                "главное подразделение (если есть)"
                });
            if (aMainWindow.ShowDialog() == true)
                loadData();
        }

        private void BtnEditDivision_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataGridDivision.SelectedCells != null)
            {
                try
                {
                    var obj = DataGridDivision.SelectedCells;
                    int id = int.Parse(((DataRowView) obj[0].Item).Row.ItemArray[0].ToString());
                    DataView dv = loadOneSubdivision(id);
                    subdivision sb = new subdivision();
                    sb.genitive_name = dv.Table.Rows[0].ItemArray[4].ToString();
                    sb.shortname = dv.Table.Rows[0].ItemArray[2].ToString();
                    sb.code_maindivision = new ForeignKeyModel(){name = dv.Table.Rows[0].ItemArray[6].ToString() , nameForeignColumn = "fullname", nameForeignTable = "subdivision" };
                    sb.dative_name = dv.Table.Rows[0].ItemArray[3].ToString();
                    sb.fullname = dv.Table.Rows[0].ItemArray[1].ToString();
                    sb.code_enterprise = Int32.Parse(dv.Table.Rows[0].ItemArray[5].ToString());
                    string condition = " code = " + id;
                    WindowEdit windowEdit = new WindowEdit(sb, new List<string>() { "Полное название", "Краткое название","Дательный падеж", "Родительный падеж","Код предприятия", "Код главного подразделения" }, condition);
                    if (windowEdit.ShowDialog() == true)
                        loadSubdivision(code);
                }
                catch (NullReferenceException exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void BtnDelDivision_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DataGridEnterprise_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var obj = DataGridEnterprise.SelectedCells;
                code = int.Parse(((DataRowView) obj[0].Item).Row.ItemArray[0].ToString());
                loadSubdivision(code);
            }
            catch (Exception exception)
            {
                // MessageBox.Show(exception.Message);
            }
        }
    }
}
