using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PriconnePartyManager.Scripts.Enum;

namespace PriconnePartyManager.Scripts.Xaml
{
    public class RankToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var target = (UnitRank?) value ?? UnitRank.None ;
            string sourceName;
            switch (target)
            {
                case UnitRank.None:
                    sourceName = "rank_default.png";
                    break;
                case UnitRank.Rank7:
                case UnitRank.Rank8:
                case UnitRank.Rank9:
                case UnitRank.Rank10:
                    sourceName = "rank_yellow.png";
                    break;
                case UnitRank.Rank11:
                case UnitRank.Rank12:
                case UnitRank.Rank13:
                case UnitRank.Rank14:
                case UnitRank.Rank15:
                case UnitRank.Rank16:
                case UnitRank.Rank17:
                    sourceName = "rank_purple.png";
                    break;
                case UnitRank.Rank18:
                case UnitRank.Rank19:
                case UnitRank.Rank20:
                    sourceName = "rank_red.png";
                    break;
                default:
                    sourceName = "rank_default.png";
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