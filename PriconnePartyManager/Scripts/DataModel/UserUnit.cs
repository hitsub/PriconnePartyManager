using System.Linq;
using System.Text.Json.Serialization;
using PriconnePartyManager;
using PriconnePartyManager.Scripts.Common;
using PriconnePartyManager.Scripts.Enum;

namespace PriconnePartyManager.Scripts.DataModel
{
    public class UserUnit
    {
        [JsonIgnore]
        public Unit Unit
        {
            get
            {
                if (m_Unit == null)
                {
                    m_Unit = Database.I.Units.SingleOrDefault(x => x.Id == UnitId);
                }

                return m_Unit;
            }
        }

        public int UnitId { get; set; }

        public UnitRarity Rarity { get; set; } = UnitRarity.None;

        public UnitRank Rank { get; set; } = UnitRank.None;

        public bool IsSupport { get; set; }

        [JsonIgnore]
        private Unit m_Unit;

        /// <summary>
        /// JSONでのデシリアライズの時に呼ばれるので残しておく
        /// </summary>
        public UserUnit()
        {
        }

        public UserUnit(Unit unit)
        {
            m_Unit = unit;
            UnitId = m_Unit.Id;
        }
    }
}