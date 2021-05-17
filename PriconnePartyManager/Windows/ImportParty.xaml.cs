using System.Windows;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;

namespace PriconnePartyManager.Windows
{
    public partial class ImportParty : Window
    {
        public ImportParty()
        {
            InitializeComponent();
            DataContext = new ImportPartyViewModel();
        }
    }
}