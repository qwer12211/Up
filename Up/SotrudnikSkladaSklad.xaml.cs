using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
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
using Path = System.IO.Path;

namespace Up
{
    /// <summary>
    /// Логика взаимодействия для SotrudnikSkladaSklad.xaml
    /// </summary>
    public partial class SotrudnikSkladaSklad : Window
    {

        private BookDBEntities2 context = new BookDBEntities2();
        public SotrudnikSkladaSklad()
        {
            InitializeComponent();
           

            BooksDgr.ItemsSource =context.Supply.ToList();

            NameBookComboBox.ItemsSource =context.Books.ToList();
            NameBookComboBox.DisplayMemberPath= "BookTitle";

            StatusComboBox.ItemsSource = context.Statuses.ToList();
            StatusComboBox.DisplayMemberPath = "StatusName";


            SupplierComboBox.ItemsSource = context.Supplier.ToList();
            SupplierComboBox.DisplayMemberPath = "SupplierName";

            ShopComboBox.ItemsSource = context.Stores.ToList();
            ShopComboBox.DisplayMemberPath = "StoreAddress";


        }

        
        private void NameBookComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BooksDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (BooksDgr.SelectedItem != null)
            {
                var selectedRow = (Supply)BooksDgr.SelectedItem;

    
                NameBookComboBox.SelectedItem = context.Books.FirstOrDefault(b => b.ID_Book == selectedRow.Book_ID); 
                StatusComboBox.SelectedItem = context.Statuses.FirstOrDefault(s => s.ID_Status == selectedRow.Status_ID); 
                SupplierComboBox.SelectedItem = context.Supplier.FirstOrDefault(s => s.ID_Supplier == selectedRow.Supplier_ID); 
                ShopComboBox.SelectedItem = context.Stores.FirstOrDefault(st => st.ID_Store == selectedRow.Store_ID); 

                QuantityTextBox.Text = selectedRow.BookAmount.ToString(); 

                DatePicker.SelectedDate = selectedRow.SupplyDate; 
            }
        }

        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (BooksDgr.SelectedItem != null)
                {
                    var selectedBook = (Books)BooksDgr.SelectedItem;
                    var selectedStatus = (Statuses)StatusComboBox.SelectedItem;

     
                    if (selectedStatus.StatusName == "Нет в наличии")
                    {
                        selectedBook.BookAmount = 0;
                    }

                    selectedBook.Statuses = selectedStatus;

                    context.SaveChanges();

                    BooksDgr.ItemsSource = context.Books.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при изменении статуса: {ex.Message}");
            }
        }


        private bool ValidateFields()
        {
            StringBuilder errors = new StringBuilder();

            if (NameBookComboBox.SelectedItem == null)
                errors.AppendLine("Выберите название книги.");
            if (StatusComboBox.SelectedItem == null)
                errors.AppendLine("Выберите статус.");
            if (SupplierComboBox.SelectedItem == null)
                errors.AppendLine("Выберите поставщика.");
            if (ShopComboBox.SelectedItem == null)
                errors.AppendLine("Выберите магазин.");
            if (DatePicker.SelectedDate == null)
                errors.AppendLine("Выберите дату поставки.");
            if (string.IsNullOrWhiteSpace(QuantityTextBox.Text) || !int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
                errors.AppendLine("Введите корректное количество (целое положительное число).");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }


        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;


            var selectedBook = (Books)NameBookComboBox.SelectedItem;


            var newSupply = new Supply
            {
                Book_ID = selectedBook.ID_Book,
                Status_ID = ((Statuses)StatusComboBox.SelectedItem).ID_Status,
                Supplier_ID = ((Supplier)SupplierComboBox.SelectedItem).ID_Supplier,
                Store_ID = ((Stores)ShopComboBox.SelectedItem).ID_Store,
                SupplyDate = (DateTime)DatePicker.SelectedDate,
                BookAmount = int.Parse(QuantityTextBox.Text)  
            };


            var bookToUpdate = context.Books.FirstOrDefault(b => b.ID_Book == selectedBook.ID_Book);
            if (bookToUpdate != null)
            {
                if (bookToUpdate.BookAmount < newSupply.BookAmount)
                {
                    MessageBox.Show("На складе недостаточно книг. Количество будет установлено в 0.",
                        "Недостаток книг", MessageBoxButton.OK, MessageBoxImage.Warning);
                    newSupply.BookAmount = 0; 
                }
                else
                {
                    bookToUpdate.BookAmount -= newSupply.BookAmount;  

                    if (bookToUpdate.BookAmount < 30)
                    {
                        MessageBox.Show("Внимание! Количество книг на складе меньше 30.", "Низкие запасы", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            try
            {
    
                context.Supply.Add(newSupply);

    
                context.SaveChanges();

                BooksDgr.ItemsSource = context.Supply.ToList();

                MessageBox.Show("Новая запись успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }









        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {

        }





        private void LeaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            SotrudnikSkladaMain mainSotrudnik = new SotrudnikSkladaMain();

            mainSotrudnik.Show();

            this.Close();
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string folderName = "Отчёты";
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = Path.Combine(desktopPath, folderName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = GenerateUniqueNamePdf(Path.Combine(folderPath, "Накладная.pdf"));
                if (filePath == null)
                {
                    return; 
                }

                PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Times New Roman", 12);

                gfx.DrawString("Накладная поставок", new XFont("Times New Roman", 24), XBrushes.Black, new XPoint(250, 40));
                gfx.DrawString($"№{Path.GetFileNameWithoutExtension(filePath)}", new XFont("Times New Roman", 16), XBrushes.Black, new XPoint(250, 80));

                gfx.DrawString("Список поставок:", font, XBrushes.Black, new XPoint(100, 120));

                int yPosition = 160;
                var supplies = context.Supply.ToList();
                foreach (var supply in supplies)
                {
                    string supplierName = context.Supplier.FirstOrDefault(s => s.ID_Supplier == supply.Supplier_ID)?.SupplierName;
                    string supplyDate = supply.SupplyDate.ToString("dd.MM.yyyy");

                    gfx.DrawString($"{supplierName} - {supplyDate}", font, XBrushes.Black, new XPoint(100, yPosition));
                    yPosition += 20;
                }

                document.Save(filePath);
                MessageBox.Show($"Накладная успешно сохранена в папку: {folderName}\nФайл: {Path.GetFileName(filePath)}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении накладной: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private static string GenerateUniqueNamePdf(string pdfFilePath)
        {
            try
            {
                string directory = Path.GetDirectoryName(pdfFilePath);
                string reportNumberFilePath = Path.Combine(directory, "lastReportNumber.txt");

                int reportNumber = 1; 

                if (File.Exists(reportNumberFilePath))
                {
                    string lastReportNumber = File.ReadAllText(reportNumberFilePath);
                    if (int.TryParse(lastReportNumber, out int lastNumber))
                    {
                        reportNumber = lastNumber + 1; 
                    }
                }

                string fileName = $"Накладная_{reportNumber}.pdf";
                string filePath = Path.Combine(directory, fileName);


                File.WriteAllText(reportNumberFilePath, reportNumber.ToString());

                return filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании уникального имени файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

    }
}
