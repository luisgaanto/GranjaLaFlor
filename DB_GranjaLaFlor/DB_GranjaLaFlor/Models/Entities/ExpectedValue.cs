using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_GranjaLaFlor.Models.Entities
{
    /*
     * Represents the expected production values associated
     * with each week of the broiler growth cycle.
     *
     * This entity is a fixed editable catalog. The six records
     * are created directly in the database and the web application
     * only allows their values to be viewed and updated.
     */
    [Table("expected_values")]
    public class ExpectedValue
    {
        [Key]
        [Column("expected_value_id")]
        public int ExpectedValueId { get; set; }

        /*
         * Fixed week represented by one of the supported values
         * from Semana 1 through Semana 6.
         *
         * This value is created directly in the database and
         * cannot be modified from the web application.
         */
        [Required]
        [StringLength(20)]
        [Column("expected_value_week")]
        [Display(Name = "Semana")]
        public string ExpectedValueWeek { get; set; } = string.Empty;

        /*
         * Expected feed consumption expressed in kilograms
         * with precision to represent grams.
         *
         * Examples:
         * 0.170 represents 170 grams.
         * 1.200 represents 1 kilogram and 200 grams.
         */
        [Required]
        [Precision(10, 3)]
        [Column("expected_consumption")]
        [Display(Name = "Consumo esperado (kg)")]
        public decimal ExpectedConsumption { get; set; }

        /*
         * Expected bird weight expressed in kilograms
         * with precision to represent grams.
         *
         * Examples:
         * 0.200 represents 200 grams.
         * 1.100 represents 1 kilogram and 100 grams.
         */
        [Required]
        [Precision(10, 3)]
        [Column("expected_weight")]
        [Display(Name = "Peso esperado (kg)")]
        public decimal ExpectedWeight { get; set; }

        /*
         * Expected feed conversion value for the corresponding week.
         */
        [Required]
        [Precision(10, 2)]
        [Column("expected_conversion")]
        [Display(Name = "Conversión esperada")]
        public decimal ExpectedConversion { get; set; }

        /*
         * Expected accumulated mortality percentage
         * for the corresponding week.
         */
        [Required]
        [Precision(10, 2)]
        [Column("expected_mortality")]
        [Display(Name = "Mortalidad esperada (%)")]
        public decimal ExpectedMortality { get; set; }
    }
}