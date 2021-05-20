using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.Mvvm.Common;
using PriconnePartyManager.Scripts.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class DownloadIconViewModel : BindingBase
    {
        public ReactiveProperty<int> DownloadNum { get; }
        public ReactiveProperty<int> DownloadCompleteNum { get; }

        public DownloadIconViewModel(Window window)
        {
            var downloader = new IconDownloader();
            if (downloader.RequiredDownloadCount == 0)
            {
                MessageBox.Show("キャラアイコンはすべて揃っています。\n不足している場合、先にデータベースの更新を行ってください。");
                window.Close();
                return;
            }
            
            DownloadNum = new ReactiveProperty<int>();
            DownloadNum.Value = downloader.RequiredDownloadCount;

            DownloadCompleteNum = new ReactiveProperty<int>();
            downloader.OnCompleteDownload += () =>
            {
                DownloadCompleteNum.Value++;
                if (DownloadCompleteNum.Value >= DownloadNum.Value)
                {
                    Database.I.RefreshUnitIcons();
                    MessageBox.Show("アイコンのダウンロードが完了しました。\nこのソフトを再起動してください。");
                    window.Close();
                }
            };
            
            DownloadNum.AddTo(m_Disposables);
            
            downloader.DownloadIcons();
        }
    }
}