using Microsoft.EntityFrameworkCore;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShoeShopWpf.Views
{
    /// <summary>
    /// Окно выбора товара для осуществления дальнейших действий
    /// </summary>
    public partial class ProductWindow : Window
    {
        private List<Product> _allProducts;
        private List<Product> _selectedProducts;
        private string _mode;

        public List<Product> SelectedProducts => _selectedProducts; 

        public ProductWindow() : this("delete")
        {
        }

        public ProductWindow(string mode)
        {
            InitializeComponent();
            _mode = mode;

            ConfigureForMode(); 
            LoadData(); 
        }

        // Настройка интерфейса в зависимости от режима
        private void ConfigureForMode()
        {
            if (_mode == "edit")
            {
                Title = "Выбор товара для редактирования";

                var titleBlock = FindName("TitleTextBlock") as TextBlock;
                var subtitleBlock = FindName("SubtitleTextBlock") as TextBlock;
                var continueBtn = FindName("ContinueButton") as Button;

                if (titleBlock != null)
                    titleBlock.Text = "Выберите товар для редактирования";

                if (subtitleBlock != null)
                    subtitleBlock.Text = "Выберите ОДИН товар для изменения";

                if (continueBtn != null)
                    continueBtn.Content = "Редактировать";

                var hintText = new TextBlock
                {
                    Text = "Для редактирования выберите только один товар",
                    FontSize = 11,
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.DimGray,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                if (ProductsDataGrid != null)
                {
                    var headerStackPanel = ProductsDataGrid.Parent as Grid;
                    if (headerStackPanel != null && headerStackPanel.Children.Count > 0 &&
                        headerStackPanel.Children[0] is StackPanel buttonPanel)
                    {
                        buttonPanel.Children.Insert(2, hintText);
                    }
                }
            }
            else
            {
                Title = "Выбор товаров для удаления";

                var titleBlock = FindName("TitleTextBlock") as TextBlock;
                var subtitleBlock = FindName("SubtitleTextBlock") as TextBlock;
                var continueBtn = FindName("ContinueButton") as Button;

                if (titleBlock != null)
                    titleBlock.Text = "Выберите товары для удаления";

                if (subtitleBlock != null)
                    subtitleBlock.Text = "Отметьте товары, которые нужно удалить";

                if (continueBtn != null)
                    continueBtn.Content = "Продолжить";
            }
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
                        .OrderBy(p => p.Name)
                        .ToList();

                    foreach (var product in _allProducts)
                    {
                        product.IsSelected = false;
                    }

                    ProductsDataGrid.ItemsSource = _allProducts;
                    UpdateSelectedCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void UpdateSelectedCount()
        {
            if (_allProducts == null) return;

            int selectedCount = _allProducts.Count(p => p.IsSelected);

            var selectedCountBlock = FindName("SelectedCountTextBlock") as TextBlock;
            if (selectedCountBlock != null)
            {
                selectedCountBlock.Text = $"Выбрано: {selectedCount}";

                // В режиме редактирования выделяем красным, если выбрано больше одного товара
                if (_mode == "edit" && selectedCount > 1)
                {
                    selectedCountBlock.Text = $"Выбрано: {selectedCount} (выберите только 1 товар)";
                    selectedCountBlock.Foreground = Brushes.Red;
                }
                else
                {
                    selectedCountBlock.Foreground = Brushes.Black;
                }
            }

            var continueBtn = FindName("ContinueButton") as Button;
            if (continueBtn != null)
                continueBtn.IsEnabled = selectedCount > 0;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (_allProducts == null) return;

            // В режиме редактирования нельзя выбрать все товары
            if (_mode == "edit")
            {
                MessageBox.Show("В режиме редактирования нужно выбрать только один товар",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var product in _allProducts)
            {
                product.IsSelected = true;
            }

            ProductsDataGrid.Items.Refresh(); 
            UpdateSelectedCount(); 
        }

        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (_allProducts == null) return;

            // Снимаем выбор со всех товаров
            foreach (var product in _allProducts)
            {
                product.IsSelected = false;
            }

            ProductsDataGrid.Items.Refresh(); 
            UpdateSelectedCount();
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedCount(); 
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedProducts = _allProducts?.Where(p => p.IsSelected).ToList();

            // Проверяем, что есть выбранные товары
            if (_selectedProducts == null || _selectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один товар", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_mode == "edit")
            {
                if (_selectedProducts.Count > 1)
                {
                    MessageBox.Show("Для редактирования выберите только один товар", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DialogResult = true; 
            Close();
        }

        private void ProductsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_mode == "edit" && ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                foreach (var product in _allProducts)
                {
                    product.IsSelected = false;
                }

                selectedProduct.IsSelected = true;
                ProductsDataGrid.Items.Refresh();
                UpdateSelectedCount();

                _selectedProducts = new List<Product> { selectedProduct };
                DialogResult = true;
                Close();
            }
        }
    }
}