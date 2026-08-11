using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck
{
    /*
     * Represents the editable Weekly Check information
     * used by the Create and Edit forms.
     *
     * User-entered values and selections are received through
     * this ViewModel. Obtained and calculated values are always
     * generated again by WeeklyCheckService before saving.
     */
    public class WeeklyCheckFormViewModel
    {
        /*
         * Used during Edit to identify the record.
         * During Create, the value remains zero.
         */
        [Display(Name = "ID")]
        public int WeeklyCheckId { get; set; }

        /*
         * User-selected and entered values.
         */
        [Required(ErrorMessage = "Debe seleccionar una pollera.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Debe seleccionar una pollera válida.")]
        [Display(Name = "Pollera")]
        public int BroilerHouseId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una camada.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage =
                "Debe seleccionar una camada válida.")]
        [Display(Name = "Camada")]
        public int BroodId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una semana.")]
        [Display(Name = "Semana")]
        public string WeeklyCheckWeek { get; set; } = string.Empty;

        [Required(
            ErrorMessage =
                "El peso total de la muestra es obligatorio.")]
        [Range(
            typeof(decimal),
            "0.001",
            "9999999.999",
            ErrorMessage =
                "El peso total de la muestra debe ser mayor que cero.")]
        [Display(Name = "Peso total de la muestra (kg)")]
        [DisplayFormat(
            DataFormatString = "{0:N3}",
            ApplyFormatInEditMode = true)]
        public decimal TotalBirdWeight { get; set; }

        [StringLength(
            200,
            ErrorMessage =
                "La descripción no puede superar los 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string? WeeklyCheckDescription { get; set; }

        /*
         * Internal identifier of the Expected Value record
         * associated with the selected week.
         *
         * The user does not select this identifier manually.
         * WeeklyCheckService obtains it using WeeklyCheckWeek.
         */
        public int ExpectedValueId { get; set; }

        /*
         * Information obtained automatically from the selected
         * Brood and its seven active Daily Checks.
         */
        [Display(Name = "Cantidad inicial aves")]
        public int BroodBirdInitialNum { get; set; }

        [Display(Name = "Saldo actual aves")]
        public int FinalDailyBirdBalance { get; set; }

        [Display(Name = "Consumo acumulado final (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal FinalAccumulatedConsumption { get; set; }

        [Display(Name = "Mortalidad acumulada final")]
        public int FinalAccumulatedMortality { get; set; }

        /*
         * Expected values copied from the Expected Value catalog
         * corresponding to the selected week.
         */
        [Display(Name = "Consumo esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedConsumption { get; set; }

        [Display(Name = "Peso esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedWeight { get; set; }

        [Display(Name = "Conversión esperada")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedConversion { get; set; }

        [Display(Name = "Mortalidad esperada (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedMortality { get; set; }

        /*
         * Calculated values displayed as read-only information.
         *
         * These values are never trusted when submitted by the form
         * and are recalculated by WeeklyCheckService before saving.
         */
        [Display(Name = "Cantidad Aves Muestra")]
        public int SampleBirdQuantity { get; set; }

        [Display(Name = "Peso promedio semanal (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal AverageWeeklyWeight { get; set; }

        [Display(Name = "Consumo Real (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyRealConsumption { get; set; }

        [Display(Name = "Diferencia Consumo (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyConsumptionDifference { get; set; }

        [Display(Name = "Diferencia de peso (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyWeightDifference { get; set; }

        [Display(Name = "Conversión Real")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealConversion { get; set; }

        [Display(Name = "Diferencia Conversión")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyConversionDifference { get; set; }

        [Display(Name = "Mortalidad Real (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealMortality { get; set; }

        [Display(Name = "Diferencia Mortalidad (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyMortalityDifference { get; set; }

        /*
         * Daily Check records obtained for the selected
         * Brood and week.
         */
        public IEnumerable<WeeklyCheckDailyCheckViewModel> DailyChecks
        {
            get;
            set;
        } = new List<WeeklyCheckDailyCheckViewModel>();

        /*
         * Dropdown options loaded by the Service layer.
         */
        public IEnumerable<SelectListItem> BroilerHouseOptions { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> BroodOptions { get; set; }
            = new List<SelectListItem>();

        public IEnumerable<SelectListItem> WeeklyCheckWeekOptions { get; set; }
            = new List<SelectListItem>();
    }
}