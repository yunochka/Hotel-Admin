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

namespace HotelAdm2App
{
    /// <summary>
    /// Логика взаимодействия для HousemaidWindow.xaml
    /// </summary>
    public partial class HousemaidWindow : Window
    {
        public HousemaidWindow(Staff user)
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;

          
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private string GetDayTime()
        {
            int hour = DateTime.Now.Hour;
            if (hour < 12) return "утро";
            if (hour < 18) return "день";
            return "вечер";
        }

        private void CompleteTask_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Задача отмечена как выполненная!", "Успех",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class RoomTask
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
    }
}