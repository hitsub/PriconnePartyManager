using System;
using System.Windows;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;

namespace PriconnePartyManager.Windows
{
    public partial class DownloadUnitIcon : Window
    {
        public bool IsClosed { get; private set; }
        public DownloadUnitIcon()
        {
            InitializeComponent();
            DataContext = new DownloadIconViewModel(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            IsClosed = true;
        }
    }
}