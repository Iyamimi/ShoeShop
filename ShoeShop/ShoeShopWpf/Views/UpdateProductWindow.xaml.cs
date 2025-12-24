using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ShoeShopLibrary.Contexts;
using ShoeShopLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace ShoeShopWpf.Views
{
    /// <summary>
    /// Окно добавления и редактирования товаров
    /// </summary>
    public partial class UpdateProductWindow : Window
    {
        private bool _isEditMode = false; 
        private Product _editingProduct = null;
        private string _currentImagePath = null;
        public bool ProductSaved { get; private set; }

        public UpdateProductWindow()
        {
            InitializeComponent();

            var titleBlock = FindName("TitleTextBlock") as System.Windows.Controls.TextBlock;
            var subtitleBlock = FindName("SubtitleTextBlock") as System.Windows.Controls.TextBlock;

            if (titleBlock != null)
                titleBlock.Text = "Добавление нового товара";

            if (subtitleBlock != null)
                subtitleBlock.Text = "Заполните информацию о товаре";

            LoadReferenceData();
            LoadProductImage(null);
        }
        public UpdateProductWindow(Product product) : this()
        {
            _isEditMode = true;
            _editingProduct = product;

            Title = "Редактирование товара";
            var titleBlock = FindName("TitleTextBlock") as System.Windows.Controls.TextBlock;
            var subtitleBlock = FindName("SubtitleTextBlock") as System.Windows.Controls.TextBlock;

            if (titleBlock != null)
                titleBlock.Text = "Редактирование товара";

            if (subtitleBlock != null)
                subtitleBlock.Text = $"Редактирование: {product.Name}";

            LoadProductData();
        }

        private void LoadReferenceData()
        {
            try
            {
                using (var context = new ShoeShopDbContext())
                {
                    var categories = context.Categories
                        .OrderBy(c => c.Name)
                        .ToList();
                    CategoryComboBox.ItemsSource = categories;

                    var manufacturers = context.Manufacturers
                        .OrderBy(m => m.Name)
                        .ToList();
                    ManufacturerComboBox.ItemsSource = manufacturers;

                    var suppliers = context.Suppliers
                        .OrderBy(s => s.Name)
                        .ToList();
                    SupplierComboBox.ItemsSource = suppliers;

                    if (categories.Any()) CategoryComboBox.SelectedIndex = 0;
                    if (manufacturers.Any()) ManufacturerComboBox.SelectedIndex = 0;
                    if (suppliers.Any()) SupplierComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка загрузки справочников: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            if (_editingProduct == null)
                return;

            ArticleTextBox.Text = _editingProduct.Article;
            NameTextBox.Text = _editingProduct.Name;
            DescriptionTextBox.Text = _editingProduct.Description;
            UnitTextBox.Text = _editingProduct.Unit;
            PriceTextBox.Text = _editingProduct.Price.ToString();
            DiscountTextBox.Text = _editingProduct.Discount.ToString();
            QuantityTextBox.Text = _editingProduct.Quantity.ToString();

            LoadProductImage(_editingProduct.Photo); 

            if (CategoryComboBox.ItemsSource != null)
            {
                foreach (var item in CategoryComboBox.Items)
                {
                    if (item is Category category && category.CategoryId == _editingProduct.CategoryId)
                    {
                        CategoryComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (ManufacturerComboBox.ItemsSource != null)
            {
                foreach (var item in ManufacturerComboBox.Items)
                {
                    if (item is Manufacturer manufacturer && manufacturer.ManufacturerId == _editingProduct.ManufacturerId)
                    {
                        ManufacturerComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            if (SupplierComboBox.ItemsSource != null)
            {
                foreach (var item in SupplierComboBox.Items)
                {
                    if (item is Supplier supplier && supplier.SupplierId == _editingProduct.SupplierId)
                    {
                        SupplierComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        // Загрузка изображения товара
        private void LoadProductImage(string imagePath)
        {
            try
            {
                var productImagePreview = FindName("ProductImagePreview") as System.Windows.Controls.Image;
                var photoPathTextBox = FindName("PhotoPathTextBox") as System.Windows.Controls.TextBox;

                if (productImagePreview == null || photoPathTextBox == null)
                    return;

                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    _currentImagePath = imagePath;

                    string fileNameOnly = Path.GetFileName(imagePath);
                    photoPathTextBox.Text = fileNameOnly;

                    string fullPath;

                    if (Path.IsPathRooted(imagePath))
                    {
                        fullPath = imagePath;
                    }
                    else if (imagePath.Contains("\\") || imagePath.Contains("/"))
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        fullPath = Path.Combine(basePath, imagePath);
                    }
                    else
                    {
                        string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                        fullPath = Path.Combine(imagesFolder, imagePath);
                    }

                    if (File.Exists(fullPath))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        productImagePreview.Source = bitmap;
                    }
                    else
                    {
                        productImagePreview.Source = new BitmapImage(new Uri("pack://application:,,,/Icon.ico"));
                    }
                }
                else
                {
                    _currentImagePath = null;
                    productImagePreview.Source = new BitmapImage(new Uri("pack://application:,,,/Icon.ico"));
                    photoPathTextBox.Text = "";
                }
            }
            catch
            {
                var productImagePreview = FindName("ProductImagePreview") as System.Windows.Controls.Image;
                if (productImagePreview != null)
                    productImagePreview.Source = new BitmapImage(new Uri("pack://application:,,,/Icon.ico"));
            }
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*",
                Title = "Выберите изображение товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourceFilePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(sourceFilePath);

                    string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    string destinationFilePath = Path.Combine(imagesFolder, fileName);

                    int counter = 1;
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string fileExtension = Path.GetExtension(fileName);

                    while (File.Exists(destinationFilePath))
                    {
                        string newFileName = $"{fileNameWithoutExt}_{counter}{fileExtension}";
                        destinationFilePath = Path.Combine(imagesFolder, newFileName);
                        counter++;
                    }

                    File.Copy(sourceFilePath, destinationFilePath, true);

                    _currentImagePath = destinationFilePath;

                    var photoPathTextBox = FindName("PhotoPathTextBox") as System.Windows.Controls.TextBox;
                    if (photoPathTextBox != null)
                    {
                        photoPathTextBox.Text = Path.GetFileName(destinationFilePath);
                    }

                    LoadProductImage(destinationFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearImageButton_Click(object sender, RoutedEventArgs e)
        {
            _currentImagePath = null;
            var photoPathTextBox = FindName("PhotoPathTextBox") as System.Windows.Controls.TextBox;
            if (photoPathTextBox != null)
            {
                photoPathTextBox.Text = "";
            }
            LoadProductImage(null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ProductSaved = false;
            DialogResult = false;
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearError(); 

                if (!ValidateForm()) 
                    return;

                using (var context = new ShoeShopDbContext())
                {
                    Product product;

                    if (_isEditMode && _editingProduct != null)
                    {
                        product = context.Products
                            .FirstOrDefault(p => p.Article == _editingProduct.Article);

                        if (product == null)
                        {
                            ShowError("Товар не найден в базе данных");
                            return;
                        }
                    }
                    else
                    {
                        var existingProduct = context.Products
                            .FirstOrDefault(p => p.Article == ArticleTextBox.Text.Trim());

                        if (existingProduct != null)
                        {
                            ShowError($"Товар с артикулом '{ArticleTextBox.Text}' уже существует");
                            ArticleTextBox.Focus();
                            ArticleTextBox.SelectAll();
                            return;
                        }

                        product = new Product(); 
                    }

                    if (_isEditMode && _editingProduct != null)
                    {
                        string oldImagePath = _editingProduct.Photo;
                        DeleteOldImageIfNeeded(oldImagePath, _currentImagePath);
                    }

                    product.Article = ArticleTextBox.Text.Trim();
                    product.Name = NameTextBox.Text.Trim();
                    product.Description = DescriptionTextBox.Text.Trim();
                    product.Unit = UnitTextBox.Text.Trim();
                    product.Price = decimal.Parse(PriceTextBox.Text);
                    product.Discount = int.Parse(DiscountTextBox.Text);
                    product.Quantity = int.Parse(QuantityTextBox.Text);

                    product.Photo = _currentImagePath;

                    if (CategoryComboBox.SelectedItem is Category selectedCategory)
                        product.CategoryId = selectedCategory.CategoryId;

                    if (ManufacturerComboBox.SelectedItem is Manufacturer selectedManufacturer)
                        product.ManufacturerId = selectedManufacturer.ManufacturerId;

                    if (SupplierComboBox.SelectedItem is Supplier selectedSupplier)
                        product.SupplierId = selectedSupplier.SupplierId;

                    if (!_isEditMode)
                    {
                        context.Products.Add(product); 
                    }
                    else
                    {
                        context.Entry(product).State = EntityState.Modified; 
                    }

                    context.SaveChanges();

                    ProductSaved = true; 
                    DialogResult = true; 

                    MessageBox.Show(_isEditMode ? "Товар успешно обновлен!" : "Товар успешно добавлен!",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close();
                }
            }
            catch (DbUpdateException dbEx)
            {
                ShowError($"Ошибка сохранения в базу данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка при сохранении товара: {ex.Message}");
            }
        }

        private void DeleteOldImageIfNeeded(string oldImagePath, string newImagePath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(oldImagePath) &&
                    !string.IsNullOrWhiteSpace(newImagePath) &&
                    oldImagePath != newImagePath)
                {
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при удалении старого изображения: {ex.Message}");
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
            {
                ShowError("Введите артикул товара");
                ArticleTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowError("Введите наименование товара");
                NameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(UnitTextBox.Text))
            {
                ShowError("Введите единицу измерения");
                UnitTextBox.Focus();
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                ShowError("Введите корректную цену (положительное число)");
                PriceTextBox.Focus();
                PriceTextBox.SelectAll();
                return false;
            }

            if (!int.TryParse(DiscountTextBox.Text, out int discount) || discount < 0 || discount > 100)
            {
                ShowError("Скидка должна быть от 0 до 100%");
                DiscountTextBox.Focus();
                DiscountTextBox.SelectAll();
                return false;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                ShowError("Количество не может быть отрицательным");
                QuantityTextBox.Focus();
                QuantityTextBox.SelectAll();
                return false;
            }

            if (CategoryComboBox.SelectedItem == null)
            {
                ShowError("Выберите категорию");
                CategoryComboBox.Focus();
                return false;
            }

            if (ManufacturerComboBox.SelectedItem == null)
            {
                ShowError("Выберите производителя");
                ManufacturerComboBox.Focus();
                return false;
            }

            if (SupplierComboBox.SelectedItem == null)
            {
                ShowError("Выберите поставщика");
                SupplierComboBox.Focus();
                return false;
            }

            return true;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != '.')
                {
                    e.Handled = true; 
                    return;
                }
            }
        }

        private void ShowError(string message)
        {
            var errorTextBlock = FindName("ErrorTextBlock") as System.Windows.Controls.TextBlock;
            var errorBorder = FindName("ErrorBorder") as System.Windows.Controls.Border;

            if (errorTextBlock != null)
                errorTextBlock.Text = message;

            if (errorBorder != null)
                errorBorder.Visibility = Visibility.Visible;
        }

        private void ClearError()
        {
            var errorTextBlock = FindName("ErrorTextBlock") as System.Windows.Controls.TextBlock;
            var errorBorder = FindName("ErrorBorder") as System.Windows.Controls.Border;

            if (errorTextBlock != null)
                errorTextBlock.Text = "";

            if (errorBorder != null)
                errorBorder.Visibility = Visibility.Collapsed;
        }
    }
}
