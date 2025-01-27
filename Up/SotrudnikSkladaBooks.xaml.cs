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
    public partial class SotrudnikSkladaBooks : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        public SotrudnikSkladaBooks()
        {
            InitializeComponent();
            MinHeight = 800;
            MinWidth = 1500;
            BooksDgr.ItemsSource = context.Books.ToList();

            StatusComboBox.ItemsSource = context.Statuses.ToList();
            StatusComboBox.DisplayMemberPath = "StatusName";

            genreComboBox.ItemsSource = context.Genre.ToList();
            genreComboBox.DisplayMemberPath = "GenreName";
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            SotrudnikSkladaMain mainSotrudnik = new SotrudnikSkladaMain();
            mainSotrudnik.Show();
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(NameBookTextBox.Text) ||
                    string.IsNullOrEmpty(AuthorTextBox.Text) ||
                    string.IsNullOrEmpty(ISBNTextBox.Text) ||
                    string.IsNullOrEmpty(PriceTextBox.Text) ||
                    genreComboBox.SelectedItem == null ||
                    StatusComboBox.SelectedItem == null ||
                    string.IsNullOrEmpty(BookAmountTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }


                string isbnPattern = @"^(97(8|9))?\d{1,5}(-\d{1,7}){1,3}-\d{1,7}-\d{1,7}-\d{1}$";
                string isbn = ISBNTextBox.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(isbn, isbnPattern))
                {
                    MessageBox.Show("Неверный формат ISBN.");
                    return;
                }


                var existingBook = context.Books.FirstOrDefault(b => b.BookISBN == isbn);
                if (existingBook != null)
                {
                    MessageBox.Show("Книга с таким ISBN уже существует");
                    return;
                }

  
                decimal price;
                int amount;
                if (!decimal.TryParse(PriceTextBox.Text, out price) || price <= 0)
                {
                    MessageBox.Show("Неверный формат цены.");
                    return;
                }

                if (!int.TryParse(BookAmountTextBox.Text, out amount) || amount <= 0)
                {
                    MessageBox.Show("Неверный формат количества.");
                    return;
                }

                string bookTitle = NameBookTextBox.Text.Trim();
                var existinBook = context.Books.FirstOrDefault(b => b.BookTitle.ToLower() == bookTitle.ToLower());
                if (existinBook != null)
                {
                    MessageBox.Show("Книга с таким названием уже существует.");
                    return;
                }

                Books newBook = new Books
                {
                    BookTitle = NameBookTextBox.Text,
                    BookAuthor = AuthorTextBox.Text,
                    BookISBN = isbn,
                    BookPrice = price,
                    Genre = (Genre)genreComboBox.SelectedItem,
                    Statuses = (Statuses)StatusComboBox.SelectedItem,
                    BookAmount = amount
                };

                context.Books.Add(newBook);
                context.SaveChanges();

     
                BooksDgr.ItemsSource = context.Books.ToList();

                NameBookTextBox.Clear();
                AuthorTextBox.Clear();
                ISBNTextBox.Clear();
                PriceTextBox.Clear();
                BookAmountTextBox.Clear();
                genreComboBox.SelectedItem = null;
                StatusComboBox.SelectedItem = null;

                MessageBox.Show("Книга успешно добавлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении книги: {ex.Message}");
            }
        }


        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BooksDgr.SelectedItem != null)
                {
                    var selectedBook = (Books)BooksDgr.SelectedItem;

                    if (string.IsNullOrEmpty(NameBookTextBox.Text) ||
                        string.IsNullOrEmpty(AuthorTextBox.Text) ||
                        string.IsNullOrEmpty(ISBNTextBox.Text) ||
                        string.IsNullOrEmpty(PriceTextBox.Text) ||
                        genreComboBox.SelectedItem == null ||
                        StatusComboBox.SelectedItem == null ||
                        string.IsNullOrEmpty(BookAmountTextBox.Text))
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля.");
                        return;
                    }

                    string newTitle = NameBookTextBox.Text.Trim();
                    string newISBN = ISBNTextBox.Text.Trim();

           
                    var duplicateTitleBook = context.Books
                        .FirstOrDefault(b => b.BookTitle.ToLower() == newTitle.ToLower() && b.ID_Book != selectedBook.ID_Book);
                    if (duplicateTitleBook != null)
                    {
                        MessageBox.Show("Книга с таким названием уже существует.");
                        return;
                    }

                    var duplicateISBNBook = context.Books
                        .FirstOrDefault(b => b.BookISBN == newISBN && b.ID_Book != selectedBook.ID_Book);
                    if (duplicateISBNBook != null)
                    {
                        MessageBox.Show("Книга с таким ISBN уже существует.");
                        return;
                    }

                    decimal price;
                    int amount;
                    if (!decimal.TryParse(PriceTextBox.Text, out price) || price <= 0)
                    {
                        MessageBox.Show("Неверный формат цены.");
                        return;
                    }

                    if (!int.TryParse(BookAmountTextBox.Text, out amount) || amount <= 0)
                    {
                        MessageBox.Show("Неверный формат количества.");
                        return;
                    }

                    selectedBook.BookTitle = newTitle;
                    selectedBook.BookAuthor = AuthorTextBox.Text;
                    selectedBook.BookISBN = newISBN;
                    selectedBook.BookPrice = price;
                    selectedBook.Genre = (Genre)genreComboBox.SelectedItem;
                    selectedBook.Statuses = (Statuses)StatusComboBox.SelectedItem;
                    selectedBook.BookAmount = amount;

                    context.SaveChanges();
                    BooksDgr.ItemsSource = context.Books.ToList();

                    NameBookTextBox.Clear();
                    AuthorTextBox.Clear();
                    ISBNTextBox.Clear();
                    PriceTextBox.Clear();
                    BookAmountTextBox.Clear();
                    genreComboBox.SelectedItem = null;
                    StatusComboBox.SelectedItem = null;

                    MessageBox.Show("Данные книги успешно обновлены.");
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите книгу для изменения.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при изменении книги: {ex.Message}");
            }
        }


        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BooksDgr.SelectedItem != null)
                {
                    var selectedBook = (Books)BooksDgr.SelectedItem;

                    var result = MessageBox.Show($"Вы уверены, что хотите удалить книгу '{selectedBook.BookTitle}'?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        context.Books.Remove(selectedBook);
                        context.SaveChanges();

                        BooksDgr.ItemsSource = context.Books.ToList();

                        MessageBox.Show("Книга успешно удалена");
                    }
                }
                else
                {
                    MessageBox.Show("Пожалуйста, выберите книгу для удаления.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при удалении книги: {ex.Message}");
            }
        }

        private void BooksDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BooksDgr.SelectedItem != null)
            {
                var selectedRow = (Books)BooksDgr.SelectedItem;

                NameBookTextBox.Text = selectedRow.BookTitle;
                AuthorTextBox.Text = selectedRow.BookAuthor;
                ISBNTextBox.Text = selectedRow.BookISBN;
                PriceTextBox.Text = selectedRow.BookPrice.ToString();
                BookAmountTextBox.Text = selectedRow.BookAmount.ToString();

                genreComboBox.SelectedItem = context.Genre.FirstOrDefault(g => g.GenreName == selectedRow.Genre.GenreName);
                StatusComboBox.SelectedItem = context.Statuses.FirstOrDefault(s => s.StatusName == selectedRow.Statuses.StatusName);
            }
        }


        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ISBNTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

           
        }

    }
}
