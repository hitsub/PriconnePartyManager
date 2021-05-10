using System.Windows.Media.Imaging;
using PriconnePartyManager;
using PriconnePartyManager.Scripts.Enum;
using PriconnePartyManager.Scripts.Sql.Model;

namespace PriconnePartyManager.Scripts.DataModel
{
	/// <summary>
	/// ユニットクラス
	/// </summary>
	public class Unit
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public bool IsUnlockRarity6 { get; private set; }

		/// <summary> 並び順(小さいほど前) </summary>
		public int Order { get; private set; }
		
		public BitmapImage Icon { get; private set; }
		
		public UnitType Type { get; }

		public Unit(UnitProfile profile, UnitData data, BitmapImage icon, bool isUnlockRarity6 = false)
		{
			Id = profile.UnitId;
			Name = profile.UnitName;
			Order = data.Order;
			Icon = icon;
			IsUnlockRarity6 = isUnlockRarity6;
			Type = GetUnitType(Order);
		}

		private static UnitType GetUnitType(int order)
		{
			if (order < 300)
			{
				return UnitType.Front;
			}
			if (order < 600)
			{
				return UnitType.Middle;
			}
			return UnitType.Back;
		}
	}
}
