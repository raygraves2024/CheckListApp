using System.Globalization;
using Microsoft.Maui.Controls;

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
            else if (value is bool boolValue)
            {
                return boolValue ? Colors.Green : Colors.Red;
            }
            return Colors.Red; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
