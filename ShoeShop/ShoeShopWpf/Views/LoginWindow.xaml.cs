using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;
using ShoeShopWpf.Views;
using System.Windows;

namespace ShoeShopWpf.Views
{
    /// <summary>
    /// Окно авторизации пользователей
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            LoginTextBox.Focus();
        }
        public User CurrentUser { get; private set; }

        // Кнопка авторизации
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearError();

                if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                {
                    ShowError("Введите логин");
                    LoginTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
                {
                    ShowError("Введите пароль");
                    PasswordTextBox.Focus();
                    return;
                }

                // Работа с базой данных(поик пользователя и его роли)
                using (var context = new ShoeShopDbContext())
                {
                    var user = context.Users
                        .Include(u => u.Role)
                        .FirstOrDefault(u => u.Login == LoginTextBox.Text && u.Password == PasswordTextBox.Password);

                    if (user == null)
                    {
                        ShowError("Неверный логин или пароль");
                        LoginTextBox.Focus();
                        LoginTextBox.SelectAll();
                        return;
                    }

                    CurrentUser = user;

                    this.Hide();

                    var mainWindow = new MainWindow(user);
                    mainWindow.Show();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка авторизации: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void ClearError()
        {
            ErrorTextBlock.Text = "";
            ErrorBorder.Visibility = Visibility.Collapsed;
        }
    }
}
