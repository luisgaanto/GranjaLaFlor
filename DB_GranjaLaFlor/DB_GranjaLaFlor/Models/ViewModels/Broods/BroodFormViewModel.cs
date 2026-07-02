using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels.Broods
{
    /*
     * Architecture Decision | ViewModel
     * This ViewModel represents the Brood Create/Edit form.
     * It includes persistent form data and UI-specific data such as
     * dropdown lists. This keeps Entity classes focused only on database mapping.
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/models/model-binding
     */
    public class BroodFormViewModel
    {
        public int BroodId { get; set; }

        [Display(Name = "Nombre de la Camada")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Fecha de Ingreso")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Date)]
        public DateTime BroodDate { get; set; } = DateTime.Today;

        [Display(Name = "Cantidad Inicial de Aves")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int BroodBirdInitialNum { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(150, ErrorMessage = "La descripción no puede superar los 150 caracteres.")]
        public string? BroodDescription { get; set; }

        [Display(Name = "Pollera")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int BroilerHouseId { get; set; }

        /*
         * UI Data | Used to populate the Brood Name dropdown list.
         * Business rule allows only 7 broods per year.
        */
        public List<SelectListItem> BroodNames { get; set; } = new();

        /*
         * UI Data | Used to populate the BroilerHouse dropdown list.
         * This property does not belong to the database entity.
         */
        public List<SelectListItem> BroilerHouses { get; set; } = new();
    }
}