using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ShoeShopWpf.Views
{
    /// <summary>
    /// Католог товаров
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private List<Product> _allProducts;
        private List<Manufacturer> _manufacturers;
        private DispatcherTimer _searchTimer;

        public MainWindow(User user)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            _currentUser = user;

            Title = $"Магазин обуви";
            UserInfoTextBlock.Text = user.FullName;

            _searchTimer = new DispatcherTimer();
            _searchTimer.Interval = TimeSpan.FromMilliseconds(500);
            _searchTimer.Tick += SearchTimer_Tick;

            // Показываем панель администратора для менеджеров и администраторов
            if (user.Role?.RoleName == "Менеджер" || user.Role?.RoleName == "Администратор")
            {
                AdminControlsPanel.Visibility = Visibility.Visible;
            }

            InitializeFilters();
            LoadData();
            LoadProducts();
        }

        // Фильтры
        private void InitializeFilters()
        {
            SortComboBox.SelectedIndex = 0;
            OnlyDiscountCheckBox.Checked += FilterCheckBox_Changed;
            OnlyDiscountCheckBox.Unchecked += FilterCheckBox_Changed;
            OnlyInStockCheckBox.Checked += FilterCheckBox_Changed;
            OnlyInStockCheckBox.Unchecked += FilterCheckBox_Changed;
        }

       
        // Обработчик изменения фильтров
        private void FilterCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == ManufacturerComboBox || sender == SortComboBox)
            {
                LoadProducts();
            }
        }

        // Обработчик изменения текста в полях поиска
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();
            LoadProducts();
        }

        // Выход из системы
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        // Обработчик сброса фильтров
        private void ResetFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            ManufacturerComboBox.SelectedIndex = 0;
            MaxPriceTextBox.Text = "";
            OnlyDiscountCheckBox.IsChecked = false;
            OnlyInStockCheckBox.IsChecked = false;
            SortComboBox.SelectedIndex = 0;
        }

        // Обработчик добавления товара
        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new UpdateProductWindow()
            {
                Owner = this
            };

            var result = addWindow.ShowDialog();

            if (result == true && addWindow.ProductSaved)
            {
                LoadData();
                LoadProducts();

                MessageBox.Show("Товар успешно добавлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Обработчик редактирования товара
        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selectWindow = new ProductWindow("edit")
            {
                Owner = this
            };

            var result = selectWindow.ShowDialog();

            if (result != true || selectWindow.SelectedProducts == null || selectWindow.SelectedProducts.Count != 1)
            {
                return;
            }

            var productToEdit = selectWindow.SelectedProducts.First();

            var editWindow = new UpdateProductWindow(productToEdit)
            {
                Owner = this
            };

            var editResult = editWindow.ShowDialog();

            if (editResult == true && editWindow.ProductSaved)
            {
                LoadData();
                LoadProducts(); 

                MessageBox.Show("Товар успешно обновлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Обработчик удаления товаров
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            var selectWindow = new ProductWindow()
            {
                Owner = this
            };

            var result = selectWindow.ShowDialog();

            if (result != true || selectWindow.SelectedProducts == null || selectWindow.SelectedProducts.Count == 0)
            {
                return;
            }

            var deleteWindow = new DeleteProductWindow(selectWindow.SelectedProducts, _allProducts)
            {
                Owner = this
            };

            var deleteResult = deleteWindow.ShowDialog();

            if (deleteResult == true && deleteWindow.ProductsDeleted)
            {
                LoadData();
                LoadProducts();

                MessageBox.Show($"Удалено {selectWindow.SelectedProducts.Count} товаров!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != ".")
            {
                e.Handled = true;
            }
        }

        // Обработчик заказа товара
        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем роль пользователя
            if (_currentUser.Role?.RoleName == "Менеджер" || _currentUser.Role?.RoleName == "Администратор")
            {
                MessageBox.Show("Функция заказа доступна только для клиентов",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (sender is Button button && button.Tag is string article)
            {
                var product = _allProducts?.FirstOrDefault(p => p.Article == article);
                if (product != null)
                {
                    if (product.Quantity > 0)
                    {
                        MessageBox.Show($"Товар '{product.Name}' добавлен в заказ!",
                            "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Товар отсутствует на складе",
                            "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        // Обработчик просмотра заказов
        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrderWindow(_currentUser)
            {
                Owner = this
            };
            ordersWindow.ShowDialog();
        }

        // Загрузка данных из базы данных
        private void LoadData()
        {
            try
            {
                using (var context = new ShoeShopDbContext())
                {
                    _allProducts = context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Manufacturer)
                        .Include(p => p.Supplier)
                        .ToList();

                    foreach (var product in _allProducts)
                    {
                        product.IsSelected = false;
                    }

                    _manufacturers = context.Manufacturers
                        .OrderBy(m => m.Name)
                        .ToList();

                    var manufacturersForComboBox = new List<Manufacturer>();
                    manufacturersForComboBox.Add(new Manufacturer
                    {
                        ManufacturerId = 0,
                        Name = "Все производители"
                    });
                    manufacturersForComboBox.AddRange(_manufacturers);

                    ManufacturerComboBox.ItemsSource = manufacturersForComboBox;
                    ManufacturerComboBox.DisplayMemberPath = "Name";
                    ManufacturerComboBox.SelectedValuePath = "ManufacturerId";
                    ManufacturerComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Загрузка и фильтрация товаров
        private void LoadProducts()
        {
            try
            {
                if (_allProducts == null) return;

                var filteredProducts = _allProducts.AsEnumerable();

                // Фильтрация по тексту
                if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    var search = SearchTextBox.Text.ToLower();
                    filteredProducts = filteredProducts.Where(p =>
                        (p.Description != null && p.Description.ToLower().Contains(search)) ||
                        p.Name.ToLower().Contains(search));
                }

                // Фильтрация по производителю
                if (ManufacturerComboBox.SelectedItem is Manufacturer selectedManufacturer)
                {
                    if (selectedManufacturer.ManufacturerId > 0)
                    {
                        filteredProducts = filteredProducts.Where(p =>
                            p.ManufacturerId == selectedManufacturer.ManufacturerId);
                    }
                }

                // Фильтрация по максимальной цене
                if (decimal.TryParse(MaxPriceTextBox.Text, out decimal maxPrice) && maxPrice > 0)
                {
                    filteredProducts = filteredProducts.Where(p => p.DiscountPrice <= maxPrice);
                }

                // Фильтрация товаров со скидкой
                if (OnlyDiscountCheckBox.IsChecked == true)
                {
                    filteredProducts = filteredProducts.Where(p => p.Discount > 0);
                }

                // Фильтрация товаров в наличии
                if (OnlyInStockCheckBox.IsChecked == true)
                {
                    filteredProducts = filteredProducts.Where(p => p.Quantity > 0);
                }

                // Сортировка товаров
                filteredProducts = SortComboBox.SelectedIndex switch
                {
                    0 => filteredProducts.OrderBy(p => p.Name),
                    1 => filteredProducts.OrderBy(p => p.Supplier.Name),
                    2 => filteredProducts.OrderBy(p => p.DiscountPrice),
                    3 => filteredProducts.OrderByDescending(p => p.DiscountPrice),
                    _ => filteredProducts.OrderBy(p => p.Name)
                };

                var productsList = filteredProducts.ToList();

                ProductsItemsControl.ItemsSource = productsList;
                ProductsCountTextBlock.Text = $"Товаров: {productsList.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
