using PriconnePartyManager.Scripts.Mvvm.Common;
using PriconnePartyManager.Windows;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class MainWindowViewModel : BindingBase
    {
        public ReactiveCommand AddParty { get; } = new ReactiveCommand();

        public MainWindowViewModel()
        {
            AddParty.Subscribe(() =>
            {
                var editWindow = new EditParty();
                editWindow.Show();
            });
        }
    }
}