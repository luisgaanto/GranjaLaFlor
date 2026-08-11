using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.Models.ViewModels.WeeklyCheck
{
    /*
     * Represents the complete Weekly Check information
     * required by the Details and Delete views.
     *
     * Related entity names and copied expected values are included
     * so the views do not depend directly on navigation properties.
     */
    public class WeeklyCheckGetByIdViewModel
    {
        [Display(Name = "ID")]
        public int WeeklyCheckId { get; set; }

        [Display(Name = "Estado")]
        public bool WeeklyCheckState { get; set; }

        [Display(Name = "Pollera")]
        public string BroilerHouseName { get; set; } = string.Empty;

        [Display(Name = "Camada")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Fecha Camada")]
        [DataType(DataType.Date)]
        public DateTime BroodDate { get; set; }

        [Display(Name = "Cantidad Inicial Aves")]
        public int BroodBirdInitialNum { get; set; }

        [Display(Name = "Semana")]
        public string WeeklyCheckWeek { get; set; } = string.Empty;

        [Display(Name = "Cantidad Aves Muestra")]
        public int SampleBirdQuantity { get; set; }

        [Display(Name = "Peso Total Muestra (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal TotalBirdWeight { get; set; }

        [Display(Name = "Peso Promedio Semanal (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal AverageWeeklyWeight { get; set; }

        [Display(Name = "Consumo Real (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyRealConsumption { get; set; }

        [Display(Name = "Consumo Esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedConsumption { get; set; }

        [Display(Name = "Diferencia Consumo (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyConsumptionDifference { get; set; }

        [Display(Name = "Peso Esperado (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyExpectedWeight { get; set; }

        [Display(Name = "Diferencia Peso (kg)")]
        [DisplayFormat(DataFormatString = "{0:N3}")]
        public decimal WeeklyWeightDifference { get; set; }

        [Display(Name = "Conversión Real")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealConversion { get; set; }

        [Display(Name = "Conversión Esperada")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedConversion { get; set; }

        [Display(Name = "Diferencia Conversión")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyConversionDifference { get; set; }

        [Display(Name = "Mortalidad Real (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyRealMortality { get; set; }

        [Display(Name = "Mortalidad Esperada (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyExpectedMortality { get; set; }

        [Display(Name = "Diferencia Mortalidad (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal WeeklyMortalityDifference { get; set; }

        [Display(Name = "Descripción")]
        public string? WeeklyCheckDescription { get; set; }

        /*
         * Internal relationship identifiers.
         */
        public int BroodId { get; set; }

        public int ExpectedValueId { get; set; }
    }
}