using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using CMS.Core;

namespace TabItemEnterprises
{
    class ModelRoom
    {
        public int code { get; set; }
        public string numberRoom { get; set; }
        public string typeRoom { get; set; }

        public float Area { get; set; }
    }

    /// <summary>
    /// Логика взаимодействия для CreateOrderChangeFixRoom.xaml
    /// </summary>
    public partial class CreateOrderChangeFixRoom : Window
    {
        private List<ModelRoom> _roomsOldList;
        private List<ModelRoom> _roomsNewList;
        public CreateOrderChangeFixRoom()
        {
            InitializeComponent();
            _roomsOldList = new List<ModelRoom>();
            DatePicker.SelectedDate= DateTime.Now;
            _roomsNewList = new List<ModelRoom>();
            string sqlQueryLoadSubdivision = "select code from subdivision";
            InteractionsDB interactionsDb = new InteractionsDB();
            ComboBoxOldSubdivision.ItemsSource = interactionsDb.DbExecuteWithReturn(sqlQueryLoadSubdivision);
            ComboBoxOldSubdivision.DisplayMemberPath = "code";
            ComboBoxOldSubdivision.SelectedIndex = 0;

        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGridNew.Items.Count > 0)
            {
                try
                {
                    string insertData = "";
                    foreach (var VARIABLE in _roomsNewList)
                    {
                        insertData += String.Format(" insert into @Data(CodeRoom) values ({0}) ", VARIABLE.code);
                    }
                    string dateSelected = DatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                    string sql = String.Format("declare @Data ArrayRoom, @TextRes varchar(500)" +
                                               " {0} " +
                                               " " +
                                               "exec CreateOrderChangeFixSubdev {1}, {2}, @Data, '{3}', @TextRes", 
                        insertData, 
                        ((DataRowView)ComboBoxOldSubdivision.SelectedItem).Row.ItemArray[0].ToString(),
                        ((DataRowView)ComboBoxNewSubdivision.SelectedItem).Row.ItemArray[0].ToString(),
                        dateSelected);

                    ConnectDB connectDb = new ConnectDB();
                    SqlConnection sqlConnection = new SqlConnection(connectDb.getConnectionString());
                    sqlConnection.Open();
                    SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                    DialogResult = true;
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private void ComboBoxOldSubdivision_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _roomsOldList.Clear();
                dataGridOld.ItemsSource = null;
                InteractionsDB interactionsDb = new InteractionsDB();
                ComboBoxNewSubdivision.ItemsSource = interactionsDb.DbExecuteWithReturn(
                    "select code from subdivision where code != " +
                    ((DataRowView) ComboBoxOldSubdivision.SelectedItem).Row.ItemArray[0]);
                ComboBoxNewSubdivision.DisplayMemberPath = "code";
                ComboBoxNewSubdivision.SelectedIndex = 0;
                string sqlGetRoom = String.Format(
                    "select code, concat_ws('', number_building, number_floor, number_room ) as numberRoom, " +
                    "(select _type_name from type_premises where premises._type = type_premises.code) as typeRoom, Area " +
                    "from premises where number_subdivision = {0}",
                    ((DataRowView) ComboBoxOldSubdivision.SelectedItem).Row.ItemArray[0]);
                DataTable dataTable = interactionsDb.DbExecuteWithReturn(sqlGetRoom).Table;
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    _roomsOldList.Add(new ModelRoom()
                    {
                        code = int.Parse(dataTable.Rows[i].ItemArray[0].ToString()),
                        numberRoom = dataTable.Rows[i].ItemArray[1].ToString(),
                        typeRoom = dataTable.Rows[i].ItemArray[2].ToString(),
                        Area = float.Parse(dataTable.Rows[i].ItemArray[3].ToString())
                    });
                }

                dataGridOld.ItemsSource = _roomsOldList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonDeleteRoom_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGridNew.SelectedItem != null)
            {
                _roomsNewList.Remove((ModelRoom) dataGridNew.SelectedItem);
                _roomsOldList.Add((ModelRoom) dataGridNew.SelectedItem);

                dataGridOld.ItemsSource = null;
                dataGridOld.ItemsSource = _roomsOldList;

                dataGridNew.ItemsSource = null;
                dataGridNew.ItemsSource = _roomsNewList;
            }
        }

        private void ButtonAddRoom_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataGridOld.SelectedItem != null)
            {
                _roomsNewList.Add((ModelRoom) dataGridOld.SelectedItem);
                _roomsOldList.Remove((ModelRoom) dataGridOld.SelectedItem);

                dataGridOld.ItemsSource = null;
                dataGridOld.ItemsSource = _roomsOldList;

                dataGridNew.ItemsSource = null;
                dataGridNew.ItemsSource = _roomsNewList;
            }
        }
    }
}
