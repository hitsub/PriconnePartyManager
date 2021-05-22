using System;
using System.Windows;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.ViewModel;

namespace PriconnePartyManager.Windows
{
    public partial class OpenAttackRoute : Window
    {
        public OpenAttackRoute(Action<UserAttackRoute> onOpenAttackRoute)
        {
            InitializeComponent();
            DataContext = new OpenAttackRouteViewModel(onOpenAttackRoute, this);
        }
    }
}