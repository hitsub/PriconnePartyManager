using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Windows.UI.Xaml.Media;
using PriconnePartyManager.Scripts.Enum;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class RarityToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = (UnitRarity?) value ?? UnitRarity.None ;
            var sourceName = string.Empty;
            switch (target)
            {
                case UnitRarity.None:
                    sourceName = "rarity_default.png";
                    break;
                case UnitRarity.Rarity3:
                    sourceName = "rarity_3.png";
                    break;
                case UnitRarity.Rarity4:
                    sourceName = "rarity_4.png";
                    break;
                case UnitRarity.Rarity5:
                    sourceName = "rarity_5.png";
                    break;
                case UnitRarity.Rarity6:
                    sourceName = "rarity_6.png";
                    break;
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