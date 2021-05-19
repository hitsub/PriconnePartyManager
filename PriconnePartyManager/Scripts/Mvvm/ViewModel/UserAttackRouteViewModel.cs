using System.Linq;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class UserAttackRouteViewModel : BindingBase
    {
        public ReactiveCollection<AttackRouteListElementViewModel> AttackParties { get; }

        public UserAttackRouteViewModel(UserAttackRoute route)
        {
            AttackParties = new ReactiveCollection<AttackRouteListElementViewModel>();
        }
    }
}