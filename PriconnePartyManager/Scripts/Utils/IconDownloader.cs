using System;
using System.IO;
using System.Linq;
using System.Net;
using Imazen.WebP;
using PriconnePartyManager.Scripts.Sql;

namespace PriconnePartyManager.Scripts.Utils
{
    public class IconDownloader
    {
        private const int RarityCoefficient = 10;
        private const string Url = "https://redive.estertion.win/icon/unit/{0}.webp";
        private const string DownloadLocation = "./data/icons/";
        private const int MaxDownload = 50;

        public void DownloadIcons()
        {
            var sql = new SqlConnector();
            var ids = sql.UnitProfiles.Select(x => x.UnitId).ToArray();
            var rarity6Ids = sql.UnlockRarity6.Where(x => x.UnlockLevel > 0).Select(x => x.UnitId).Distinct().ToArray();
            var playables = sql.UnitPlayables.Select(x => x.UnitId).Distinct().ToArray();
            sql.Dispose();

            var count = 0;
            foreach (var id in ids)
            {
                if (count > MaxDownload)
                {
                    break;
                }
                if (!playables.Contains(id))
                {
                    continue;
                }
                var isUnlockRarity6 = rarity6Ids.Contains(id);
                var name = (id + (isUnlockRarity6 ? 6 : 3) * RarityCoefficient).ToString();

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
                };
                webClient.DownloadFileAsync(new Uri(string.Format(Url, name)), $"{DownloadLocation}{name}.webp");
                
                count++;
            }
        }

        private void ConvertWebpToPng(string name, bool isDeleteInputFile = true)
        {
            var stream = File.Open($"{DownloadLocation}{name}.webp", FileMode.Open);
            var decoder = new SimpleDecoder();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            //var bmp = decoder.DecodeFromBytes(bytes, bytes.Length);
            //bmp.Save($"{DownloadLocation}{name}.png", ImageFormat.Png);
            //bmp.Dispose();
            stream.Dispose();

            if (isDeleteInputFile)
            {
                File.Delete($"{DownloadLocation}{name}.webp");
            }
        }
    }
}