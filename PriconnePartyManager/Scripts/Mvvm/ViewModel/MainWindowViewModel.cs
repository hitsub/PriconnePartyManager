using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extensions;
using PriconnePartyManager.Scripts.Mvvm.Common;
using PriconnePartyManager.Windows;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class MainWindowViewModel : BindingBase
    {
        public ReactiveCollection<PartyListElementViewModel> UserParties { get; private set; }
        
        public ReactiveCollection<AttackRouteListElementViewModel> AttackParties { get; }
        
        public ReactiveProperty<string> AttackRouteComment { get; }

        public ReactiveCommand AddParty { get; } = new ReactiveCommand();

        public ReactiveCommand ImportParty { get; } = new ReactiveCommand();
        
        public ReactiveCommand SaveRoute { get; } = new ReactiveCommand();
        
        public ReactiveCommand NewRoute { get; } = new ReactiveCommand();
        
        public ReactiveCommand OpenRoute { get; } = new ReactiveCommand();
        
        private Dictionary<int, int> m_DoublingCheckTable = new Dictionary<int, int>();

        private UserAttackRoute m_CurrentAttackRoute = null;

        public MainWindowViewModel()
        {
            UserParties = new ReactiveCollection<PartyListElementViewModel>();
            UserParties.AddRange(Database.I.UserParties.Select(x => new PartyListElementViewModel(x, OnSelectAttackRoute)));
            
            AttackParties = new ReactiveCollection<AttackRouteListElementViewModel>();
            
            AttackRouteComment = new ReactiveProperty<string>();

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

            SaveRoute.Subscribe(SaveAttackRoute);
            NewRoute.Subscribe(CreateNewAttackRoute);

            Database.I.OnAddUserParty += OnAddUserParty;
            Database.I.OnChangeUserParty += OnChangeUserParty;
            Database.I.OnRemoveUserParty += OnRemoveUserParty;
        }

        ~MainWindowViewModel()
        {
            Database.I.OnAddUserParty -= OnAddUserParty;
            Database.I.OnChangeUserParty -= OnChangeUserParty;
            Database.I.OnRemoveUserParty -= OnRemoveUserParty;
        }

        private void OnAddUserParty(UserParty party)
        {
            UserParties.Add(new PartyListElementViewModel(party, OnSelectAttackRoute));
        }

        private void OnChangeUserParty(UserParty party)
        {
            var userPartyIndex = UserParties.ToList().FindIndex(x => x.Id == party.Id);
            if (userPartyIndex >= 0)
            {
                UserParties[userPartyIndex].UpdateParty(party);
            }
            
            var attackRouteIndex = AttackParties.ToList().FindIndex(x => x.Id == party.Id);
            if (attackRouteIndex >= 0)
            {
                AttackParties[attackRouteIndex].UpdateParty(party);
                CheckDoubling();
            }
        }

        private void OnRemoveUserParty(UserParty party)
        {
            var userPartyIndex = UserParties.ToList().FindIndex(x => x.Id == party.Id);
            if (userPartyIndex >= 0)
            {
                UserParties.RemoveAt(userPartyIndex);
            }
        }

        private void SaveAttackRoute()
        {
            if (AttackParties.Count == 0)
            {
                MessageBox.Show("凸ルートを編成してください");
                return;
            }
            if (m_CurrentAttackRoute == null)
            {
                m_CurrentAttackRoute = new UserAttackRoute(AttackParties.Select(x => x.Party), AttackRouteComment.Value);
            }
            else
            {
                m_CurrentAttackRoute.Save(AttackParties.Select(x => x.Party), AttackRouteComment.Value);
            }
            Database.I.SaveAttackRoute(m_CurrentAttackRoute);
            MessageBox.Show("保存しました");
        }

        private void CreateNewAttackRoute()
        {
            if (m_CurrentAttackRoute != null || AttackParties.Count > 0)
            {
                var res = MessageBox.Show("未保存の凸ルートは破棄されます。よろしいですか？", "確認", MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            AttackParties.Clear();
            m_CurrentAttackRoute = null;
            AttackRouteComment.Value = string.Empty;
            foreach (var vm in UserParties)
            {
                vm.IsSelectedRoute.Value = false;
            }
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
            //キャラ重複チェック
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
            
            //サポキャラ重複チェック
            foreach (var attackPartyViewModel in AttackParties)
            {
                var supportIds = attackPartyViewModel.PartyUnits.Where(x => x.IsSupport.Value).Select(x => x.UserUnit.UnitId).ToArray();
                if (supportIds.Length < 2)
                {
                    continue;
                }
                foreach (var userUnitViewModel in attackPartyViewModel.PartyUnits)
                {
                    if (userUnitViewModel.UserUnit.IsSupport)
                    {
                        userUnitViewModel.IsDoublingSupport.Value = supportIds.Contains(userUnitViewModel.UserUnit.UnitId);
                    }
                }
            }
        }
    }
}