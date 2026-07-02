using System.ComponentModel.DataAnnotations;

namespace DB_GranjaLaFlor.Models.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Correo Electrónico")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [EmailAddress(ErrorMessage = "Ingrese un formato de correo electrónico válido.")]
        public string UserEmail { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; } = string.Empty;
        /*
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
        */
    }
}