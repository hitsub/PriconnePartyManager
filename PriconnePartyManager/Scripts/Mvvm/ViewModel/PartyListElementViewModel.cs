using System.Collections.ObjectModel;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.DataModel;
using PriconnePartyManager.Scripts.Mvvm.Common;
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
        
        public ReadOnlyReactiveCollection<UserUnitViewModel> PartyUnits { get; private set; }

        public ReactiveCommand OnDelete { get; } = new ReactiveCommand();
        
        private readonly ObservableCollection<UserUnit> m_PartyUnitsCollection;

        public PartyListElementViewModel(UserParty party)
        {
            m_PartyUnitsCollection = new ObservableCollection<UserUnit>(party.UserUnits);
            PartyUnits = m_PartyUnitsCollection.ToReadOnlyReactiveCollection(x => new UserUnitViewModel(x));

            OnDelete.Subscribe(() => Database.I.RemoveParty(party));
        }
    }
}