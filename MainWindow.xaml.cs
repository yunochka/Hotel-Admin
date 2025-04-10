using HotelAdm2App;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace HotelAdm2App
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private int _loginAttempts;
        private bool _isCaptchaVisible;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += delegate (object s, RoutedEventArgs e) { txtUsername.Focus(); };
            // Изначально скрываем элементы капчи
            txtCaptcha.Visibility = Visibility.Collapsed;
            txtCaptchaInput.Visibility = Visibility.Collapsed;
            btnRefreshCaptcha.Visibility = Visibility.Collapsed;
            CodeTxt.Visibility = Visibility.Collapsed;
            BorderCaptcha.Visibility = Visibility.Collapsed;

            _isCaptchaVisible = false;
        }

        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] captcha = new char[5];
            for (int i = 0; i < 5; i++)
            {
                captcha[i] = chars[_random.Next(chars.Length)];
            }
            txtCaptcha.Text = new string(captcha);
            txtCaptchaInput.Clear();
        }

        private void RefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();
            string captcha = txtCaptchaInput.Text.Trim();

            // Проверка пустых полей
            if (string.IsNullOrWhiteSpace(username))
            {
                HandleLoginFailure("Введите логин!");
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                HandleLoginFailure("Введите пароль!");
                return;
            }

            // Проверка капчи, если она видима
            if (_isCaptchaVisible && !ValidateCaptcha(captcha))
            {
                return;
            }

            try
            {
                using (var db = new HotelsqlEntities()) // Заменил GetContext() на прямое создание
                {
                    // Проверка подключения к базе данных
                    try
                    {
                        db.Database.Connection.Open();
                        db.Database.Connection.Close();
                    }
                    catch (Exception ex)
                    {
                        HandleLoginFailure($"Ошибка подключения к базе данных: {ex.Message}");
                        CheckCaptchaVisibility();
                        return;
                    }

                    // Сначала проверяем существование пользователя
                    var user = db.Staff.FirstOrDefault(s => s.Login == username);
                    if (user == null)
                    {
                        HandleLoginFailure("Пользователь с таким логином не найден!");
                        CheckCaptchaVisibility();
                        return;
                    }

                    // Отдельно проверяем пароль
                    if (user.Password != password)
                    {
                        HandleLoginFailure("Неверный пароль!");
                        CheckCaptchaVisibility();
                        return;
                    }

                    // Если дошли сюда, значит логин и пароль верны
                    OpenDashboardWindow(user);
                    Close();
                }
            }
            catch (Exception ex)
            {
                HandleLoginFailure($"Ошибка входа: {ex.Message}");
                CheckCaptchaVisibility();
            }
        }

        private bool ValidateCaptcha(string captcha)
        {
            if (string.IsNullOrWhiteSpace(captcha))
            {
                HandleLoginFailure("Введите код подтверждения!");
                GenerateCaptcha();
                return false;
            }

            if (captcha != txtCaptcha.Text)
            {
                HandleLoginFailure("Неверный код подтверждения!");
                GenerateCaptcha();
                return false;
            }
            return true;
        }

        private void HandleLoginFailure(string message)
        {
            ShowError(message);
            _loginAttempts++;
        }

        private void CheckCaptchaVisibility()
        {
            if (_loginAttempts >= 3 && !_isCaptchaVisible)
            {
                txtCaptcha.Visibility = Visibility.Visible;
                txtCaptchaInput.Visibility = Visibility.Visible;
                btnRefreshCaptcha.Visibility = Visibility.Visible;
                CodeTxt.Visibility = Visibility.Visible;
                BorderCaptcha.Visibility = Visibility.Visible;
                _isCaptchaVisible = true;
                GenerateCaptcha();
            }
        }

        private void OpenDashboardWindow(Staff user)
        {
            Window dashboardWindow = null;

            if (user.Role != null)
            {
                switch (user.Role.Role_Name)
                {
                    case "Администратор":
                        dashboardWindow = new MainAdminWindow(user);
                        break;
                    case "Менеджер":
                        dashboardWindow = new ManagerWindow(user); // Предполагаю, что тут должен быть ManagerWindow
                        break;
                    case "Горничная":
                        dashboardWindow = new HousemaidWindow(user);
                        break;
                    default:
                        MessageBox.Show($"Роль '{user.Role.Role_Name}' не поддерживается", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
            }

            if (dashboardWindow == null)
            {
                MessageBox.Show("Не удалось определить роль пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dashboardWindow.Show();
        }

        private void ShowError(string message)
        {
            txtError.Text = message;
            txtError.Visibility = Visibility.Visible;

            DoubleAnimation anim = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3)
            };
            txtError.BeginAnimation(OpacityProperty, anim);
        }
    }
}