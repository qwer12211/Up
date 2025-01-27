using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для SotrudnikMagazinaAmountBooks.xaml
    /// </summary>
    public partial class SotrudnikMagazinaAmountBooks : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        public SotrudnikMagazinaAmountBooks()
        {

            InitializeComponent();
            BooksDgr.ItemsSource = context.Supply.ToList();

   

            StatusComboBox.ItemsSource = context.Statuses.ToList();
            StatusComboBox.DisplayMemberPath = "StatusName";
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();

            this.Close();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDgr.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите книгу.");
                return;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите статус.");
                return;
            }

            var selectedRow = (Supply)BooksDgr.SelectedItem;
            var selectedStatus = (Statuses)StatusComboBox.SelectedItem;

            if (selectedRow != null && selectedStatus != null)
            {
                selectedRow.Status_ID = selectedStatus.ID_Status;

                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Статус успешно обновлен!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении статуса: {ex.Message}");
                }

                BooksDgr.ItemsSource = context.Supply.ToList();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SotrudnikMagazinaSales mainWindow = new SotrudnikMagazinaSales();

            mainWindow.Show();

            this.Close();

        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BooksDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksDgr.SelectedItem != null)
            {

                var selectedBook = BooksDgr.SelectedItem as Supply; 

                if (selectedBook != null)
                {
                    var selectedStatus = selectedBook.Statuses; 
                    StatusComboBox.SelectedItem = selectedStatus;
                }
            }
        }
    }
}
