using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;
using System.Windows;

namespace ShoeShopWpf.Views
{
    /// <summary>
    /// Окно удаления товаров
    /// </summary>
    public partial class DeleteProductWindow : Window
    {
        private List<Product> _productsToDelete;
        private List<Product> _originalProductList;

        public bool ProductsDeleted { get; private set; }

        public DeleteProductWindow(List<Product> productsToDelete, List<Product> originalProductList)
        {
            InitializeComponent();
            _productsToDelete = productsToDelete ?? new List<Product>();
            _originalProductList = originalProductList ?? new List<Product>();

            LoadProducts();
            UpdateSelectedCount();
        }

        private void LoadProducts()
        {
            ProductsDataGrid.ItemsSource = _productsToDelete;
        }

        private void UpdateSelectedCount()
        {
            SelectedCountTextBlock.Text = $"Удалить: {_productsToDelete.Count} товаров";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ProductsDeleted = false;
            DialogResult = false;
            Close();
        }

        // Обработчикудаления товаров
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_productsToDelete.Count == 0)
            {
                MessageBox.Show("Нет товаров для удаления.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить {_productsToDelete.Count} товаров?\n\n" +
                "Эта операция необратима!",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using (var context = new ShoeShopDbContext())
                {
                    var articleList = _productsToDelete.Select(p => p.Article).ToList();

                    var productsWithOrders = context.OrderItems
                        .Where(oi => articleList.Contains(oi.Article))
                        .Select(oi => oi.Article)
                        .Distinct()
                        .ToList();

                    // Если товары есть в заказах - запрещаем удаление
                    if (productsWithOrders.Any())
                    {
                        var articlesString = string.Join(", ", productsWithOrders);
                        MessageBox.Show(
                            $"Нельзя удалить товары, которые есть в заказах:\n{articlesString}\n\n" +
                            "Сначала удалите связанные заказы.",
                            "Ошибка удаления",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    // Находим товары для удаления в базе данных
                    var productsToRemove = context.Products
                        .Where(p => articleList.Contains(p.Article))
                        .ToList();

                    context.Products.RemoveRange(productsToRemove);
                    context.SaveChanges();

                    _originalProductList.RemoveAll(p => articleList.Contains(p.Article));

                    ProductsDeleted = true;
                    DialogResult = true;

                    MessageBox.Show($"Успешно удалено {productsToRemove.Count} товаров.",
                        "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
            }
            catch (DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка базы данных при удалении: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении товаров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}