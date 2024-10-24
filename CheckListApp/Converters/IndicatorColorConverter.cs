using System.Globalization;

namespace ChecklistApp.Converters
{
    public class IndicatorColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string indicator)
            {
                return indicator == "✓" ? Colors.Green : Colors.Red;
            }
            return Colors.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}