using Azure.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Data.Entity;

namespace HotelAdm2App
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class MainAdminWindow : Window
    {
        private Staff _currentUser;

        public MainAdminWindow(Staff user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            // Установка данных пользователя в интерфейс
            using (var context = new HotelsqlEntities()) // Подключение к базе данных
            {
                var rooms = context.Hotel_Room.Include(r => r.Staff).ToList();
                var guests = context.Guest.Include(g => g.Staff).ToList();
                var user = context.Staff.Include(s => s.Role).ToList();
                RoomsGrid.ItemsSource = rooms;
                GuestsGrid.ItemsSource = guests;
                StaffGrid.ItemsSource = user;
            }
            RoomsGrid.IsReadOnly = true;
            GuestsGrid.IsReadOnly = true;
            StaffGrid.IsReadOnly = true;
            txtCurrentUser.Text = $"{_currentUser.Full_Name} {_currentUser.Name} {_currentUser.First_Name}";
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Дополнительная инициализация при загрузке
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?", "Подтверждение выхода",
                                      MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }
        private void AddRoomButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditRoomButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RoomFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomsGrid == null || RoomFilterComboBox.SelectedItem == null)
                return;

            using (var context = new HotelsqlEntities())
            {
                var selectedFilter = (RoomFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                IQueryable<Hotel_Room> query = context.Hotel_Room.Include(r => r.Staff);

                switch (selectedFilter)
                {
                    case "Свободные":
                        query = query.Where(r => r.Room_Status == "Свободен");
                        break;
                    case "Занятые":
                        query = query.Where(r => r.Room_Status == "Занят");
                        break;
                        // Для "Все номера" фильтрация не применяется
                }

                RoomsGrid.ItemsSource = query.ToList();
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog // Диалог сохранения файла Word
            {
                FileName = "Отчет",             // Имя файла по умолчанию
                DefaultExt = ".docx",           // Расширение по умолчанию
                Filter = "Word documents (.docx)|*.docx" // Фильтр файлов
            };

            if (saveFileDialog.ShowDialog() == true) // Открытие диалога и проверка выбора
            {
                HotelReportGenerator.GenerateHotelReport(saveFileDialog.FileName); // Экспорт данных в Word
            }
        }
    }
}