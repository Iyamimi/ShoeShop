using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace ShoeShopWpf.Converters
{
    public class DiscountToForegroundConverter : IValueConverter
    {
        // Цвет текста от скидки
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int discount && discount > 15)
            {
                return Brushes.White;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
