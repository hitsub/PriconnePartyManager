using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PriconnePartyManager.Scripts.Common;
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
        
        public ReactiveCollection<TagViewModel> Tags { get; }
        
        public ReactiveProperty<int> TagCount { get; } = new ReactiveProperty<int>();
        
        public ReactiveProperty<string> Comment { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveProperty<bool> IsExpandComment { get; } = new ReactiveProperty<bool>(false);
        
        public ReactiveProperty<Visibility> IsShowExpandCommentButton { get; } = new ReactiveProperty<Visibility>();
        
        public ReactiveProperty<string> EstimateDamage { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveCommand OnUnSelect { get; set; } = new ReactiveCommand();
        
        public string Id { get; }
        
        public UserParty Party { get; private set; }

        public AttackRouteListElementViewModel(UserParty party, Action<UserParty> onUnSelect)
        {
            Id = party.Id;
            Party = party;
            PartyUnits = new ReactiveCollection<UserUnitViewModel>();
            Tags = new ReactiveCollection<TagViewModel>();

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
            Party = party;
            PartyUnits.Clear();
            PartyUnits.AddRange(party.UserUnits.Select(x => new UserUnitViewModel(x)));
            Comment.Value = party.Comment?.GetFirstLine() ?? string.Empty;
            EstimateDamage.Value = party.EstimateDamage;
            IsShowExpandCommentButton.Value = party.Comment?.GetLineCount() <= 1 ? Visibility.Collapsed : Visibility.Visible;
            Tags.Clear();
            var tags = party.Tags?.Select(x => Database.I.Tags.SingleOrDefault(db => db.Id == x));
            if (tags?.Count() > 0)
            {
                Tags.AddRange(tags.Select(x => new TagViewModel(x)));
                TagCount.Value += tags.Count();
            }
        }
    }
}