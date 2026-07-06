using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_GranjaLaFlor.Models.Entities
{
    /*
     * Architecture Decision | Entity Model
     * This class represents only the persistent data stored in the
     * "income_concentrates" database table. UI-specific data such as
     * dropdown lists will be implemented through ViewModels.
     *
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/overview
     */
    [Table("income_concentrates")]
    public class IncomeConcentrate
    {
        [Key]
        [Column("income_concentrate_id")]
        public int IncomeConcentrateId { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Date)]
        [Column("income_concentrate_date")]
        public DateTime IncomeConcentrateDate { get; set; }

        [Display(Name = "Quintales")]
        [Column("income_quintals")]
        public decimal IncomeQuintals { get; set; }

        [Display(Name = "Kilos")]
        [Column("income_kilos")]
        public decimal IncomeKilos { get; set; }

        [Display(Name = "Acumulado")]
        [Column("income_accumulated")]
        public decimal IncomeAccumulated { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200)]
        [Column("income_description")]
        public string? IncomeDescription { get; set; }

        [Display(Name = "Estado")]
        [Column("income_state")]
        public bool IncomeState { get; set; } = true;

        [Display(Name = "Camada")]
        [Column("brood_id")]
        public int BroodId { get; set; }

        public Brood? Brood { get; set; }
    }
}