using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Represents the information required to generate
     * a new Brood Report.
     *
     * The user selects the Broiler House and Brood and
     * enters the report number displayed as "Nº" in the
     * original report.
     *
     * All operational, daily and weekly information is
     * obtained automatically by BroodReportService.
     */
    public class BroodReportFormViewModel
    {
        /*
         * User-entered report number.
         *
         * The value may be repeated between different
         * generated reports.
         */
        [Required(ErrorMessage = "El número del reporte es obligatorio.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "El número del reporte debe ser mayor que cero.")]
        [Display(Name = "Nº")]
        public int ReportNumber { get; set; }

        /*
         * Broiler House selected by the user.
         *
         * The Broiler House is used to filter the available
         * Broods but is not stored directly in brood_reports.
         */
        [Required(
            ErrorMessage =
                "Debe seleccionar una pollera.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Debe seleccionar una pollera válida.")]
        [Display(Name = "Pollera")]
        public int BroilerHouseId { get; set; }

        /*
         * Brood associated with the generated report.
         *
         * BroodId becomes the foreign key stored
         * in the BroodReport entity.
         */
        [Required(
            ErrorMessage =
                "Debe seleccionar una camada.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Debe seleccionar una camada válida.")]
        [Display(Name = "Camada")]
        public int BroodId { get; set; }

        /*
         * Dropdown options loaded by BroodReportService.
         */
        public IEnumerable<SelectListItem> BroilerHouseOptions { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> BroodOptions { get; set; }
            = new List<SelectListItem>();
    }
}