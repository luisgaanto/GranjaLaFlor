using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/*
 * Data Annotations define simple validation rules and UI directly on the Entity.
 *
 * More complex business validations should be implemented in the corresponding Service class.
 *
 * Entity Framework Fluent API should be used when configuring relationships, constraints or database mappings that cannot be
 * expressed through Data Annotations.
 */

namespace DB_GranjaLaFlor.Models.Entities
{
    [Table("roles")]
    public class Role
    {
        // Entity properties
        //  Initialize string properties with an empty value to satisfy nullable reference types.
        // each property maps to a table in the DB. "Column" helps to map porperty to DB column 
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Display(Name = "Nombre del Rol")]
        [Required(ErrorMessage = "Este campo es requerido")]
        [StringLength(30, ErrorMessage = "El nombre no puede superar los 30 caracteres.")]
        [Column("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        [Column("role_description")]
        public string? RoleDescription { get; set; }

        // RoleState is used to implement Soft Delete.
        // Records are never physically deleted from the database.
        
        [Display(Name = "Activo/Inactivo")]
        [Column("role_state")]
        public bool RoleState { get; set; } = true;
    }

}
