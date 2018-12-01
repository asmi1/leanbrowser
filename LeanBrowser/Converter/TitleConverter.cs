using System;
using System.Globalization;
using System.Windows.Data;

namespace LeanBrowser.Converter
{
    public class TitleConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? "New tab";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
