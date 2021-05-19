using System;
using System.Linq;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extensions;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class UserAttackRouteViewModel : BindingBase
    {
        public ReactiveCollection<AttackRouteListElementViewModel> AttackParties { get; }
        
        public ReactiveProperty<string> Comment { get; }

        public ReactiveCommand OpenRoute { get; set; }
        
        public ReactiveCommand DeleteRoute { get; set; }
        
        public string Id { get; private set; }

        public UserAttackRouteViewModel(UserAttackRoute route, Action<UserAttackRoute> onOpenRoute)
        {
            Id = route.Id;
            
            AttackParties = new ReactiveCollection<AttackRouteListElementViewModel>();
            AttackParties.AddRange(route.RouteParties.Select(x => new AttackRouteListElementViewModel(x, null)));
            
            Comment = new ReactiveProperty<string>(route.Comment);

            OpenRoute = new ReactiveCommand();
            OpenRoute.Subscribe(() => onOpenRoute?.Invoke(route));
            
            DeleteRoute = new ReactiveCommand();
            DeleteRoute.Subscribe(() => Database.I.RemoveAttackRoute(route));

            Comment.AddTo(m_Disposables);
        }
    }
}