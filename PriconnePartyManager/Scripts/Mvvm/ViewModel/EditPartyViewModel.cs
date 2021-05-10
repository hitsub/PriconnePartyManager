using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Enum;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class EditPartyViewModel : BindingBase
    {
        public ReadOnlyReactiveCollection<UnitViewModel> UnitList { get; private set; }

        public ReactiveProperty<IList> SelectedUnit { get; set; } = new ReactiveProperty<IList>();
        public ReactiveCommand OnChangeSelected { get; } = new ReactiveCommand();

        public ReactiveProperty<ListUnitType> ShowUnitType { get; } = new ReactiveProperty<ListUnitType>(ListUnitType.All);
        
        public ReadOnlyReactiveCollection<UserUnitViewModel> PartyUnits { get; private set; }

        public ReactiveProperty<bool> IsFullParty { get; } = new ReactiveProperty<bool>();
        
        public ReactiveCommand OnSubmit { get; } = new ReactiveCommand();
        public ReactiveCommand OnCancel { get; } = new ReactiveCommand();

        private readonly Unit[] m_Units;
        private readonly ObservableCollection<Unit> m_UnitsCollection;
        private readonly ObservableCollection<UserUnit> m_PartyUnitsCollection;
        private List<UserUnit> m_PartUnits = new List<UserUnit>();

        public EditPartyViewModel(Unit[] units)
        {
            m_Units = units.OrderBy(x => x.Order).ToArray();
            m_UnitsCollection = new ObservableCollection<Unit>(units);
            UnitList = m_UnitsCollection.ToReadOnlyReactiveCollection(x => new UnitViewModel(x));
            m_PartyUnitsCollection = new ObservableCollection<UserUnit>();
            PartyUnits = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new UserUnitViewModel(x));

            OnCancel.Subscribe(x => CloseWindow((Window)x));
            OnSubmit.Subscribe(SaveParty);
            
            ShowUnitType.Subscribe(OnChangeShowUnitType);
            OnChangeSelected.Subscribe(OnChangeSelectedUnit);
            OnChangeShowUnitType(ShowUnitType.Value);
        }

        /// <summary>
        /// 指定ユニットの一覧表示
        /// </summary>
        private void SetUnitsList(IEnumerable<Unit> units)
        {
            m_UnitsCollection.Clear();
            foreach (var unit in units)
            {
                m_UnitsCollection.Add(unit);
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
        }

        /// <summary>
        /// 表示用ユニットタイプの変更
        /// </summary>
        private void OnChangeShowUnitType(ListUnitType type)
        {
            switch (type)
            {
                case ListUnitType.Front:
                case ListUnitType.Middle:
                case ListUnitType.Back:
                    foreach (var unit in UnitList)
                    {
                        unit.IsVisibility.Value = (int) unit.Unit.Type == (int) type;
                    }
                    break;
                default:
                    SetUnitsList(m_Units);
                    break;
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

        private void SaveParty()
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            };
            var json = JsonSerializer.Serialize(m_PartUnits, options);
            Console.WriteLine(json);

            var instance = JsonSerializer.Deserialize<UserUnit[]>(json, options);
            Console.WriteLine(instance);
        }
    }
}