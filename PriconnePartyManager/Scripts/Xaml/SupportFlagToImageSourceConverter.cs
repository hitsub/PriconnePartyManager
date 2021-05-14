using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class SupportFlagToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = (bool?) value ?? false ;
            var sourceName = string.Empty;
            if (target)
            {
                sourceName = "support_label.png";
            }
            else
            {
                sourceName = "rarity_default.png";
            }
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.None;
            var uri = new Uri($@"pack://application:,,,/Resources/{sourceName}");
            bitmap.UriSource = uri;
            bitmap.EndInit();
            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}