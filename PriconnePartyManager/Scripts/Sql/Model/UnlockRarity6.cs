using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriconnePartyManager.Scripts.Sql.Model
{
	[Table("unlock_rarity_6")]
	public class UnlockRarity6
	{
		[Column("unit_id"), Required, Key]
		public int UnitId { get; set; }
		
		[Column("unlock_level"), Required]
		public int UnlockLevel { get; set; }
	}
}
