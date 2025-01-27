using System;
using System.Collections.Generic;
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

namespace Up
{
    /// <summary>
    /// Логика взаимодействия для DirectorMainRevenue.xaml
    /// </summary>
    public partial class DirectorMainRevenue : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        public DirectorMainRevenue()
        {
            InitializeComponent();
            MinHeight = 800;
            MinWidth = 1500;

            var result = from store in context.Stores
                         join storeBook in context.StoresBooks on store.ID_Store equals storeBook.Store_ID
                         join sale in context.Sales on storeBook.Book_ID equals sale.Book_ID
                         group sale by store.StoreAddress into storeGroup
                         select new
                         {
                             АдресМагазина = storeGroup.Key,
                             Выручка = storeGroup.Sum(s => s.Revenue)
                         };

            BooksDgr.ItemsSource = result.ToList();

        }


        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            DirectorMain mainWindow = new DirectorMain();

            mainWindow.Show();

            this.Close();
        }

        private void BooksDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
