using System.Globalization;
using System.Windows.Data;

namespace TodoApp.Converters;

public class ProgressWidthConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is double width && values[1] is double progress)
            return width * Math.Clamp(progress, 0, 1);
        return 0d;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
