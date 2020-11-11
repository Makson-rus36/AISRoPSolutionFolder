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
           ent.fullname = "ФГБОУ ВО ВГТУ";
           ent.shortname = "ВГТУ";
           WriterData writerData = new WriterData();
           writerData.WriteInDb(ent, WriteMode.INSERTMODE, null);
           loadData();
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var obj = (DataRowView)DataGridEnterprise.SelectedItem;
            enterprises ent = new enterprises();
            ent.fullname = "ФГБОУ ВО ВГУ";
            ent.shortname = "ВГУ";
            int id = int.Parse(obj.DataView.ToTable().Rows[0].ItemArray[0].ToString());
            WriterData writerData = new WriterData();
            writerData.WriteInDb(ent, WriteMode.UPDATEMODE, String.Format("code = {0}", id));
            loadData();
            //int a = 0;
        }
    }
}
