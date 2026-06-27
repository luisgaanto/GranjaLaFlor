using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_GranjaLaFlor.Models.Entities
{
    [Table("broiler_houses")]
    public class BroilerHouse
    {
        [Key]
        [Column("broiler_house_id")]
        public int BroilerHouseId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre")]
        [Column("broiler_house_name")]
        public string BroilerHouseName { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Descripción")]
        [Column("broiler_house_description")]
        public string? BroilerHouseDescription { get; set; }

        [Display(Name = "Estado")]
        [Column("broiler_house_state")]
        public bool BroilerHouseState { get; set; }
    }
}