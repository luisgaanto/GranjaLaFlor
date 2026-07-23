using DB_GranjaLaFlor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectGranjaLaFlor.Models
{
    /*
     * Represents the daily operational control associated
     * with a Brood and an Income Concentrate record.
     *
     * The entity stores mortality, feed consumption,
     * accumulated values and the resulting balances.
     */
    [Table("daily_checks")]
    public class DailyCheck
    {
        [Key]
        [Column("daily_check_id")]
        public int DailyCheckId { get; set; }
       
        [Required]
        [Column("daily_check_date", TypeName = "date")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime DailyCheckDate { get; set; }
        
        [Required]
        [Column("natural_mortality")]
        [Display(Name = "Mortalidad natural")]
        public int NaturalMortality { get; set; }

        [Required]
        [Column("select_quantity")]
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
        [Precision(10, 2)]
        [Column("consumption_quintals")]
        [Display(Name = "Consumo en quintales")]
        public decimal ConsumptionQuintals { get; set; }
       
        [Required]
        [Precision(10, 2)]
        [Column("consumption_kilos")]
        [Display(Name = "Consumo en kilos")]
        public decimal ConsumptionKilos { get; set; }

        [Required]
        [Precision(10, 2)]
        [Column("accumulated_consumption")]
        [Display(Name = "Consumo acumulado")]
        public decimal AccumulatedConsumption { get; set; }

        [Required]
        [Precision(10, 2)]
        [Column("concentrate_balance")]
        [Display(Name = "Saldo de concentrado")]
        public decimal ConcentrateBalance { get; set; }

        [Column("daily_check_description")]
        [StringLength(
            200,
            ErrorMessage =
                "La descripción no puede superar los 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string? DailyCheckDescription { get; set; }

        [Required]
        [Column("daily_check_state")]
        [Display(Name = "Estado")]
        public bool DailyCheckState { get; set; }

        /*
         * Foreign key that identifies the Brood associated
         * with the Daily Check.
         */
        [Required]
        [Column("brood_id")]
        [Display(Name = "Camada")]
        public int BroodId { get; set; }

        /*
         * Control day represented by one of the supported
         * values from Día 1 through Día 7.
         */
        [Required]
        [Column("daily_check_day")]
        [StringLength(10)]
        [Display(Name = "Día de control")]
        public string DailyCheckDay { get; set; } = string.Empty;

        /*
         * Foreign key that identifies the Income Concentrate
         * record associated with the Daily Check.
         */
        [Required]
        [Column("income_concentrate_id")]
        [Display(Name = "Ingreso de concentrado")]
        public int IncomeConcentrateId { get; set; }

        /*
         * Navigation property to the Brood associated
         * with this Daily Check.
         */
        [ForeignKey(nameof(BroodId))] public Brood Brood { get; set; } = null!;

        /*
         * Navigation property to the Income Concentrate
         * record associated with this Daily Check.
         */
        [ForeignKey(nameof(IncomeConcentrateId))] public IncomeConcentrate IncomeConcentrate { get; set; } = null!;
    }
}