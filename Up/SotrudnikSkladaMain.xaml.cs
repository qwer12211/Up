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
    /// Логика взаимодействия для SotrudnikSkladaMain.xaml
    /// </summary>
    public partial class SotrudnikSkladaMain : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        
        public SotrudnikSkladaMain()
        {
            InitializeComponent();
            MinHeight = 800;
            MinWidth = 1500;


            var result = from book in context.Books
                         join storeBook in context.StoresBooks on book.ID_Book equals storeBook.Book_ID
                         join store in context.Stores on storeBook.Store_ID equals store.ID_Store
                         select new
                         {
                             НазваниеКниги = book.BookTitle,
                             КоличествоНаСкладе = book.BookAmount,
                             КоличествоВМагазине = storeBook.StoreBookAmount,
                             АдресМагазина = store.StoreAddress
                         };
            BooksDgr.ItemsSource = result.ToList();

        }

        

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();

            this.Close();
        }

        private void PostavkaButton_Click(object sender, RoutedEventArgs e)
        {
            SotrudnikSkladaSklad sklad = new SotrudnikSkladaSklad();

            sklad.Show();

            this.Close();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            SotrudnikSkladaBooks mainSotrudnik = new SotrudnikSkladaBooks();

            mainSotrudnik.Show();

            this.Close();
        }
    }
}
