using System.ComponentModel;
using PriconnePartyManager.Scripts.Utils;

namespace PriconnePartyManager.Scripts.Enum
{
	public enum UnitRarity
	{
		[Description("指定なし")]
		None = 0,
		[Description("★3")]
		Rarity3 = 3,
		[Description("★4")]
		Rarity4 = 4,
		[Description("★5")]
		Rarity5 = 5,
		[Description("★6")]
		Rarity6 = 6,
	}
}
