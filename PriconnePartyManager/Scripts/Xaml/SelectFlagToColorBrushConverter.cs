using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class SelectFlagToColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = (bool?) value ?? true;
            return flag ? Brushes.LightGray : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}