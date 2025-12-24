using System.Globalization;
using System.Windows.Data;

namespace ShoeShopWpf.Converters
{
    public class QuantityToTextConverter : IValueConverter
    {
        // Преобразование количества товара в текст
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                if (quantity > 0)
                {
                    return "В наличии";
                }
                else
                {
                    return "Нет в наличии";
                }
            }
            return "Нет данных";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
