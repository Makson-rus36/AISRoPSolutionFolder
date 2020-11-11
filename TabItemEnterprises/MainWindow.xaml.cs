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
            DataGridEnterprise.ItemsSource = interactions.DbExecuteWithReturn("select * from enterprises");
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
           enterprises ent = new enterprises();
           ent.fullname = "";
           ent.shortname = "";
           WindowAddData.MainWindow aMainWindow = new WindowAddData.MainWindow(ent, new List<string>(){"Полное название", "Краткое название"});
           if(aMainWindow.ShowDialog() == true)
               loadData();
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = (DataRowView)DataGridEnterprise.SelectedItem;
            enterprises ent = new enterprises();
            ent.fullname = obj.DataView.ToTable().Rows[0].ItemArray[1].ToString();
            ent.shortname = obj.DataView.ToTable().Rows[0].ItemArray[2].ToString();
            int id = int.Parse(obj.DataView.ToTable().Rows[0].ItemArray[0].ToString());
            string condition = " code = " + id;
            loadData();
            //int a = 0;
        }
    }
}
