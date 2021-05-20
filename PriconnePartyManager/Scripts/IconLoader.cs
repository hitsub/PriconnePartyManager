using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace PriconnePartyManager.Scripts
{
	public class IconLoader
	{
		private const int RarityCoefficient = 10;
		private Dictionary<string, BitmapImage> m_BitmapCache;

		public IconLoader()
		{
			m_BitmapCache = new Dictionary<string, BitmapImage>();
		}
		
		public BitmapImage LoadIcon(int unitId, bool isUnlockRarity6)
		{
			var maxRarity = isUnlockRarity6 ? 6 : 3;
			return LoadIcon(unitId, maxRarity);
		}

		private BitmapImage LoadIcon(int unitId, int maxRarity)
		{
			var fileName = (unitId + maxRarity * RarityCoefficient).ToString();
			if (!File.Exists($@"./data/icons/{fileName}.png"))
			{
				return null;
			}

			var cache = m_BitmapCache.SingleOrDefault(x => x.Key == fileName).Value;
			if (cache != null)
			{
				return cache;
			}
			var bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.CacheOption = BitmapCacheOption.OnLoad;
			bitmap.CreateOptions = BitmapCreateOptions.None;
			var uri = new Uri(System.IO.Path.GetFullPath($@"./data/icons/{fileName}.png"));
			bitmap.UriSource = uri;
			bitmap.EndInit();
			
			m_BitmapCache.Add(fileName, bitmap);
			return bitmap;
		}
	}
}
