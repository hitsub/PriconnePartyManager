using System;
using System.Linq;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extensions;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class OpenAttackRouteViewModel : BindingBase
    {
        public ReactiveCollection<UserAttackRouteViewModel> Routes { get; }

        public OpenAttackRouteViewModel(Action<UserAttackRoute> onOpenRoute)
        {
            var routes = Database.I.UserAttackRoutes;
            Routes = new ReactiveCollection<UserAttackRouteViewModel>();
            Routes.AddRange(routes.Select(x => new UserAttackRouteViewModel(x, onOpenRoute)));

            Database.I.OnRemoveUserAttackRoute += OnDeleteAttackRoute;
        }

        ~OpenAttackRouteViewModel()
        {
            Database.I.OnRemoveUserAttackRoute -= OnDeleteAttackRoute;
        }

        private void OnDeleteAttackRoute(UserAttackRoute route)
        {
            var routeIndex = Routes.ToList().FindIndex(x => x.Id == route.Id);
            if (routeIndex >= 0)
            {
                Routes.RemoveAt(routeIndex);
            }
        }
    }
}