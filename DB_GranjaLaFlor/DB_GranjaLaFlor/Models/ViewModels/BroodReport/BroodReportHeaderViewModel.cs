using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.BroodReport
{
    /*
     * Represents the variable information displayed
     * in the Brood Report header.
     *
     * Fixed information belonging to the original report
     * template is not stored in the snapshot.
     */
    public class BroodReportHeaderViewModel
    {
        /*
         * Report number entered by the user.
         */
        [Display(Name = "Nº")]
        public int ReportNumber { get; set; }

        /*
         * Date of the first Daily Check included
         * in the Brood Report.
         */
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        /*
         * Farm name displayed in the original report.
         */
        [Display(Name = "Granja")]
        public string FarmName { get; set; }
            = string.Empty;

        /*
         * Initial bird quantity of the Brood.
         */
        [Display(Name = "Nº Aves")]
        public int BirdQuantity { get; set; }

        /*
         * Broiler House name displayed using the
         * original report label "Galera".
         */
        [Display(Name = "Galera")]
        public string BroilerHouseName { get; set; }
            = string.Empty;
    }
}