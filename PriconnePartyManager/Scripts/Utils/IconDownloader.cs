using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using Imazen.WebP;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.Sql;

namespace PriconnePartyManager.Scripts.Utils
{
    public class IconDownloader
    {
        private const int RarityCoefficient = 10;
        private const string Url = "http://hitsub.net/icon/unit/{0}.webp";
        private const string DownloadLocation = "./data/icons/";
        private const int MaxDownload = 250;

        /// <summary> DL必要数 </summary>
        public int RequiredDownloadCount { get; }

        public event Action OnCompleteDownload;

        public IconDownloader()
        {
            var units = Database.I.Units;
            foreach (var unit in units)
            {
                var name = (unit.Id + (unit.IsUnlockRarity6 ? 6 : 3) * RarityCoefficient).ToString();
                if (File.Exists($"{DownloadLocation}{name}.png"))
                {
                    continue;
                }
                RequiredDownloadCount++;
            }
        }

        public void DownloadIcons()
        {
            var units = Database.I.Units;
            foreach (var unit in units)
            {
                var name = (unit.Id + (unit.IsUnlockRarity6 ? 6 : 3) * RarityCoefficient).ToString();

                //既にあればスルーする
                if (File.Exists($"{DownloadLocation}{name}.png"))
                {
                    continue;
                }
                //未変換だったら変換だけする
                if (File.Exists($"{DownloadLocation}{name}.webp"))
                {
                    ConvertWebpToPng(name);
                    continue;
                }
                
                var webClient = new WebClient();
                webClient.DownloadFileCompleted += (sender, args) =>
                {
                    ConvertWebpToPng(name);
                    File.Delete($"{DownloadLocation}{name}.webp");
                    OnCompleteDownload?.Invoke();
                };
                webClient.DownloadFileAsync(new Uri(string.Format(Url, name)), $"{DownloadLocation}{name}.webp");
            }
        }

        private void ConvertWebpToPng(string name, bool isDeleteInputFile = true)
        {
            var stream = File.Open($"{DownloadLocation}{name}.webp", FileMode.Open);
            var decoder = new SimpleDecoder();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            var bmp = decoder.DecodeFromBytes(bytes, bytes.Length);
            bmp.Save($"{DownloadLocation}{name}.png", ImageFormat.Png);
            bmp.Dispose();
            stream.Dispose();

            if (isDeleteInputFile)
            {
                File.Delete($"{DownloadLocation}{name}.webp");
            }
        }
    }
}