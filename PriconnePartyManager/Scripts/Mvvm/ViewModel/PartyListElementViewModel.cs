using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Extension;
using PriconnePartyManager.Scripts.Extensions;
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
        public ReactiveProperty<bool> IsSelectedRoute { get; } = new ReactiveProperty<bool>();
        
        public ReactiveProperty<UserParty> Party { get; } = new ReactiveProperty<UserParty>();
        
        public ReactiveCollection<UserUnitViewModel> PartyUnits { get; private set; }
        
        public ReactiveProperty<string> Comment { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveProperty<bool> IsExpandComment { get; } = new ReactiveProperty<bool>(false);
        
        public ReactiveProperty<Visibility> IsShowExpandCommentButton { get; } = new ReactiveProperty<Visibility>();
        
        public ReactiveProperty<string> EstimateDamage { get; } = new ReactiveProperty<string>(string.Empty);

        public ReactiveCommand EditParty { get; } = new ReactiveCommand();

        public ReactiveCommand OnDelete { get; set; } = new ReactiveCommand();

        public ReactiveCommand OnDuplicate { get; set; } = new ReactiveCommand();

        public ReactiveCommand OnExport { get; set; } = new ReactiveCommand();
        
        public ReactiveCommand OnSelect { get; set; } = new ReactiveCommand();
        
        public string Id { get; }

        public PartyListElementViewModel(UserParty party, Action<UserParty, bool> onSelect)
        {
            Id = party.Id;
            Party.Value = party;
            
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

            EditParty.Subscribe(x =>
            {
                
                var editWindow = new EditParty((UserParty)x);
                editWindow.Show();
            });
            OnDelete.Subscribe(() => DeleteParty(party));

            OnExport.Subscribe(() => ExportParty(party));

            OnDuplicate.Subscribe(() => DuplicateParty(party));

            OnSelect.Subscribe(() =>
            {
                IsSelectedRoute.Value = !IsSelectedRoute.Value;
                onSelect?.Invoke(party, IsSelectedRoute.Value);
            });
            
            UpdateParty(party);
        }

        public void UpdateParty(UserParty party)
        {
            PartyUnits.Clear();
            PartyUnits.AddRange(party.UserUnits.Select(x => new UserUnitViewModel(x)));
            
            IsShowExpandCommentButton.Value = party.Comment?.GetLineCount() <= 1 ? Visibility.Collapsed : Visibility.Visible;
            
            Comment.Value = party.Comment?.GetFirstLine() ?? string.Empty;

            EstimateDamage.Value = party.EstimateDamage;
        }

        private void DeleteParty(UserParty party)
        {
            var result = MessageBox.Show("パーティーを削除します。よろしいですか？", "確認", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Database.I.RemoveParty(party);
            }
        }

        private void DuplicateParty(UserParty party)
        {
            var duplicate = new UserParty(party.UserUnits, party.Comment, party.EstimateDamage);
            Database.I.AddParty(duplicate);
        }

        private void ExportParty(UserParty party)
        {
            Clipboard.SetText(party.GetJson());
            MessageBox.Show("クリップボードにパーティー情報をコピーしました。\n画面上のインポートボタンから追加することが出来ます。");
        }
    }
}