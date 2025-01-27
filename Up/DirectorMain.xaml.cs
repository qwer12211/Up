using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using Path = System.IO.Path;


namespace Up
{
    /// <summary>
    /// Логика взаимодействия для DirectorMain.xaml
    /// </summary>
    public partial class DirectorMain : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        public DirectorMain()
        {
            InitializeComponent();
            MinHeight = 800;
            MinWidth = 1500;

        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.Show();

            this.Close();
        }

        private void employeesButton_Click(object sender, RoutedEventArgs e)
        {
            DirectorEmployees mainWindow = new DirectorEmployees();

            mainWindow.Show();

            this.Close();
        }

        private void RevenueButton_Click(object sender, RoutedEventArgs e)
        {
            DirectorMainRevenue mainWindow = new DirectorMainRevenue();

            mainWindow.Show();

            this.Close();
        }

        private void reportComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedReport = reportComboBox.SelectedItem as ComboBoxItem;

            if (selectedReport != null)
            {
                string reportContent = selectedReport.Content.ToString();

                if (reportContent == "Самые продаваемые книги")
                {
                    StartDatePicker.Visibility = Visibility.Visible;
                    EndDatePicker.Visibility = Visibility.Visible;
                    TextBlock_1.Visibility = Visibility.Visible;
                    TextBlock_2.Visibility = Visibility.Visible;
                }
                else
                {
                    StartDatePicker.Visibility = Visibility.Collapsed;
                    EndDatePicker.Visibility = Visibility.Collapsed;
                    TextBlock_1.Visibility = Visibility.Collapsed;
                    TextBlock_2.Visibility = Visibility.Collapsed;
                }
            }
        }


        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedReport = reportComboBox.SelectedItem as ComboBoxItem;

                if (selectedReport != null)
                {
                    string reportContent = selectedReport.Content.ToString();
                    var random = new Random();
                    int reportNumber = random.Next(10000000, 99999999);
                    string folderName = "Отчёты";
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string folderPath = Path.Combine(desktopPath, folderName);

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    string fileName = $"Отчёт{reportNumber}.pdf";
                    string filePath = Path.Combine(folderPath, fileName);

                   

                    PdfSharp.Pdf.PdfDocument document = new PdfSharp.Pdf.PdfDocument();
                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Times New Roman", 12);

                    gfx.DrawString("ОТЧЁТ", new XFont("Times New Roman", 24), XBrushes.Black, new XPoint(250, 40));
                    gfx.DrawString($"№{reportNumber}", new XFont("Times New Roman", 16), XBrushes.Black, new XPoint(250, 80));

                    if (reportContent == "Самые продаваемые книги")
                    {
                        if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
                        {
                            MessageBox.Show("Пожалуйста, выберите период для формирования отчета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        DateTime startDate = StartDatePicker.SelectedDate.Value;
                        DateTime endDate = EndDatePicker.SelectedDate.Value;


                        MessageBox.Show($"StartDate: {startDate.ToShortDateString()} - EndDate: {endDate.ToShortDateString()}");

                        gfx.DrawString($"Отчет по самым продаваемым книгам за период с {startDate.ToShortDateString()} по {endDate.ToShortDateString()}.", font, XBrushes.Black, new XPoint(100, 120));

                        int yPosition = 160;
                        var topSellingBooks = (from book in context.Books
                                               join sale in context.Sales on book.ID_Book equals sale.Book_ID
                                               where sale.SaleDate >= startDate && sale.SaleDate <= endDate
                                               group sale by new { book.BookTitle, book.BookAuthor } into bookGroup
                                               orderby bookGroup.Sum(s => s.SaleBookAmount) descending
                                               select new
                                               {
                                                   Title = bookGroup.Key.BookTitle,
                                                   Author = bookGroup.Key.BookAuthor,
                                                   SalesAmount = bookGroup.Sum(s => s.SaleBookAmount)
                                               }).ToList(); // Убираем Take(5)

                        foreach (var book in topSellingBooks)
                        {
                            gfx.DrawString($"{book.Author} – «{book.Title}» - {book.SalesAmount} экземпляров", font, XBrushes.Black, new XPoint(100, yPosition));
                            yPosition += 20;
                        }
                    }
                    else if (reportContent == "Общая выручка")
                    {
                        gfx.DrawString("Общая выручка по каждому магазину", font, XBrushes.Black, new XPoint(100, 120));

                        int yPosition = 160;
                        var storesRevenue = context.Stores.Select(s => new
                        {
                            StoreName = s.StoreAddress,
                            Revenue = s.Sales.Sum(r => r.Revenue)
                        }).ToList();

                        foreach (var store in storesRevenue)
                        {
                            gfx.DrawString($"{store.StoreName} – {store.Revenue.ToString("C")}", font, XBrushes.Black, new XPoint(100, yPosition));
                            yPosition += 20;
                        }
                    }
                    else if (reportContent == "Остатки")
                    {
                        gfx.DrawString("Остатки книг на складе и в магазинах", font, XBrushes.Black, new XPoint(100, 120));

                        int yPosition = 160;

      
                        var stockLevels = (from store in context.Stores
                                           join storeBook in context.StoresBooks on store.ID_Store equals storeBook.Store_ID
                                           join book in context.Books on storeBook.Book_ID equals book.ID_Book
                                           select new
                                           {
                                               StoreName = store.StoreAddress,
                                               BookTitle = book.BookTitle,
                                               BookAuthor = book.BookAuthor,
                                               StoreBookAmount = storeBook.StoreBookAmount,
                                               BookAmount = book.BookAmount 
                                           }).ToList();

                        var groupedStockLevels = stockLevels
                                                 .GroupBy(s => new { s.StoreName, s.BookTitle, s.BookAuthor })
                                                 .Select(g => new
                                                 {
                                                     g.Key.StoreName,
                                                     g.Key.BookTitle,
                                                     g.Key.BookAuthor,
                                                     TotalStoreStockAmount = g.Sum(s => s.StoreBookAmount),
                                                     TotalWarehouseStockAmount = g.Max(s => s.BookAmount) 
                                                 }).ToList();

                  
                        gfx.DrawString("Остатки книг в магазинах:", new XFont("Times New Roman", 12), XBrushes.Black, new XPoint(100, yPosition));
                        yPosition += 20;

                        foreach (var stock in groupedStockLevels)
                        {
                            gfx.DrawString($"{stock.StoreName} – «{stock.BookTitle}» ({stock.BookAuthor})", font, XBrushes.Black, new XPoint(100, yPosition));
                            yPosition += 20; 
                            gfx.DrawString($"Остаток в магазине: {stock.TotalStoreStockAmount} книг", font, XBrushes.Black, new XPoint(100, yPosition));
                            yPosition += 20;
                        }

        
                        yPosition += 20;

                        gfx.DrawString("Остатки книг на складе:", new XFont("Times New Roman", 12), XBrushes.Black, new XPoint(100, yPosition));
                        yPosition += 20;

                        foreach (var stock in groupedStockLevels)
                        {
                            gfx.DrawString($"{stock.BookTitle} ({stock.BookAuthor})", font, XBrushes.Black, new XPoint(100, yPosition));
                            yPosition += 20; 
                            gfx.DrawString($"Остаток на складе: {stock.TotalWarehouseStockAmount} книг", font, XBrushes.Black, new XPoint(100, yPosition));
                            yPosition += 20;
                        }
                    }



                    document.Save(filePath);
                    MessageBox.Show($"Отчёт успешно сохранён в папку: {folderName}\nФайл: {Path.GetFileName(filePath)}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }

}

