using System;
using System.Windows;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;

namespace PriconnePartyManager.Windows
{
    public partial class AddTagWindow : Window
    {
        public AddTagWindow(Action<Tag> onAddTag)
        {
            InitializeComponent();
            DataContext = new AddTagWindowViewModel(onAddTag, this);
        }
    }
}