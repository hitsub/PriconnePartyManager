using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
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
        public ReactiveProperty<string> RankView { get; }
        public ReactiveProperty<Visibility> IsShowRank { get; }
        public ReactiveProperty<UnitRarity> Rarity { get; }
        
        /// <summary>キャラ重複しているか </summary>
        public ReactiveProperty<bool> IsDoubling { get; }
        
        /// <summary>同一編成内にサポキャラが複数いるか </summary>
        public ReactiveProperty<bool> IsDoublingSupport { get; }

        public UserUnitViewModel(UserUnit userUnit)
        {
            UserUnit = userUnit;
            Icon = new ReactiveProperty<BitmapImage>(UserUnit.Unit.Icon);
            IsSupport = new ReactiveProperty<bool>(UserUnit.IsSupport);
            Rank = new ReactiveProperty<UnitRank>(UserUnit.Rank);
            RankView = new ReactiveProperty<string>(string.Empty);
            IsShowRank = new ReactiveProperty<Visibility>();
            Rarity = new ReactiveProperty<UnitRarity>(UserUnit.Rarity);
            IsDoubling = new ReactiveProperty<bool>(false);
            IsDoublingSupport = new ReactiveProperty<bool>(false);

            IsSupport.Subscribe(x => UserUnit.IsSupport = x);
            Rank.Subscribe(x =>
            {
                UserUnit.Rank = x;
                UpdateRankString(x);
            });
            Rarity.Subscribe(x => UserUnit.Rarity = x);
            
            UpdateRankString(Rank.Value);
        }

        private void UpdateRankString(UnitRank rank)
        {
            var field = rank.GetType().GetField(rank.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>(false);
            RankView.Value = attribute != null ? attribute.Description : rank.ToString();

            IsShowRank.Value = rank != UnitRank.None ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}