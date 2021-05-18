using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extension;
using PriconnePartyManager.Scripts.Extensions;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class AttackRouteListElementViewModel : BindingBase
    {

        public ReactiveCollection<UserUnitViewModel> PartyUnits { get; }
        
        public ReactiveProperty<string> Comment { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveProperty<bool> IsExpandComment { get; } = new ReactiveProperty<bool>(false);
        
        public ReactiveProperty<Visibility> IsShowExpandCommentButton { get; } = new ReactiveProperty<Visibility>();
        
        public ReactiveProperty<string> EstimateDamage { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveCommand OnUnSelect { get; set; } = new ReactiveCommand();
        
        public string Id { get; }

        public AttackRouteListElementViewModel(UserParty party, Action<UserParty> onUnSelect)
        {
            Id = party.Id;
            PartyUnits = new ReactiveCollection<UserUnitViewModel>();

            IsExpandComment.Subscribe(x =>
            {
                if (x)
                {
                    Comment.Value = party.Comment;
                }
                else
                {
                    Comment.Value = party.Comment?.GetFirstLine() ?? string.Empty;
                }
            });

            OnUnSelect.Subscribe(() =>
            {
                onUnSelect?.Invoke(party);
            });

            UpdateParty(party);
        }

        public void UpdateParty(UserParty party)
        {
            PartyUnits.Clear();
            PartyUnits.AddRange(party.UserUnits.Select(x => new UserUnitViewModel(x)));
            Comment.Value = party.Comment?.GetFirstLine() ?? string.Empty;
            EstimateDamage.Value = party.EstimateDamage;
            IsShowExpandCommentButton.Value = party.Comment?.GetLineCount() <= 1 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}