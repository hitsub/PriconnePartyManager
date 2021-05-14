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
        public ReactiveCommand AddParty { get; } = new ReactiveCommand();
        
        private readonly ObservableCollection<UserParty> m_PartyUnitsCollection;

        public MainWindowViewModel()
        {
            m_PartyUnitsCollection = new ObservableCollection<UserParty>(Database.I.UserParties);
            UserParties = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new PartyListElementViewModel(x));

            AddParty.Subscribe(() =>
            {
                var editWindow = new EditParty();
                editWindow.Show();
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
    }
}