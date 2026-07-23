using DB_GranjaLaFlor.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGranjaLaFlor.Models
{
    [Table("daily_checks")]
    public class DailyCheck
    {
        [Key]
        [Column("daily_check_id")]
        public int DailyCheckId { get; set; }

        [Required]
        [Column("daily_check_date")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime DailyCheckDate { get; set; }

        [Required]
        [Column("natural_mortality")]
        [Range(0, int.MaxValue,
            ErrorMessage = "La mortalidad natural no puede ser negativa.")]
        [Display(Name = "Mortalidad natural")]
        public int NaturalMortality { get; set; }

        [Required]
        [Column("select_quantity")]
        [Range(0, int.MaxValue,
            ErrorMessage = "La cantidad seleccionada no puede ser negativa.")]
        [Display(Name = "Selección")]
        public int SelectQuantity { get; set; }

        [Required]
        [Column("total_daily_mortality")]
        [Display(Name = "Mortalidad diaria total")]
        public int TotalDailyMortality { get; set; }

        [Required]
        [Column("accumulated_mortality")]
        [Display(Name = "Mortalidad acumulada")]
        public int AccumulatedMortality { get; set; }

        [Required]
        [Column("daily_bird_balance")]
        [Display(Name = "Saldo de aves")]
        public int DailyBirdBalance { get; set; }

        [Required]
        [Column("consumption_quintals", TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0", "99999999.99",
            ErrorMessage = "El consumo en quintales no puede ser negativo.")]
        [Display(Name = "Consumo en quintales")]
        public decimal ConsumptionQuintals { get; set; }

        [Required]
        [Column("consumption_kilos", TypeName = "decimal(10,2)")]
        [Display(Name = "Consumo en kilos")]
        public decimal ConsumptionKilos { get; set; }

        [Required]
        [Column("accumulated_consumption", TypeName = "decimal(10,2)")]
        [Display(Name = "Consumo acumulado")]
        public decimal AccumulatedConsumption { get; set; }

        [Required]
        [Column("concentrate_balance", TypeName = "decimal(10,2)")]
        [Display(Name = "Saldo de concentrado")]
        public decimal ConcentrateBalance { get; set; }

        [Column("daily_check_description")]
        [StringLength(200,
            ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string? DailyCheckDescription { get; set; }

        [Required]
        [Column("daily_check_state")]
        [Display(Name = "Estado")]
        public bool DailyCheckState { get; set; }

        [Required]
        [Column("brood_id")]
        [Display(Name = "Camada")]
        public int BroodId { get; set; }

        [Required]
        [Column("daily_check_day")]
        [StringLength(10)]
        [Display(Name = "Día de control")]
        public string DailyCheckDay { get; set; } = null!;

        [Required]
        [Column("income_concentrate_id")]
        [Display(Name = "Ingreso de concentrado")]
        public int IncomeConcentrateId { get; set; }

        [ForeignKey(nameof(BroodId))]
        public Brood Brood { get; set; } = null!;

        [ForeignKey(nameof(IncomeConcentrateId))]
        public IncomeConcentrate IncomeConcentrate { get; set; } = null!;
    }
}