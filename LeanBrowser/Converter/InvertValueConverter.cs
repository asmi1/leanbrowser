using System;
using System.Globalization;
using System.Windows.Data;

namespace LeanBrowser.Converter
{
    public class InvertValueConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return -(double)value;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
