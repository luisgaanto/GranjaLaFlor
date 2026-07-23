using DB_GranjaLaFlor.Models.ViewModels.IncomeConcentrates;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ProjectGranjaLaFlor.ViewModels.IncomeConcentrates
{
    public class IncomeConcentrateFilterViewModel
    {
        [Display(Name = "Camada")]
        public string? BroodName { get; set; }

        [Display(Name = "Año")]
        public int? Year { get; set; }

        [Display(Name = "Pollera")]
        public int? BroilerHouseId { get; set; }

        public List<SelectListItem> BroodOptions { get; set; } = [];

        public List<SelectListItem> YearOptions { get; set; } = [];

        public List<SelectListItem> BroilerHouseOptions { get; set; } = [];

        public IEnumerable<IncomeConcentrateListViewModel> IncomeConcentrates
        {
            get;
            set;
        } = [];
    }
}