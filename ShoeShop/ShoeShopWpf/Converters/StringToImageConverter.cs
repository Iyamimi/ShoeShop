using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ShoeShopWpf.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        // Загружается изображение или заглушка
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string imagesFolder = "Images";

                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string fullImagesPath = Path.Combine(baseDirectory, imagesFolder);

                // Создание папки Images если ее нет
                if (!Directory.Exists(fullImagesPath))
                {
                    try
                    {
                        Directory.CreateDirectory(fullImagesPath);
                    }
                    catch
                    {
                        return GetDefaultImage();
                    }
                }

                string imageFileName = value as string;

                if (string.IsNullOrWhiteSpace(imageFileName))
                {
                    return GetDefaultImage();
                }

                imageFileName = imageFileName.Trim();

                string fullPath = Path.Combine(fullImagesPath, imageFileName);

                if (File.Exists(fullPath))
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.UriSource = new Uri(fullPath);
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                    catch
                    {
                        return GetDefaultImage();
                    }
                }

                var files = Directory.GetFiles(fullImagesPath, "*.*");
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileName);

                if (!string.IsNullOrEmpty(fileNameWithoutExtension))
                {
                    foreach (var file in files)
                    {
                        string currentFileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

                        if (currentFileNameWithoutExtension.Equals(fileNameWithoutExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                var bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.UriSource = new Uri(file);
                                bitmap.EndInit();
                                bitmap.Freeze();
                                return bitmap;
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }

                return GetDefaultImage();
            }
            catch
            {
                return GetDefaultImage();
            }
        }
        private BitmapImage GetDefaultImage()
        {
            try
            {
                string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                string defaultImagePath = Path.Combine(imagesFolder, "picture.png");

                if (File.Exists(defaultImagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(defaultImagePath);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }

                // Используется иконка приложения(если не нашлось)
                return new BitmapImage(new Uri("pack://application:,,,/Icon.ico"));
            }
            catch
            {
                return new BitmapImage(new Uri("pack://application:,,,/Icon.ico"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
