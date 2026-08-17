/*
 * ViewModel | Used for operational modules with forms, foreign keys and dropdown lists.
 * It separates database entities from UI requirements and avoids adding
 * presentation-only properties to Entity classes.
 * Official documentation:
 * https://learn.microsoft.com/aspnet/mvc/overview/older-versions-1/nerddinner/use-viewdata-and-implement-viewmodel-classes
 */


/*
 * Architecture Decision | Entity Model
 * This class represents only the persistent data stored in the "broods"
 * database table. Following the Separation of Concerns principle,
 * presentation-specific data (dropdowns, UI lists, etc.) will be implemented
 * through ViewModels instead of Entity classes.
 * Reference:
 * https://learn.microsoft.com/aspnet/core/mvc/overview
 */


using ProjectGranjaLaFlor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_GranjaLaFlor.Models.Entities
{
    /*
     * Entity Model | Represents the broods table in the database.
     * This class only maps persistent data and should not contain
     * UI-only properties such as dropdown lists.
     */
    [Table("broods")]
    public class Brood
    {
        [Key]
        [Column("brood_id")]
        public int BroodId { get; set; }

        [Display(Name = "Nombre de la Camada")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        [Column("brood_name")]
        public string BroodName { get; set; } = string.Empty;

        [Display(Name = "Fecha de Ingreso")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Date)]
        [Column("brood_date")]
        public DateTime BroodDate { get; set; }

        [Display(Name = "Cantidad Inicial de Aves")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [Column("brood_bird_initial_num")]
        public int BroodBirdInitialNum { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(150, ErrorMessage = "La descripción no puede superar los 150 caracteres.")]
        [Column("brood_description")]
        public string? BroodDescription { get; set; }

        [Display(Name = "Activo/Inactivo")]
        [Column("brood_state")]
        public bool BroodState { get; set; } = true;

        [Display(Name = "Pollera")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [Column("broiler_house_id")]
        public int BroilerHouseId { get; set; }

        /*
         * Navigation Property | Allows EF Core to load the related BroilerHouse.
         * BroilerHouseId stores the FK value; BroilerHouse represents the relationship.
         */
        public BroilerHouse? BroilerHouse { get; set; }

        /*
         * Navigation property that represents all
         * Daily Checks associated with the current Brood.
         *
         * One Brood can contain multiple Daily Checks.
        */
        public ICollection<DailyCheck> DailyChecks { get; set; } = new List<DailyCheck>();

        /*
         * Navigation property that represents all
         * Brood Reports associated with the current Brood.
         *
         * One Brood can contain multiple historical
         * Brood Reports generated over time.
         */
        public ICollection<BroodReport> BroodReports { get; set; }
            = new List<BroodReport>();

    }
}