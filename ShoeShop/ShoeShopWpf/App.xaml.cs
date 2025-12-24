using ShoeShopWpf.Views;
using System.Windows;

namespace ShoeShopWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("ru-RU");
                System.Threading.Thread.CurrentThread.CurrentUICulture =
                    new System.Globalization.CultureInfo("ru-RU");

                // Настройка цен в рублях
                var culture = new System.Globalization.CultureInfo("ru-RU");
                culture.NumberFormat.CurrencySymbol = "₽";
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

                var loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска: {ex.Message}");
                Shutdown();
            }
        }
    }

}
