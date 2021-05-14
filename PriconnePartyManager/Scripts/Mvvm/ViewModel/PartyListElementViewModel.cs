using System;
using System.Collections.ObjectModel;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extension;
using PriconnePartyManager.Scripts.Mvvm.Common;
using PriconnePartyManager.Windows;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    /// <summary>
    /// 凸候補リストの要素
    /// </summary>
    public class PartyListElementViewModel : BindingBase
    {
        /// <summary> 凸ルートとして選ばれているか </summary>
        public ReactiveProperty<bool> IsSelectedRoute = new ReactiveProperty<bool>();
        
        public ReactiveProperty<UserParty> Party { get; } = new ReactiveProperty<UserParty>();
        
        public ReadOnlyReactiveCollection<UserUnitViewModel> PartyUnits { get; private set; }
        
        public ReactiveProperty<string> CommentFirstLine { get; } = new ReactiveProperty<string>(string.Empty);

        public ReactiveCommand EditParty { get; } = new ReactiveCommand();

        public ReactiveCommand OnDelete { get; } = new ReactiveCommand();
        
        private readonly ObservableCollection<UserUnit> m_PartyUnitsCollection;

        public PartyListElementViewModel(UserParty party)
        {
            Party.Value = party;
            
            m_PartyUnitsCollection = new ObservableCollection<UserUnit>(party.UserUnits);
            PartyUnits = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new UserUnitViewModel(x));

            CommentFirstLine.Value = party.Comment?.GetFirstLine() ?? string.Empty;

            EditParty.Subscribe(x =>
            {
                
                var editWindow = new EditParty((UserParty)x);
                editWindow.Show();
            });
            OnDelete.Subscribe(() => Database.I.RemoveParty(party));
        }
    }
}