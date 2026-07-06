using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates
{
    /*
     * Architecture Decision | Form ViewModel
     * This ViewModel represents the Create/Edit form for Income Concentrates.
     * It contains user input fields and UI-specific dropdown data while
     * calculated fields are handled in the Service layer.
     *
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/models/model-binding
     */
    public class IncomeConcentrateFormViewModel
    {
        public int IncomeConcentrateId { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Date)]
        public DateTime IncomeConcentrateDate { get; set; } = DateTime.Today;

        [Display(Name = "Quintales")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public decimal IncomeQuintals { get; set; }

        [Display(Name = "Kilos Calculados")]
        public decimal IncomeKilos { get; set; }

        [Display(Name = "Acumulado Estimado")]
        public decimal IncomeAccumulated { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        public string? IncomeDescription { get; set; }

        [Display(Name = "Camada")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int BroodId { get; set; }

        public List<SelectListItem> Broods { get; set; } = new();
    }
}