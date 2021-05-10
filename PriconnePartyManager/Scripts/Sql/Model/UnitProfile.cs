using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PriconnePartyManager.Scripts.Sql.Model
{
    [Table("unit_profile")]
    public class UnitProfile
    {
        [Column("unit_id"), Required, Key]
        public int UnitId { get; set; }

        [Column("unit_name"), Required]
        public string UnitName { get; set; }
    }
}