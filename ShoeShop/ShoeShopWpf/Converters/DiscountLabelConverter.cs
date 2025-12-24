using System.Globalization;
using System.Windows.Data;

namespace ShoeShopWpf.Converters
{
    public class DiscountLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int discount && discount > 0)
                return "Действующая скидка";
            return "Нет скидки";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
