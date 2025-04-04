using System.ComponentModel.DataAnnotations;

namespace Monolito_Modular.Application.DTOs
{
    public class UpdatePasswordDTO
    {
        [Required(ErrorMessage ="La contraseña actual es requerida.")]
        public required string CurrentPassword { get; set; }
        [Required(ErrorMessage ="La contraseña nueva es requerida.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ])[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+$", ErrorMessage = "La contraseña debe ser alfanumérica y contener al menos una mayúscula.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")] 
        [MaxLength(20, ErrorMessage = "La contraseña debe tener como máximo 20 caracteres")]
        public required string NewPassword { get; set; }
        [Required(ErrorMessage ="La confirmación de la contraseña nueva es requerida.")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public required string ConfirmPassword { get; set; }
    }
}