using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriconnePartyManager.Scripts.Sql.Model
{
    [Table("unit_rarity")]
    public class UnitPlayable
    {
        [Column("unit_id"), Required, Key]
        public int UnitId { get; set; }
    }
}