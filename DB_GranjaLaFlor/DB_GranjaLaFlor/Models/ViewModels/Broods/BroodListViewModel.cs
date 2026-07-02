namespace DB_GranjaLaFlor.Models.ViewModels.Broods
{
    /*
     * Architecture Decision | List ViewModel
     * This ViewModel represents the data displayed in Brood list views.
     * It includes fields from Brood and related display data such as
     * BroilerHouseName, avoiding direct Entity exposure in operational views.
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/views/overview
     */
    public class BroodListViewModel
    {
        public int BroodId { get; set; }

        public string BroodName { get; set; } = string.Empty;

        public DateTime BroodDate { get; set; }

        public int BroodBirdInitialNum { get; set; }

        public string? BroodDescription { get; set; }

        public bool BroodState { get; set; }

        public string BroilerHouseName { get; set; } = string.Empty;
    }
}