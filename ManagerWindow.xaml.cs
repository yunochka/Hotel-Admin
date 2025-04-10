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

namespace HotelAdm2App
{
    /// <summary>
    /// Логика взаимодействия для ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        public ObservableCollection<Registration> CurrentRegistrations { get; set; }

        public ManagerWindow(Staff user)
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;

            // Инициализация коллекции текущих заселений
            CurrentRegistrations = new ObservableCollection<Registration>();
            CurrentRegistrationsGrid.ItemsSource = CurrentRegistrations;

 

            // Установка дат по умолчанию
            dpCheckIn.SelectedDate = DateTime.Now;
            dpCheckOut.SelectedDate = DateTime.Now.AddDays(1);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // Дополнительная инициализация при загрузке
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                cmbRoomNumber.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля!",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            // Создание новой регистрации
            var newRegistration = new Registration
            {
                RoomNumber = (cmbRoomNumber.SelectedItem as ComboBoxItem)?.Content.ToString().Split(' ')[0],
                GuestName = $"{txtLastName.Text} {txtFirstName.Text}",
                CheckInDate = dpCheckIn.SelectedDate ?? DateTime.Now,
                CheckOutDate = dpCheckOut.SelectedDate ?? DateTime.Now.AddDays(1)
            };

            // Добавление в список
            CurrentRegistrations.Add(newRegistration);

            // Очистка формы
            txtLastName.Text = string.Empty;
            txtFirstName.Text = string.Empty;
            txtPhone.Text = string.Empty;
            cmbRoomNumber.SelectedIndex = -1;
            dpCheckIn.SelectedDate = DateTime.Now;
            dpCheckOut.SelectedDate = DateTime.Now.AddDays(1);

            MessageBox.Show("Гость успешно зарегистрирован!",
                          "Успех",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void CurrentRegistrationsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class Registration
    {
        public string RoomNumber { get; set; }
        public string GuestName { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}