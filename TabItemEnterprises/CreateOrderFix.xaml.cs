using CMS.Core;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace TabItemEnterprises
{
    /// <summary>
    /// Логика взаимодействия для CreateOrderFix.xaml
    /// </summary>
    public partial class CreateOrderFix : Window
    {
        public CreateOrderFix()
        {
            InitializeComponent();

            DatePicker.SelectedDate = DateTime.Now;
            InteractionsDB interactions = new InteractionsDB();
            ComboBoxNumberSubdivision.ItemsSource = interactions.DbExecuteWithReturn("select code from subdivision");
            ComboBoxNumberSubdivision.DisplayMemberPath = "code";
            ComboBoxNumberSubdivision.SelectedIndex = 0;

            ComboBoxTypeRoom.ItemsSource = interactions.DbExecuteWithReturn("select code, _type_name from type_premises");
            ComboBoxTypeRoom.DisplayMemberPath = "_type_name";
            ComboBoxTypeRoom.SelectedIndex = 0;
        }

        private void BtnOK_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(FieldFloorRoom.Text) &&
                    !String.IsNullOrWhiteSpace(FieldNumberRoom.Text) &&
                    !String.IsNullOrWhiteSpace(FiledNumberBuild.Text) &&
                    ComboBoxNumberSubdivision.SelectedIndex != -1 &&
                    ComboBoxTypeRoom.SelectedIndex != -1)
                {
                    ConnectDB connectDb = new ConnectDB();
                    SqlConnection sqlConnection = new SqlConnection(connectDb.getConnectionString());
                    sqlConnection.Open();

                    string sqlQueryGetRoom =String.Format("select count(code) as countRoom " +
                                                          "from premises " +
                                                          "where number_building = {0} and number_floor = {1} and number_room = {2}",
                       FiledNumberBuild.Text, FieldFloorRoom.Text, FieldNumberRoom.Text);
                    SqlCommand sqlCommandGetRoomWithNumber = new SqlCommand(sqlQueryGetRoom, sqlConnection);
                    if (int.Parse(sqlCommandGetRoomWithNumber.ExecuteScalar().ToString()) == 0)
                    {

                        string sqlProc = "CreateOrderFixRoom";
                        SqlCommand sqlCommand = new SqlCommand(sqlProc, sqlConnection);
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@RoomNumber", 
                            Value = Int32.Parse(FieldNumberRoom.Text),
                            SqlDbType = SqlDbType.Int
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@RoomFloor", 
                            Value = int.Parse(FieldFloorRoom.Text),
                            SqlDbType = SqlDbType.Int
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@BuildNum", 
                            SqlDbType = SqlDbType.Int,
                            Value = int.Parse(FiledNumberBuild.Text)
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@CodeSubdev", 
                            SqlDbType = SqlDbType.Int,
                            Value = int.Parse(((DataRowView) ComboBoxNumberSubdivision.SelectedItem).Row.ItemArray[0]
                                .ToString())
                        });
                        string dateSelected = DatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@DataExecute", 
                            SqlDbType = SqlDbType.Date, 
                            Value = dateSelected
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@TypeRoom", 
                            SqlDbType = SqlDbType.Int,
                            Value = int.Parse(((DataRowView) ComboBoxTypeRoom.SelectedItem).Row.ItemArray[0].ToString())
                        });
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@Area",
                            SqlDbType = SqlDbType.Float,
                            Value = float.Parse(FiledAreaRoom.Text)
                        });
                        string textOrder = "";
                        sqlCommand.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@TextOrder",
                            SqlDbType = SqlDbType.VarChar, Size = 500, Value = textOrder,
                            Direction = ParameterDirection.Output
                        });
                        sqlCommand.ExecuteNonQuery();
                        MessageBox.Show(sqlCommand.Parameters["@TextOrder"].Value.ToString());
                        sqlConnection.Close();
                        DialogResult = true;
                    }
                    else
                    {
                        MessageBox.Show("Помещение с данным номер уже существует");
                        sqlConnection.Close();
                    }
                   

                }
                else
                {
                    MessageBox.Show("Вы не заполнили все поля");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
