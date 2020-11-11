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
        
        class enterprises
        {
            public string fullname { get; set; }
            public ForeignKeyModel shortname { get; set; }
        }

        public MainWindow()
        {
            InitializeComponent();
            loadData();
        }

        private void loadData()
        {
            InteractionsDB interactions = new InteractionsDB();
            DataGridEnterprise.ItemsSource = interactions.DbExecuteWithReturn("select * from enterprises");
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
           enterprises ent = new enterprises();
           ent.fullname = "";
           ent.shortname = new ForeignKeyModel(){name = "", nameForeignColumn = "_type_name", nameForeignTable = "type_premises" };
           WindowAddData.MainWindow aMainWindow = new WindowAddData.MainWindow(ent, new List<string>(){"Полное название", "Краткое название"});
           if(aMainWindow.ShowDialog() == true)
               loadData();
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = DataGridEnterprise.SelectedCells;
            enterprises ent = new enterprises();
            ent.fullname = ((DataRowView)obj[0].Item).Row.ItemArray[1].ToString(); 
            //ent.shortname = ((DataRowView)obj[0].Item).Row.ItemArray[2].ToString();
            int id = int.Parse(((DataRowView)obj[0].Item).Row.ItemArray[0].ToString());
            string condition = " code = " + id;
            WindowEditData.WindowEdit windowEdit = new WindowEdit(ent, new List<string>() { "Полное название", "Краткое название" }, condition);
            if(windowEdit.ShowDialog() == true)
                loadData();
            //int a = 0;
        }
    }
}
