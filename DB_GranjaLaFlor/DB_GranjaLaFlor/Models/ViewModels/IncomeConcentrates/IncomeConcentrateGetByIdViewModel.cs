using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates
{
    /*
     * Architecture Decision | GetById ViewModel
     * This ViewModel represents a single Income Concentrate record returned
     * by its identifier. It is shared by Details, Delete and Activate views,
     * keeping presentation concerns separated from the Entity model.
     *
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/views/overview
     */
    public class IncomeConcentrateGetByIdViewModel
    {
        public int IncomeConcentrateId { get; set; }

        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime IncomeConcentrateDate { get; set; }

        [Display(Name = "Quintales")]
        public decimal IncomeQuintals { get; set; }

        [Display(Name = "Kilos")]
        public decimal IncomeKilos { get; set; }

        [Display(Name = "Acumulado")]
        public decimal IncomeAccumulated { get; set; }

        [Display(Name = "Descripción")]
        public string? IncomeDescription { get; set; }

        [Display(Name = "Estado")]
        public bool IncomeState { get; set; }

        public int BroodId { get; set; }

        [Display(Name = "Camada")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Año")]
        public int BroodYear { get; set; }
    }
}