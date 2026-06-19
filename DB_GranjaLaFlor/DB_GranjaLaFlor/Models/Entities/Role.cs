using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
/*
Using DataAnnotations as it describe how a model should be validated, what the value must meet. Used for simple validations.
Use Fluet API for logic validations (complex validations such as 
 */
namespace DB_GranjaLaFlor.Models.Entities
{
    [Table("roles")]
    public class Role
    {
        //setting properties 
        //initialize properties as it requires a initial value which does not have yet. This option does not allow NULL values. 
        //each property maps to a table in the DB. "Column" helps to map porperty to DB column 
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre del Rol")]
        [Column("role_name")]
        [StringLength(30, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Añadir descripción")]
        [Column("role_description")]
        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        public string? RoleDescription { get; set; }

        // RoleState is used to implement Soft Delete.
        // Records are never physically deleted from the database.

        [Required]
        [Display(Name = "Activo/Inactivo")]
        [Column("role_state")]
        public bool RoleState { get; set; } = true;
    }

}
