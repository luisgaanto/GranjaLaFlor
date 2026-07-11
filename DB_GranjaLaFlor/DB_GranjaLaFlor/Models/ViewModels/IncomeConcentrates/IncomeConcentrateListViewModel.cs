using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates
{
    /*
     * Architecture Decision | List ViewModel
     * This ViewModel represents the data displayed in Income Concentrate list views.
     * It includes related display data such as BroodName, avoiding direct
     * Entity exposure in operational views.
     *
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/views/overview
     */
    public class IncomeConcentrateListViewModel
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

        [Display(Name = "Camada")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Año")]
        public int BroodYear { get; set; }

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;

        public int BroodId { get; set; }
    }
}