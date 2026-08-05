using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_GranjaLaFlor.Models.Entities
{
    /*
     * Represents the weekly production control associated
     * with a Brood and an Expected Value record.
     *
     * The entity stores the sample information, real production
     * results, expected values, calculated differences and state.
     */
    [Table("weekly_checks")]
    public class WeeklyCheck
    {
        [Key]
        [Column("weekly_check_id")]
        public int WeeklyCheckId { get; set; }

        /*
         * Quantity of birds included in the weekly weight sample.
         *
         * The value is calculated automatically using two percent
         * of the final active bird balance for the selected week.
         */
        [Required]
        [Column("sample_bird_quantity")]
        [Display(Name = "Aves de muestra")]
        public int SampleBirdQuantity { get; set; }

        /*
         * Total weight obtained from all birds included
         * in the weekly sample.
         *
         * The value is entered by the user in kilograms
         * with precision to represent grams.
         */
        [Required]
        [Precision(10, 3)]
        [Column("total_bird_weight")]
        [Display(Name = "Peso total muestra (kg)")]
        public decimal TotalBirdWeight { get; set; }

        /*
         * Average bird weight calculated by dividing the
         * total sample weight by the sample bird quantity.
         */
        [Required]
        [Precision(10, 3)]
        [Column("average_weekly_weight")]
        [Display(Name = "Peso promedio semanal (kg)")]
        public decimal AverageWeeklyWeight { get; set; }

        /*
         * Real accumulated feed consumption per active bird
         * at the end of the selected week.
         */
        [Required]
        [Precision(10, 3)]
        [Column("weekly_real_consumption")]
        [Display(Name = "Consumo real (kg)")]
        public decimal WeeklyRealConsumption { get; set; }

        /*
         * Copy of the expected consumption used when the
         * Weekly Check was generated.
         *
         * The value is preserved so historical reports do not
         * change if the Expected Value catalog is updated later.
         */
        [Required]
        [Precision(10, 3)]
        [Column("weekly_expected_consumption")]
        [Display(Name = "Consumo esperado (kg)")]
        public decimal WeeklyExpectedConsumption { get; set; }

        /*
         * Difference between real and expected consumption.
         */
        [Required]
        [Precision(10, 3)]
        [Column("weekly_consumption_difference")]
        [Display(Name = "Diferencia de consumo (kg)")]
        public decimal WeeklyConsumptionDifference { get; set; }

        /*
         * Copy of the expected bird weight associated
         * with the selected week.
         */
        [Required]
        [Precision(10, 3)]
        [Column("weekly_expected_weight")]
        [Display(Name = "Peso esperado (kg)")]
        public decimal WeeklyExpectedWeight { get; set; }

        /*
         * Difference between the real average weight
         * and the expected weight.
         */
        [Required]
        [Precision(10, 3)]
        [Column("weekly_weight_difference")]
        [Display(Name = "Diferencia de peso (kg)")]
        public decimal WeeklyWeightDifference { get; set; }

        /*
         * Real feed conversion calculated using the real
         * consumption and average weekly weight.
         */
        [Required]
        [Precision(10, 2)]
        [Column("weekly_real_conversion")]
        [Display(Name = "Conversión real")]
        public decimal WeeklyRealConversion { get; set; }

        /*
         * Copy of the expected conversion used when
         * the Weekly Check was generated.
         */
        [Required]
        [Precision(10, 2)]
        [Column("weekly_expected_conversion")]
        [Display(Name = "Conversión esperada")]
        public decimal WeeklyExpectedConversion { get; set; }

        /*
         * Difference between real and expected conversion.
         */
        [Required]
        [Precision(10, 2)]
        [Column("weekly_conversion_difference")]
        [Display(Name = "Diferencia de conversión")]
        public decimal WeeklyConversionDifference { get; set; }

        /*
         * Real accumulated mortality percentage calculated
         * at the end of the selected week.
         */
        [Required]
        [Precision(10, 2)]
        [Column("weekly_real_mortality")]
        [Display(Name = "Mortalidad real (%)")]
        public decimal WeeklyRealMortality { get; set; }

        /*
         * Copy of the expected mortality percentage used
         * when the Weekly Check was generated.
         */
        [Required]
        [Precision(10, 2)]
        [Column("weekly_expected_mortality")]
        [Display(Name = "Mortalidad esperada (%)")]
        public decimal WeeklyExpectedMortality { get; set; }

        /*
         * Difference between real and expected mortality.
         */
        [Required]
        [Precision(10, 2)]
        [Column("weekly_mortality_difference")]
        [Display(Name = "Diferencia de mortalidad (%)")]
        public decimal WeeklyMortalityDifference { get; set; }

        [Column("weekly_check_description")]
        [StringLength(
            200,
            ErrorMessage =
                "La descripción no puede superar los 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string? WeeklyCheckDescription { get; set; }

        [Required]
        [Column("weekly_check_state")]
        [Display(Name = "Estado")]
        public bool WeeklyCheckState { get; set; }

        /*
         * Fixed week represented by one of the supported
         * values from Semana 1 through Semana 6.
         */
        [Required]
        [StringLength(20)]
        [Column("weekly_check_week")]
        [Display(Name = "Semana")]
        public string WeeklyCheckWeek { get; set; } = string.Empty;

        /*
         * Foreign key that identifies the Brood associated
         * with the Weekly Check.
         */
        [Required]
        [Column("brood_id")]
        [Display(Name = "Camada")]
        public int BroodId { get; set; }

        /*
         * Foreign key that identifies the Expected Value record
         * used to calculate the weekly differences.
         *
         * The value is assigned automatically by the Service
         * according to the selected week.
         */
        [Required]
        [Column("expected_value_id")]
        [Display(Name = "Valores esperados")]
        public int ExpectedValueId { get; set; }

        /*
         * Navigation property to the Brood associated
         * with this Weekly Check.
         */
        [ForeignKey(nameof(BroodId))]
        public Brood Brood { get; set; } = null!;

        /*
         * Navigation property to the Expected Value record
         * used by this Weekly Check.
         */
        [ForeignKey(nameof(ExpectedValueId))]
        public ExpectedValue ExpectedValue { get; set; } = null!;
    }
}