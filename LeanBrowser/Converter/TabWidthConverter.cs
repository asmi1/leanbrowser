using System;
using System.Globalization;
using System.Windows.Data;

namespace LeanBrowser.Converter
{
    public class TabWidthConverter : IMultiValueConverter
    {

        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int tabCount = (int) values[0];
            Double panelWidth = (Double) values[1];
            Double tabWidth = panelWidth / tabCount;
            return (Double) tabWidth;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
