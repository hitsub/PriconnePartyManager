using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PriconnePartyManager.Scripts.Xaml
{
    /// <summary>
    /// 文字列がNullOrEmptyだったらVisibility.Collapsedを返すコンバーター
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = (string) value ?? string.Empty;
            if (string.IsNullOrEmpty(text))
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}