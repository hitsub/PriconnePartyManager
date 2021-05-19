using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using Imazen.WebP;
using PriconnePartyManager.Scripts.Sql;

namespace PriconnePartyManager.Scripts.Utils
{
    public class IconDownloader
    {
        private const int RarityCoefficient = 10;
        private const string Url = "http://hitsub.net/icon/unit/{0}.webp";
        private const string DownloadLocation = "./data/icons/";
        private const int MaxDownload = 250;

        public void DownloadIcons()
        {
            var sql = new SqlConnector();
            var ids = sql.UnitProfiles.Select(x => x.UnitId).ToArray();
            var rarity6Ids = sql.UnlockRarity6.Where(x => x.UnlockLevel > 0).Select(x => x.UnitId).Distinct().ToArray();
            var playables = sql.UnitPlayables.Select(x => x.UnitId).Distinct().ToArray();
            sql.Dispose();

            var count = 0;
            var downloadCount = 0;
            var existCount = 0;
            foreach (var id in ids)
            {
                if (count > MaxDownload)
                {
                    MessageBox.Show("不足アイコンが一定数に達したため、中断されました。\n再度更新を行ってください。");
                    break;
                }
                if (!playables.Contains(id))
                {
                    existCount++;
                    continue;
                }
                var isUnlockRarity6 = rarity6Ids.Contains(id);
                var name = (id + (isUnlockRarity6 ? 6 : 3) * RarityCoefficient).ToString();

                //既にあればスルーする
                if (File.Exists($"{DownloadLocation}{name}.png"))
                {
                    existCount++;
                    continue;
                }
                //未変換だったら変換だけする
                if (File.Exists($"{DownloadLocation}{name}.webp"))
                {
                    ConvertWebpToPng(name);
                    existCount++;
                    continue;
                }
                
                var webClient = new WebClient();
                webClient.DownloadFileCompleted += (sender, args) =>
                {
                    ConvertWebpToPng(name);
                    File.Delete($"{DownloadLocation}{name}.webp");
                    downloadCount--;
                    if (downloadCount == 0)
                    {
                        MessageBox.Show("キャラアイコンの更新が終わりました。このソフトを再起動してください。");
                    }
                };
                webClient.DownloadFileAsync(new Uri(string.Format(Url, name)), $"{DownloadLocation}{name}.webp");
                
                count++;
                downloadCount++;
            }

            if (existCount == ids.Length)
            {
                MessageBox.Show("キャラアイコンはすべて揃っています。\n不足している場合、先にデータベースの更新を行ってください。");
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