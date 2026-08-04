using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.ExpectedValue
{
    /*
     * Represents the editable Expected Value information
     * used by the Edit form.
     *
     * The week is displayed as read-only information because
     * the fixed catalog records cannot be created or reassigned
     * to another week from the web application.
     */
    public class ExpectedValueFormViewModel
    {
        /*
         * Internal identifier used to locate the record
         * that will be updated.
         */
        [Display(Name = "ID")]
        public int ExpectedValueId { get; set; }

        /*
         * Fixed week associated with the Expected Value record.
         *
         * This value is displayed in the form but is not updated
         * by ExpectedValueService.
         */
        [Display(Name = "Semana")]
        public string ExpectedValueWeek { get; set; } = string.Empty;

        [Required(
            ErrorMessage =
                "El consumo esperado es obligatorio.")]
        [Range(
            typeof(decimal),
            "0.001",
            "9999999.999",
            ErrorMessage =
                "El consumo esperado debe ser mayor que cero.")]
        [Display(Name = "Consumo esperado (kg)")]
        [DisplayFormat(
            DataFormatString = "{0:N3}",
            ApplyFormatInEditMode = true)]
        public decimal ExpectedConsumption { get; set; }

        [Required(
            ErrorMessage =
                "El peso esperado es obligatorio.")]
        [Range(
            typeof(decimal),
            "0.001",
            "9999999.999",
            ErrorMessage =
                "El peso esperado debe ser mayor que cero.")]
        [Display(Name = "Peso esperado (kg)")]
        [DisplayFormat(
            DataFormatString = "{0:N3}",
            ApplyFormatInEditMode = true)]
        public decimal ExpectedWeight { get; set; }

        [Required(
            ErrorMessage =
                "La conversión esperada es obligatoria.")]
        [Range(
            typeof(decimal),
            "0.01",
            "99999999.99",
            ErrorMessage =
                "La conversión esperada debe ser mayor que cero.")]
        [Display(Name = "Conversión esperada")]
        [DisplayFormat(
            DataFormatString = "{0:N2}",
            ApplyFormatInEditMode = true)]
        public decimal ExpectedConversion { get; set; }

        [Required(
            ErrorMessage =
                "La mortalidad esperada es obligatoria.")]
        [Range(
            typeof(decimal),
            "0",
            "100",
            ErrorMessage =
                "La mortalidad esperada debe encontrarse entre 0 y 100.")]
        [Display(Name = "Mortalidad esperada (%)")]
        [DisplayFormat(
            DataFormatString = "{0:N2}",
            ApplyFormatInEditMode = true)]
        public decimal ExpectedMortality { get; set; }
    }
}