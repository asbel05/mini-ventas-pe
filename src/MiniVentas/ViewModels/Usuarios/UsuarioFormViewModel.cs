using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiniVentas.ViewModels.Usuarios
{
    public class UsuarioFormViewModel : IValidatableObject
    {
        public int UsuarioId { get; set; }
        [Required(ErrorMessage = "El usuario de acceso es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 100 caracteres.")]
        [Display(Name = "Usuario de Acceso")]
        public string NombreUsuario { get; set; } = string.Empty;
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "Seleccione el rol del usuario.")]
        [Display(Name = "Rol")]
        public byte? RolId { get; set; }
        [DataType(DataType.Password)]
        [StringLength(64, ErrorMessage = "La contraseña no puede superar los 64 caracteres.")]
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }
        [StringLength(24, ErrorMessage = "El código de vendedor no puede superar los 24 caracteres.")]
        [Display(Name = "Código de Vendedor")]
        public string? CodigoVendedor { get; set; }
        [StringLength(80, ErrorMessage = "La zona de ventas no puede superar los 80 caracteres.")]
        [Display(Name = "Zona de Ventas / Caja")]
        public string? ZonaVentas { get; set; }
        [Display(Name = "Usuario activo")]
        public bool Activo { get; set; } = true;
        public bool EsEdicion => UsuarioId > 0;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var user = NombreUsuario?.Trim() ?? string.Empty;
            if (!Regex.IsMatch(user, @"^[a-zA-Z0-9._-]+$"))
            {
                yield return new ValidationResult(
                    "El usuario de acceso solo puede contener letras, números, puntos, guiones o guiones bajos (sin espacios).",
                    [nameof(NombreUsuario)]);
            }
            if (RolId is not (1 or 2))
            {
                yield return new ValidationResult(
                    "Seleccione un rol válido.",
                    [nameof(RolId)]);
            }
            if (!EsEdicion && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "La contraseña es obligatoria para crear un usuario.",
                    [nameof(Password)]);
            }
            else if (!string.IsNullOrEmpty(Password) && Password.Length < 6)
            {
                yield return new ValidationResult(
                    "La contraseña debe tener al menos 6 caracteres.",
                    [nameof(Password)]);
            }
            bool tieneCodigo = !string.IsNullOrWhiteSpace(CodigoVendedor);
            bool tieneZona = !string.IsNullOrWhiteSpace(ZonaVentas);
            if (RolId == 2 || tieneCodigo || tieneZona)
            {
                if (!tieneCodigo)
                {
                    yield return new ValidationResult(
                        "El código de vendedor es obligatorio para el perfil de ventas.",
                        [nameof(CodigoVendedor)]);
                }
                if (!tieneZona)
                {
                    yield return new ValidationResult(
                        "La zona de ventas es obligatoria para el perfil de ventas.",
                        [nameof(ZonaVentas)]);
                }
            }
        }
    }
}