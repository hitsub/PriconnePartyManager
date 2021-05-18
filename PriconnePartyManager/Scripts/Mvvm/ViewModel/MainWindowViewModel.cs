using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
        
        public ReactiveCollection<AttackRouteListElementViewModel> AttackParties { get; }
        public ReactiveCommand AddParty { get; } = new ReactiveCommand();

        public ReactiveCommand ImportParty { get; } = new ReactiveCommand();
        
        private readonly ObservableCollection<UserParty> m_PartyUnitsCollection;
        private readonly ObservableCollection<UserParty> m_AttackPartyCollection;
        private Dictionary<int, int> m_DoublingCheckTable = new Dictionary<int, int>();

        public MainWindowViewModel()
        {
            m_PartyUnitsCollection = new ObservableCollection<UserParty>(Database.I.UserParties);
            UserParties = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new PartyListElementViewModel(x, OnSelectAttackRoute));
            
            AttackParties = new ReactiveCollection<AttackRouteListElementViewModel>();

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
                AttackParties.Add(new AttackRouteListElementViewModel(party, OnUnSelectAttackRoute));
            }
            else
            {
                var target = AttackParties.SingleOrDefault(x => x.Id == party.Id);
                AttackParties.Remove(target);
            }
            CheckDoubling();
        }

        private void OnUnSelectAttackRoute(UserParty party)
        {
            var target = AttackParties.SingleOrDefault(x => x.Id == party.Id);
            var userParty = UserParties.SingleOrDefault(x => x.Party.Value.Id == party.Id);
            //凸ルート編成後にユーザーパーティを消す場合があるので必ずチェックする
            if (userParty != null)
            {
                userParty.IsSelectedRoute.Value = false;
            }
            if (userParty == null)
            {
                var result = MessageBox.Show("マイパーティ一覧から既にこの編成が消えています。\nこの編成をマイパーティに追加して凸ルートから削除しますか？", "警告",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    var newParty = new UserParty(party.UserUnits, party.Comment, party.EstimateDamage);
                    Database.I.AddParty(newParty);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
                
            }
            AttackParties.Remove(target);
            CheckDoubling();
        }

        private void CheckDoubling()
        {
            m_DoublingCheckTable.Clear();
            foreach (var attackParty in AttackParties)
            {
                foreach (var userUnit in attackParty.PartyUnits)
                {
                    if (userUnit.UserUnit.IsSupport)
                    {
                        continue;
                    }

                    if (m_DoublingCheckTable.ContainsKey(userUnit.UserUnit.UnitId))
                    {
                        m_DoublingCheckTable[userUnit.UserUnit.UnitId]++;
                    }
                    else
                    {
                        m_DoublingCheckTable.Add(userUnit.UserUnit.UnitId, 1);
                    }
                }
            }

            var doublingIds = m_DoublingCheckTable.Where(x => x.Value >= 2).Select(x => x.Key).ToArray();
            foreach (var attackPartyViewModel in AttackParties)
            {
                foreach (var userUnitViewModel in attackPartyViewModel.PartyUnits)
                {
                    if (userUnitViewModel.UserUnit.IsSupport)
                    {
                        continue;
                    }
                    userUnitViewModel.IsDoubling.Value = doublingIds.Contains(userUnitViewModel.UserUnit.UnitId);
                }
            }
        }
    }
}