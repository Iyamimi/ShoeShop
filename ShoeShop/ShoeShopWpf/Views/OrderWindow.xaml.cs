using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;
using System.Windows;

namespace ShoeShopWpf.Views
{
    /// <summary>
    /// Окно заказов
    /// </summary>
    public partial class OrderWindow : Window
    {
        private readonly User _currentUser;

        public User CurrentUser => _currentUser;

        public OrderWindow(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;

            // Настройка интерфейса в зависимости от роли пользователя
            if (_currentUser.Role?.RoleName == "Клиент")
            {
                Title = "Мои заказы";
                TitleTextBlock.Text = "Мои заказы";
                CustomerColumn.Visibility = Visibility.Collapsed;
                UserRoleTextBlock.Text = "Пользователь";
            }
            else
            {
                Title = "Все заказы";
                TitleTextBlock.Text = "Все заказы";
                CustomerColumn.Visibility = Visibility.Visible; // Показываем колонку с клиентом
                UserRoleTextBlock.Text = _currentUser.Role?.RoleName ?? "Менеджер";
            }

            LoadOrders();
        }

        // Загрузка заказов из базы данных
        private void LoadOrders()
        {
            try
            {
                using (var context = new ShoeShopDbContext())
                {
                    IQueryable<Order> query = context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.ArticleNavigation)
                        .Include(o => o.Customer)
                        .OrderByDescending(o => o.OrderDate);

                    if (_currentUser.Role?.RoleName == "Клиент")
                    {
                        var customer = context.Customers
                            .FirstOrDefault(c => c.FullName == _currentUser.FullName);

                        if (customer != null)
                        {
                            query = query.Where(o => o.CustomerId == customer.CustomerId);
                        }
                    }

                    var orders = query.ToList();

                    // Рассчитываем общую стоимость каждого заказа
                    foreach (var order in orders)
                    {
                        decimal totalPrice = 0;
                        foreach (var orderItem in order.OrderItems)
                        {
                            var product = orderItem.ArticleNavigation;
                            if (product != null)
                            {
                                // Учитываем скидку при расчете стоимости
                                decimal itemPrice = product.Price * (100 - product.Discount) / 100;
                                totalPrice += itemPrice * orderItem.Quantity;
                            }
                        }
                        order.TotalPrice = totalPrice;
                    }

                    OrdersDataGrid.ItemsSource = orders;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
