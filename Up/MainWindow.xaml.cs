using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Up
{

    public partial class MainWindow : Window
    {
        private BookDBEntities2 context = new BookDBEntities2();
        public MainWindow()
        {
            InitializeComponent();
            RoleComboBox.ItemsSource = context.Positions.ToList();
            RoleComboBox.DisplayMemberPath = "PositionName";
        }

        private void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var phoneNumber = PhoneNumberTextBox.Text;
                var role = RoleComboBox.SelectedItem as Positions;

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    MessageBox.Show("Введите номер телефона.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!IsValidPhoneNumber(phoneNumber))
                {
                    MessageBox.Show("Неверный формат номера телефона. Пожалуйста, введите номер в формате 11 цифр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (role == null)
                {
                    MessageBox.Show("Выберите роль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                var staffMember = context.Staff.FirstOrDefault(s => s.StaffContact == phoneNumber && s.Position_ID == role.ID_Position);

                if (staffMember != null)
                {
                    OpenRoleWindow(role.PositionName);
                }
                else
                {
                    MessageBox.Show("Неверные данные. Попробуйте снова.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenRoleWindow(string roleName)
        {
            Window roleWindow;

            switch (roleName)
            {
                case "Сотрудник склада":
                    roleWindow = new SotrudnikSkladaMain();
                    break;
                case "Сотрудник магазина":
                    roleWindow = new SotrudnikMagazinaAmountBooks();
                    break;
                case "Директор":
                    roleWindow = new DirectorMain();
                    break;
                default:
                    MessageBox.Show("Роль не поддерживается.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
            }

            roleWindow.Show();
            Close();
        }


  
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^\d{11}$");
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
