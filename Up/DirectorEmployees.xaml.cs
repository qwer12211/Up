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
    public partial class DirectorEmployees : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        public DirectorEmployees()
        {
            InitializeComponent();
            MinHeight = 800;
            MinWidth = 1500;

            StafDgr.ItemsSource = context.Staff.ToList();
            StafComboBox.ItemsSource = context.Positions.ToList();
            StafComboBox.DisplayMemberPath = "PositionName";

            ShopComboBox.ItemsSource = context.Stores.ToList();
            ShopComboBox.DisplayMemberPath = "StoreAddress";
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            DirectorMain mainWindow = new DirectorMain();
            mainWindow.Show();
            this.Close();
        }

        private void BooksDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StafDgr.SelectedItem != null)
            {
                var selectedRow = (Staff)StafDgr.SelectedItem;

                SurnametBox.Text = selectedRow.StaffSurname;
                NameBookTextBox.Text = selectedRow.StaffName;
                MidleTextBox.Text = selectedRow.StaffMiddleName;
                PhoneTextBox.Text = selectedRow.StaffContact;

                StafComboBox.SelectedItem = context.Positions.FirstOrDefault(p => p.PositionName == selectedRow.Positions.PositionName);
                ShopComboBox.SelectedItem = context.Stores.FirstOrDefault(s => s.StoreAddress == selectedRow.Stores.StoreAddress);
            }
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (StafDgr.SelectedItem != null)
            {
                if (string.IsNullOrEmpty(SurnametBox.Text) ||
                    string.IsNullOrEmpty(NameBookTextBox.Text) ||
                    string.IsNullOrEmpty(PhoneTextBox.Text) ||
                    StafComboBox.SelectedItem == null ||
                    ShopComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                    return;
                }

                if (System.Text.RegularExpressions.Regex.IsMatch(SurnametBox.Text, @"\d") ||
                    System.Text.RegularExpressions.Regex.IsMatch(NameBookTextBox.Text, @"\d"))
                {
                    MessageBox.Show("Имя и фамилия не могут содержать цифры.");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneTextBox.Text, @"^\d+$"))
                {
                    MessageBox.Show("Введите корректный номер телефона (только цифры).");
                    return;
                }

                if (PhoneTextBox.Text.Length < 11)
                {
                    MessageBox.Show("Номер телефона должен содержать минимум 11 цифр.");
                    return;
                }

                if (PhoneTextBox.Text.Length > 11)
                {
                    MessageBox.Show("Номер телефона должен содержать максимум 11 цифр.");
                    return;
                }

                if (context.Staff.Any(staff => staff.StaffContact == PhoneTextBox.Text && staff.ID_Staff != ((Staff)StafDgr.SelectedItem).ID_Staff))
                {
                    MessageBox.Show("Этот номер телефона уже используется. Пожалуйста, введите уникальный номер.");
                    return;
                }

                var selectedRow = (Staff)StafDgr.SelectedItem;

                selectedRow.StaffSurname = SurnametBox.Text;
                selectedRow.StaffName = NameBookTextBox.Text;
                selectedRow.StaffMiddleName = string.IsNullOrWhiteSpace(MidleTextBox.Text) ? null : MidleTextBox.Text;  
                selectedRow.StaffContact = PhoneTextBox.Text;

                if (StafComboBox.SelectedItem != null)
                {
                    var selectedPosition = (Positions)StafComboBox.SelectedItem;
                    selectedRow.Position_ID = selectedPosition.ID_Position;
                }

                if (ShopComboBox.SelectedItem != null)
                {
                    var selectedStore = (Stores)ShopComboBox.SelectedItem;
                    selectedRow.Store_ID = selectedStore.ID_Store;
                }

                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Запись успешно изменена!");
                    StafDgr.ItemsSource = context.Staff.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для изменения.");
            }
        }


        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (StafDgr.SelectedItem != null)
            {
                var selectedRow = (Staff)StafDgr.SelectedItem;
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                                             "Подтверждение удаления",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    context.Staff.Remove(selectedRow);

                    try
                    {
                        context.SaveChanges();
                        MessageBox.Show("Запись успешно удалена!");
                        StafDgr.ItemsSource = context.Staff.ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении записи: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.");
            }
        }

        private void StafComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StafComboBox.SelectedItem != null)
            {
                var selectedPosition = (Positions)StafComboBox.SelectedItem;


                if (selectedPosition.PositionName == "Склад" || selectedPosition.PositionName == "Директор")
                {

                    ShopComboBox.Visibility = Visibility.Collapsed;
                }
                else
                {

                    ShopComboBox.Visibility = Visibility.Visible;
                }
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SurnametBox.Text) ||
                string.IsNullOrEmpty(NameBookTextBox.Text) ||
                string.IsNullOrEmpty(PhoneTextBox.Text) ||
                StafComboBox.SelectedItem == null ||
                ShopComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(SurnametBox.Text, @"\d") ||
                System.Text.RegularExpressions.Regex.IsMatch(NameBookTextBox.Text, @"\d"))
            {
                MessageBox.Show("Имя и фамилия не могут содержать цифры.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneTextBox.Text, @"^\d+$"))
            {
                MessageBox.Show("Введите корректный номер телефона (только цифры).");
                return;
            }

            if (PhoneTextBox.Text.Length < 11)
            {
                MessageBox.Show("Номер телефона должен содержать минимум 11 цифр.");
                return;
            }

            if (PhoneTextBox.Text.Length > 11)
            {
                MessageBox.Show("Номер телефона должен содержать максимум 11 цифр.");
                return;
            }

            if (context.Staff.Any(staff => staff.StaffContact == PhoneTextBox.Text))
            {
                MessageBox.Show("Этот номер телефона уже используется. Пожалуйста, введите уникальный номер.");
                return;
            }

            var newStaff = new Staff
            {
                StaffSurname = SurnametBox.Text,
                StaffName = NameBookTextBox.Text,
                StaffMiddleName = string.IsNullOrWhiteSpace(MidleTextBox.Text) ? null : MidleTextBox.Text,  
                StaffContact = PhoneTextBox.Text,
                Position_ID = ((Positions)StafComboBox.SelectedItem).ID_Position,
                Store_ID = ((Stores)ShopComboBox.SelectedItem).ID_Store
            };

            try
            {
                context.Staff.Add(newStaff);
                context.SaveChanges();
                MessageBox.Show("Запись успешно добавлена!");
                StafDgr.ItemsSource = context.Staff.ToList();

                SurnametBox.Clear();
                NameBookTextBox.Clear();
                MidleTextBox.Clear();
                PhoneTextBox.Clear();
                StafComboBox.SelectedIndex = -1;
                ShopComboBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
