using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB_GranjaLaFlor.Models.Entities
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Display(Name = "Nombre del Usuario")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras y espacios.")]
        [Column("user_name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [EmailAddress(ErrorMessage = "Ingrese un formato de correo electrónico válido.")]
        [StringLength(50, ErrorMessage = "El correo no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "Solo se permiten letras y espacios.")]
        [Column("user_email")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 30 caracteres.")]
        [DataType(DataType.Password)]
        [Column("user_password")]
        public string UserPassword { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(150, ErrorMessage = "La descripción no puede superar los 150 caracteres.")]
        [Column("user_description")]
        public string? UserDescription { get; set; }

        [Display(Name = "Activo/Inactivo")]
        [Column("user_state")]
        public bool UserState { get; set; } = true;

        [Display(Name = "Rol")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [Column("role_id")]
        public int RoleId { get; set; }

        // EF Core uses navegation property to move through other entities. Need ID for FK. 
        public Role? Role { get; set; }
    }
}