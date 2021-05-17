using System;
using System.Collections.ObjectModel;
using System.Linq;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.Common;
using PriconnePartyManager.Windows;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class MainWindowViewModel : BindingBase
    {
        public ReadOnlyReactiveCollection<PartyListElementViewModel> UserParties { get; private set; }
        
        public ReadOnlyReactiveCollection<AttackRouteListElementViewModel> AttackParties { get; }
        public ReactiveCommand AddParty { get; } = new ReactiveCommand();

        public ReactiveCommand ImportParty { get; } = new ReactiveCommand();
        
        private readonly ObservableCollection<UserParty> m_PartyUnitsCollection;
        private readonly ObservableCollection<UserParty> m_AttackPartyCollection;

        public MainWindowViewModel()
        {
            m_PartyUnitsCollection = new ObservableCollection<UserParty>(Database.I.UserParties);
            UserParties = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new PartyListElementViewModel(x, OnSelectAttackRoute));
            
            m_AttackPartyCollection = new ObservableCollection<UserParty>();
            AttackParties = m_AttackPartyCollection.ToReadOnlyReactiveCollection(x => new AttackRouteListElementViewModel(x, OnUnSelectAttackRoute));

            AddParty.Subscribe(() =>
            {
                var editWindow = new EditParty();
                editWindow.Show();
            });

            ImportParty.Subscribe(() =>
            {
                var importWindow = new ImportParty();
                importWindow.Show();
            });

            Database.I.OnAddUserParty += OnAddParty;
            Database.I.OnChangeUserParty += OnChangePaty;
            Database.I.OnRemoveUserParty += OnRemoveParty;
        }

        ~MainWindowViewModel()
        {
            Database.I.OnAddUserParty -= OnAddParty;
            Database.I.OnChangeUserParty -= OnChangePaty;
            Database.I.OnRemoveUserParty -= OnRemoveParty;
        }

        private void OnAddParty(UserParty party)
        {
            m_PartyUnitsCollection.Add(party);
        }

        private void OnChangePaty(UserParty party)
        {
            var collection = m_PartyUnitsCollection.ToList();
            var index = m_PartyUnitsCollection.ToList().FindIndex(x => x.Id == party.Id);
            m_PartyUnitsCollection[index] = party;
        }

        private void OnRemoveParty(UserParty party)
        {
            m_PartyUnitsCollection.Remove(party);
        }

        private void OnSelectAttackRoute(UserParty party, bool isSelect)
        {
            if (isSelect)
            {
                m_AttackPartyCollection.Add(party);
            }
            else
            {
                m_AttackPartyCollection.Remove(party);
            }
        }

        private void OnUnSelectAttackRoute(UserParty party)
        {
            m_AttackPartyCollection.Remove(party);
            var userParty = UserParties.SingleOrDefault(x => x.Party.Value.Id == party.Id);
            //凸ルート編成後にユーザーパーティを消す場合があるので必ずチェックする
            if (userParty != null)
            {
                userParty.IsSelectedRoute.Value = false;
            }
            
        }
    }
}