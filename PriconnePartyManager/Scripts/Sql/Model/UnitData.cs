using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriconnePartyManager.Scripts.Sql.Model
{
	[Table("unit_data")]
	public class UnitData
	{
		[Column("unit_id"), Required, Key]
		public int UnitId { get; set; }

		[Column("search_area_width"), Required]
		public int Order { get; set; }
	}
}
