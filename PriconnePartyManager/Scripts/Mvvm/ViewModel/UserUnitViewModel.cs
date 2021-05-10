using System;
using System.Windows.Media.Imaging;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Enum;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class UserUnitViewModel : BindingBase
    {
        public UserUnit UserUnit { get; }

        public ReactiveProperty<BitmapImage> Icon { get; }
        public ReactiveProperty<bool> IsSupport { get; }
        public ReactiveProperty<UnitRank> Rank { get; }
        public ReactiveProperty<UnitRarity> Rarity { get; }

        public UserUnitViewModel(UserUnit userUnit)
        {
            UserUnit = userUnit;
            Icon = new ReactiveProperty<BitmapImage>(UserUnit.Unit.Icon);
            IsSupport = new ReactiveProperty<bool>(UserUnit.IsSupport);
            Rank = new ReactiveProperty<UnitRank>(UserUnit.Rank);
            Rarity = new ReactiveProperty<UnitRarity>(UserUnit.Rarity);

            IsSupport.Subscribe(x => UserUnit.IsSupport = x);
            Rank.Subscribe(x => UserUnit.Rank = x);
            Rarity.Subscribe(x => UserUnit.Rarity = x);
            
        }
    }
}