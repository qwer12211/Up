using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Up
{
    public partial class SotrudnikMagazinaSales : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            SotrudnikMagazinaAmountBooks mainWindow = new SotrudnikMagazinaAmountBooks();
            mainWindow.Show();
            this.Close();
        }

        public SotrudnikMagazinaSales()
        {
            InitializeComponent();
            MinHeight = 800;
            MinWidth = 1500;

            // Получаем данные о продажах без использования currentEmployeeId
            var salesData = from sale in context.Sales
                            join book in context.Books on sale.Book_ID equals book.ID_Book
                            join status in context.Statuses on book.Status_ID equals status.ID_Status
                            join supply in context.Supply on book.ID_Book equals supply.Book_ID
                            join supplier in context.Supplier on supply.Supplier_ID equals supplier.ID_Supplier
                            join store in context.Stores on sale.Store_ID equals store.ID_Store
                            select new SalesData
                            {
                                BookTitle = book.BookTitle,
                                BookAuthor = book.BookAuthor,
                                SaleBookAmount = sale.SaleBookAmount,
                                StatusName = status.StatusName,
                                SupplyDate = supply.SupplyDate,
                                SupplierName = supplier.SupplierName,
                                StoreAddress = store.StoreAddress
                            };

            BooksDgr.ItemsSource = salesData.ToList();
            NameBookComboBox.ItemsSource = context.Books.ToList();
            NameBookComboBox.DisplayMemberPath = "BookTitle";
            ShopComboBox.ItemsSource = context.Stores.ToList();
            ShopComboBox.DisplayMemberPath = "StoreAddress";
        }

        private void BooksDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksDgr.SelectedItem != null)
            {
                var selectedRow = BooksDgr.SelectedItem as SalesData;

                if (selectedRow != null)
                {
                    var selectedBook = (Books)NameBookComboBox.SelectedItem;
                    if (selectedBook != null)
                    {
                        UpdateRevenue(selectedBook);
                    }
                }
            }
        }

        private void QuantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var selectedBook = (Books)NameBookComboBox.SelectedItem;
            if (selectedBook != null && !string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                int quantity;
                if (int.TryParse(QuantityTextBox.Text, out quantity) && quantity > 0)
                {
                    decimal revenue = selectedBook.BookPrice * quantity;
                    revenueTextBox.Text = revenue.ToString("F2");
                }
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameBookComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ShopComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите магазин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите количество книг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество книг (целое число больше 0).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedBook = (Books)NameBookComboBox.SelectedItem;
            var selectedShop = (Stores)ShopComboBox.SelectedItem;
            var selectedDate = DatePicker.SelectedDate;

            // Проверяем, существует ли уже такая продажа
            var existingSale = context.Sales.FirstOrDefault(s =>
                s.Book_ID == selectedBook.ID_Book &&
                s.Store_ID == selectedShop.ID_Store &&
                s.SaleDate == selectedDate.Value);

            if (existingSale != null)
            {
                MessageBox.Show("Продажа с этой книгой, в этом магазине и на эту дату уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создаём новую продажу
            var newSale = new Sales
            {
                Book_ID = selectedBook.ID_Book,
                Store_ID = selectedShop.ID_Store,
                SaleDate = selectedDate.Value,
                SaleBookAmount = quantity
            };

            try
            {
                context.Sales.Add(newSale);
                context.SaveChanges();

                decimal revenue = selectedBook.BookPrice * quantity;
                revenueTextBox.Text = revenue.ToString("F2");

                var salesData = from sale in context.Sales
                                join book in context.Books on sale.Book_ID equals book.ID_Book
                                join status in context.Statuses on book.Status_ID equals status.ID_Status
                                join supply in context.Supply on book.ID_Book equals supply.Book_ID
                                join supplier in context.Supplier on supply.Supplier_ID equals supplier.ID_Supplier
                                join store in context.Stores on sale.Store_ID equals store.ID_Store
                                select new SalesData
                                {
                                    BookTitle = book.BookTitle,
                                    BookAuthor = book.BookAuthor,
                                    SaleBookAmount = sale.SaleBookAmount,
                                    StatusName = status.StatusName,
                                    SupplyDate = supply.SupplyDate,
                                    SupplierName = supplier.SupplierName,
                                    StoreAddress = store.StoreAddress
                                };

                BooksDgr.ItemsSource = salesData.ToList();

                NameBookComboBox.SelectedItem = null;
                ShopComboBox.SelectedItem = null;
                DatePicker.SelectedDate = null;
                QuantityTextBox.Clear();

                MessageBox.Show("Продажа успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateRevenue(Books selectedBook)
        {
            if (!string.IsNullOrEmpty(QuantityTextBox.Text))
            {
                int quantity;
                if (int.TryParse(QuantityTextBox.Text, out quantity) && quantity > 0)
                {
                    decimal revenue = selectedBook.BookPrice * (decimal)quantity;
                    revenueTextBox.Text = revenue.ToString("F2");
                }
            }
        }


        


        private void ShopComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



        private void NameBookComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = (Books)NameBookComboBox.SelectedItem;
            if (selectedBook != null)
            {
                // Проверяем статус книги
                var bookStatus = context.Statuses.FirstOrDefault(s => s.ID_Status == selectedBook.Status_ID)?.StatusName;
                if (bookStatus == "нет в наличии")
                {
                    // Устанавливаем количество в 0, если книга нет в наличии
                    QuantityTextBox.Text = "0";
                }
                else
                {
                    // Если книга в наличии, очищаем поле количества
                    QuantityTextBox.Clear();
                }
            }
        }
    }
}
