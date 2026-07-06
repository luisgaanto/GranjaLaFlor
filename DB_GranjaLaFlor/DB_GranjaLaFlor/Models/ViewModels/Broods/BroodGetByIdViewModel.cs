using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels.Broods
{
    /*
     * Architecture Decision | GetById ViewModel
     * This ViewModel represents a single Brood returned by its identifier.
     * It is shared by Details, Delete and Activate views to avoid duplicating
     * identical data models while keeping presentation concerns separated
     * from the Entity model.
     *
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/views/overview
     */
    public class BroodGetByIdViewModel
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

        public int BroilerHouseId { get; set; }

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;
    }
}