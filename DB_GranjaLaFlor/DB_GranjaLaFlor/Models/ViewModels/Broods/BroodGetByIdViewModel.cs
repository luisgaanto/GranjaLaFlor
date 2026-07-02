namespace DB_GranjaLaFlor.Models.ViewModels.Broods
{
    /*
     * Architecture Decision | GetById ViewModel
     * This ViewModel represents a single Brood returned by its identifier.
     * It is shared by Details, Delete and Activate views to avoid duplicating
     * identical data models while keeping presentation concerns separated
     * from the Entity model.
     * Reference:
     * https://learn.microsoft.com/aspnet/core/mvc/views/overview
     */
    public class BroodGetByIdViewModel
    {
        public int BroodId { get; set; }

        public string BroodName { get; set; } = string.Empty;

        public DateTime BroodDate { get; set; }

        public int BroodBirdInitialNum { get; set; }

        public string? BroodDescription { get; set; }

        public bool BroodState { get; set; }

        public int BroilerHouseId { get; set; }

        public string BroilerHouseName { get; set; } = string.Empty;
    }
}