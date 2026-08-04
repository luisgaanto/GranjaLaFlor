using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.ExpectedValue
{
    /*
     * Represents the Expected Value information displayed
     * in the Index view.
     *
     * The ViewModel contains only the properties required
     * to display and identify each fixed weekly record.
     */
    public class ExpectedValueListViewModel
    {
        /*
         * Internal identifier used by the Edit action.
         * It is not required as a visible Index column.
         */
        public int ExpectedValueId { get; set; }

        [Display(Name = "Semana")]
        public string ExpectedValueWeek { get; set; } = string.Empty;

        [Display(Name = "Consumo esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal ExpectedConsumption { get; set; }

        [Display(Name = "Peso esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal ExpectedWeight { get; set; }

        [Display(Name = "Conversión esperada")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ExpectedConversion { get; set; }

        [Display(Name = "Mortalidad esperada (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ExpectedMortality { get; set; }
    }
}