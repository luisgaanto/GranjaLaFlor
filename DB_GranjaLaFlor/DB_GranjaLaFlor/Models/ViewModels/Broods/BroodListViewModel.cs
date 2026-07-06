using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels.Broods
{
    /*
     * Architecture Decision | List ViewModel
     * This ViewModel represents the data displayed in Brood list views.
     * It includes fields from Brood and related display data such as
     * BroilerHouseName, avoiding direct Entity exposure in operational views.
     *
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/views/overview
     */
    public class BroodListViewModel
    {
        public int BroodId { get; set; }

        [Display(Name = "Nombre")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Fecha de Ingreso")]
        [DataType(DataType.Date)]
        public DateTime BroodDate { get; set; }

        [Display(Name = "Aves Iniciales")]
        public int BroodBirdInitialNum { get; set; }

        [Display(Name = "Descripción")]
        public string? BroodDescription { get; set; }

        [Display(Name = "Estado")]
        public bool BroodState { get; set; }

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;
    }
}