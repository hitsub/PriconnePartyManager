using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Enum;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class EditPartyViewModel : BindingBase
    {
        public ReadOnlyReactiveCollection<UnitViewModel> UnitList { get; private set; }

        public ReactiveProperty<IList> SelectedUnit { get; } = new ReactiveProperty<IList>();
        public ReactiveCommand OnChangeSelected { get; } = new ReactiveCommand();

        public ReactiveProperty<ListUnitType> ShowUnitType { get; } = new ReactiveProperty<ListUnitType>(ListUnitType.All);
        
        public ReadOnlyReactiveCollection<UserUnitViewModel> PartyUnits { get; private set; }

        public ReactiveProperty<bool> IsFullParty { get; } = new ReactiveProperty<bool>();
        
        public ReactiveProperty<Visibility> IsVisibleSelected { get; } = new ReactiveProperty<Visibility>(Visibility.Collapsed);
        
        public ReactiveProperty<string> SearchText { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveProperty<string> PartyComment { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveProperty<string> EstimateDamage { get; } = new ReactiveProperty<string>(string.Empty);
        
        public ReactiveCommand OnSearchTextChanged { get; set; } = new ReactiveCommand();

        public ReactiveCommand OnSubmit { get; } = new ReactiveCommand();
        public ReactiveCommand OnCancel { get; } = new ReactiveCommand();

        private readonly ObservableCollection<Unit> m_UnitsCollection;
        private readonly ObservableCollection<UserUnit> m_PartyUnitsCollection;
        private List<UserUnit> m_PartUnits = new List<UserUnit>();
        private ListUnitType m_CurrentUnitType = ListUnitType.All;
        private UserParty m_Party;

        public EditPartyViewModel(Unit[] units, UserParty party = null)
        {
            m_UnitsCollection = new ObservableCollection<Unit>(units);
            UnitList = m_UnitsCollection.ToReadOnlyReactiveCollection(x => new UnitViewModel(x));
            m_PartyUnitsCollection = new ObservableCollection<UserUnit>();
            PartyUnits = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new UserUnitViewModel(x, () => UnselectUserUnit(x)));

            OnCancel.Subscribe(x => CloseWindow((Window)x));
            OnSubmit.Subscribe(x => SaveParty((Window)x));
            
            OnSearchTextChanged.Subscribe(() => { SearchUnit(SearchText.Value); });

            ShowUnitType.Subscribe(OnChangeShowUnitType);
            OnChangeSelected.Subscribe(OnChangeSelectedUnit);

            if (party != null)
            {
                m_Party = party;
                m_PartUnits = party.UserUnits.ToList();
                foreach (var unitViewModel in UnitList)
                {
                    if (m_PartUnits.Any(x => x.UnitId == unitViewModel.Unit.Id))
                    {
                        unitViewModel.SetSelect(true);
                    }
                }
                m_PartyUnitsCollection.Clear();
                foreach (var unit in m_PartUnits)
                {
                    m_PartyUnitsCollection.Add(unit);
                }

                IsFullParty.Value = true;
                IsVisibleSelected.Value = Visibility.Visible;
                PartyComment.Value = party.Comment;
                EstimateDamage.Value = party.EstimateDamage;
            }
        }

        /// <summary>
        /// 編成ユニットの変更
        /// </summary>
        private void OnChangeSelectedUnit()
        {
            var selected = SelectedUnit.Value.Cast<UnitViewModel>().ToArray();
            //追加
            if (m_PartUnits.Count < selected.Count())
            {
                m_PartUnits.Add(new UserUnit(selected.Last().Unit));
                m_PartUnits = m_PartUnits.OrderByDescending(x => x.Unit.Order).ToList();
                m_PartyUnitsCollection.Clear();
                foreach (var unit in m_PartUnits)
                {
                    m_PartyUnitsCollection.Add(unit);
                }
            }
            //削除
            else
            {
                var selectedIds = selected.Select(x => x.Unit.Id).ToArray();
                m_PartUnits = m_PartUnits.Where(x => selectedIds.Contains(x.Unit.Id)).ToList();
                m_PartyUnitsCollection.Clear();
                foreach (var unit in m_PartUnits)
                {
                    m_PartyUnitsCollection.Add(unit);
                }
            }

            IsFullParty.Value = selected.Length == 5;
            IsVisibleSelected.Value = selected.Length != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UnselectUserUnit(UserUnit userUnit)
        {
            var index = m_PartUnits.FindIndex(x => x.UnitId == userUnit.UnitId);
            if (index < 0)
            {
                return;
            }
            m_PartUnits.RemoveAt(index);
            m_PartyUnitsCollection.RemoveAt(index);
            foreach (var unitViewModel in UnitList)
            {
                if (unitViewModel.Unit.Id == userUnit.UnitId)
                {
                    unitViewModel.SetSelect(false);
                    return;
                }
            }
        }

        /// <summary>
        /// 表示用ユニットタイプの変更
        /// </summary>
        private void OnChangeShowUnitType(ListUnitType type)
        {
            m_CurrentUnitType = type;
            
            foreach (var unit in UnitList)
            {
                var isShowUnitType = (int) unit.Unit.Type == (int) m_CurrentUnitType || m_CurrentUnitType == ListUnitType.All;
                if (isShowUnitType && unit.Unit.Name.Contains(SearchText.Value))
                {
                    unit.IsVisibility.Value = Visibility.Visible;
                }
                else
                {
                    unit.IsVisibility.Value = Visibility.Collapsed;
                }
            }
        }

        private void SearchUnit(string name)
        {
            foreach (var unit in UnitList)
            {
                var isShowUnitType = (int) unit.Unit.Type == (int) m_CurrentUnitType || m_CurrentUnitType == ListUnitType.All;
                if (isShowUnitType && unit.Unit.Name.Contains(name))
                {
                    unit.IsVisibility.Value = Visibility.Visible;
                }
                else
                {
                    unit.IsVisibility.Value = Visibility.Collapsed;
                }
            }
        }

        private void CloseWindow(Window window)
        {
            if (SelectedUnit.Value?.Count > 0)
            {
                var result = MessageBox.Show("保存せず閉じます。よろしいですか？", "確認", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    window.Close();
                }
                return;
            }
            window.Close();
        }

        private void SaveParty(Window window)
        {
            if (m_Party == null)
            {
                m_Party = new UserParty(m_PartUnits, PartyComment.Value, EstimateDamage.Value);
            }
            else
            {
                m_Party.UpdateData(m_PartUnits, PartyComment.Value, EstimateDamage.Value);
            }
            Database.I.SaveParty(m_Party);
            window.Close();
        }
    }
}