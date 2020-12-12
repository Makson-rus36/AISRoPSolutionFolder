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
            interactions = new InteractionsDB();
            ComboBoxSubdiv.ItemsSource =
                interactions.DbExecuteWithReturn("select code from subdivision where code_enterprise = " + code_ent);
            ComboBoxSubdiv.DisplayMemberPath = "code";
            ComboBoxSubdiv.SelectedIndex = 0;
        }

        private void loadRooms(int code_ent)
        {
            InteractionsDB interactions = new InteractionsDB();
            dataGridRooms.ItemsSource = interactions.DbExecuteWithReturn(
                "select distinct premises.code, concat_ws('', premises.number_building,premises.number_floor, premises.number_room ) as numberRoom, " +
                "(select _type_name from type_premises where premises._type = type_premises.code) as typeRoom, number_subdivision, Area " +
                " from premises, subdivision " +
                " where subdivision.code_enterprise = " + code_ent);
        }

        private void loadOrders(int code_ent)
        {
            InteractionsDB interactionsDb = new InteractionsDB();
            string sql =
                "select distinct orders.code, (select _type_name from type_orders where type_orders.code = orders._type) as typeOrder, date_order, number_order, new_subdivision " +
                " from orders, subdivision where subdivision.code_enterprise = " + code_ent;
            dataGridOrder.ItemsSource = interactionsDb.DbExecuteWithReturn(sql);
        }

        private void loadQuest1(string date)
        {
            string sql =String.Format(
                "select(select concat_ws('', premises.number_building, premises.number_floor, premises.number_room) from premises where premises.code = history_room_change.code_room) as numberRoom, " +
                " (select(select _type_name from type_premises where type_premises.code = premises._type) from premises where premises.code = history_room_change.code_room) as typeRoom, " +
                " (select fullname from subdivision where code = (select new_subdivision from orders where orders.number_order = history_room_change.code_order_fix) ) as nameSubdev " +
                " from history_room_change where (date_fix <= '{0}' and date_unfix > '{1}') or(date_fix <= '{2}' and date_unfix is null)", date,date,date);
            InteractionsDB interactionsDb = new InteractionsDB();
            dataGridHistory.ItemsSource = interactionsDb.DbExecuteWithReturn(sql);
        }

        private void loadQuest3(String year, string code_subdiv)
        {

            string sql = String.Format("declare @DateSt date, @DateEnd date, @NumberSubdivision int " +
                         " set @DateSt = '{0}-01-01' " +
                         " set @DateEnd = '{0}-12-31' " +
                         " set @NumberSubdivision = {1} " +
                         " select(select(select Area from premises where premises.code = room_in_orders.code_room) from room_in_orders where room_in_orders.code_order = orders.number_order) as SquareRoom, " +
                         " (select case when new_subdivision = @NumberSubdivision then 'прибавилось' when old_subdivision = @NumberSubdivision then 'убавилось' end) as ActionOnRoom, " +
                         " date_order as DateAction " +
                         " from orders " +
                         " where old_subdivision = @NumberSubdivision and date_order between @DateSt and @DateEnd or new_subdivision = @NumberSubdivision and date_order between @DateSt and @DateEnd", year, code_subdiv);

            InteractionsDB interactionsDb = new InteractionsDB();
            DataGridDinamics.ItemsSource = interactionsDb.DbExecuteWithReturn(sql);

        }

        private void loadQuest2(string code_ent)
        {
            TreeViewDepencity.Items.Clear();
            string sql = "select fullname, code from subdivision where code_maindivision is null and code_enterprise = " + code_ent;
            InteractionsDB interactionsDb = new InteractionsDB();
            DataTable dataTable = interactionsDb.DbExecuteWithReturn(sql).Table;

            foreach (DataRow VARIABLE in dataTable.Rows)
            {
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = VARIABLE.ItemArray[0].ToString();
                FindUsageAllDepenctity(treeViewItem, VARIABLE.ItemArray[1].ToString(), code_ent);
                TreeViewDepencity.Items.Add(treeViewItem);
            }

        }

        private void FindUsageAllDepenctity(TreeViewItem treeViewItem, string code_division, string code_ent)
        {
            string sql = String.Format("select fullname, code from subdivision where code_maindivision = {0} and code_enterprise = {1}", code_division, code_ent);
            InteractionsDB interactionsDb = new InteractionsDB();
            DataTable dataTable = interactionsDb.DbExecuteWithReturn(sql).Table;
            foreach (DataRow VARIABLE in dataTable.Rows)
            {
                TreeViewItem TreeViewItem2 = new TreeViewItem();
                TreeViewItem2.Header = VARIABLE.ItemArray[0].ToString();
                treeViewItem.Items.Add(TreeViewItem2);
                FindUsageAllDepenctity(TreeViewItem2, VARIABLE.ItemArray[1].ToString(), code_ent);
            }
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
            try
            {
                var obj = DataGridDivision.SelectedCells;
                string sql = "delete from subdivision where code = " +
                             (((DataRowView) obj[0].Item).Row.ItemArray[0].ToString());
                InteractionsDB interactionsDb = new InteractionsDB();
                interactionsDb.DbExecuteNoReturn(sql);
                obj = DataGridEnterprise.SelectedCells;
                code = int.Parse(((DataRowView) obj[0].Item).Row.ItemArray[0].ToString());
                loadSubdivision(code);
            }
            catch (Exception exception)
            {
                // MessageBox.Show(exception.Message);
            }

        }

        private void DataGridEnterprise_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var obj = DataGridEnterprise.SelectedCells;
                code = int.Parse(((DataRowView) obj[0].Item).Row.ItemArray[0].ToString());
                loadSubdivision(code);
                loadRooms(code);
                loadOrders(code);
                loadQuest2(code.ToString());
            }
            catch (Exception exception)
            {
                // MessageBox.Show(exception.Message);
            }
        }

        private void BtnAddRoom_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateOrderFix createOrder = new CreateOrderFix();
                if(createOrder.ShowDialog()==true)
                    loadRooms(code);
                loadOrders(code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEditRoom_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateOrderChangeFixRoom createOrder = new CreateOrderChangeFixRoom();
                if (createOrder.ShowDialog() == true)
                    loadRooms(code);
                loadOrders(code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DataGridOrder_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dataGridOrder.SelectedCells != null)
                {
                    var obj = dataGridOrder.SelectedCells;
                    string sql = "select text_order from orders where code = " +
                                 ((DataRowView) obj[0].Item).Row.ItemArray[0].ToString();
                    InteractionsDB interactionsDb = new InteractionsDB();
                    DataTable dataTable = interactionsDb.DbExecuteWithReturn(sql).Table;
                    TextBlockOrders.Text = dataTable.Rows[0].ItemArray[0].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DatePickerDateHistory_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DatePickerDateHistory.SelectedDate != null)
            {
                string dateSelected = DatePickerDateHistory.SelectedDate.Value.ToString("yyyy-MM-dd");
                loadQuest1(dateSelected);
            }
        }

        private void ButtonShowSq_OnClick(object sender, RoutedEventArgs e)
        {
            int date;
            if (!String.IsNullOrWhiteSpace(FieldDateDinamic.Text) && int.TryParse(FieldDateDinamic.Text, out date) && ComboBoxSubdiv.SelectedIndex!=-1)
            {
                loadQuest3(date.ToString(), ((DataRowView)ComboBoxSubdiv.SelectedItem).Row.ItemArray[0].ToString());
            }
        }
    }
}
