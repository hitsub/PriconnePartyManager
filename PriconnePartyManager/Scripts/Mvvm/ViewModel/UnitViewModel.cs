using System;
using System.Windows;
using System.Windows.Media.Imaging;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Enum;
using PriconnePartyManager.Scripts.Mvvm.Common;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace PriconnePartyManager.Scripts.Mvvm.ViewModel
{
    public class UnitViewModel : BindingBase
    {
        public ReactiveProperty<string> Name { get; }
        public ReactiveProperty<BitmapImage> Icon { get; }
        
        public Unit Unit { get; }
        
        public ReactiveProperty<bool> IsSelect { get; }
        
        public ReactiveProperty<Visibility> IsVisibility { get; set; }
        
        public UnitViewModel(Unit unit)
        {
            Unit = unit;
            Name = new ReactiveProperty<string>(Unit.Name);
            Icon = new ReactiveProperty<BitmapImage>(Unit.Icon);
            IsSelect = new ReactiveProperty<bool>(false);
            IsVisibility = new ReactiveProperty<Visibility>(Visibility.Visible);

            Name.AddTo(m_Disposables);
            Icon.AddTo(m_Disposables);
            IsSelect.AddTo(m_Disposables);
            IsVisibility.AddTo(m_Disposables);
        }

        public void SetSelect(bool isSelect)
        {
            IsSelect.Value = isSelect;
        }
    }
}