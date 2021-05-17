using System;
using System.Collections.ObjectModel;
using System.Windows;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extension;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class AttackRouteListElementViewModel : BindingBase
    {

        public ReadOnlyReactiveCollection<UserUnitViewModel> PartyUnits { get; }
        
        public ReactiveProperty<string> Comment { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveProperty<bool> IsExpandComment { get; } = new ReactiveProperty<bool>(false);
        
        public ReactiveProperty<Visibility> IsShowExpandCommentButton { get; } = new ReactiveProperty<Visibility>();
        
        public ReactiveProperty<string> EstimateDamage { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveCommand OnUnSelect { get; set; } = new ReactiveCommand();
        
        private readonly ObservableCollection<UserUnit> m_PartyUnitsCollection;
        
        public string Id { get; }

        public AttackRouteListElementViewModel(UserParty party, Action<UserParty> onUnSelect)
        {
            Id = party.Id;
            m_PartyUnitsCollection = new ObservableCollection<UserUnit>(party.UserUnits);
            PartyUnits = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new UserUnitViewModel(x));

            Comment.Value = party.Comment?.GetFirstLine() ?? string.Empty;

            EstimateDamage.Value = party.EstimateDamage;

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

            IsShowExpandCommentButton.Value = party.Comment?.GetLineCount() <= 1 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}