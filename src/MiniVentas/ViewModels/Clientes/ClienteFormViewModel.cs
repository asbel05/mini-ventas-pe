using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiniVentas.ViewModels.Clientes
{
    public class ClienteFormViewModel : IValidatableObject
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de documento.")]
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(12, ErrorMessage = "El documento no puede superar los 12 caracteres.")]
        [Display(Name = "Número de Documento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre o razón social es obligatorio.")]
        [StringLength(160, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 160 caracteres.")]
        [Display(Name = "Nombre Completo o Razón Social")]
        public string Nombre { get; set; } = string.Empty;

        public bool EsEdicion => ClienteId > 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var doc = NumeroDocumento?.Trim() ?? string.Empty;

            if (TipoDocumento == "1") // DNI
            {
                if (!Regex.IsMatch(doc, @"^\d{8}$"))
                {
                    yield return new ValidationResult(
                        "El DNI debe contener exactamente 8 dígitos numéricos.",
                        new[] { nameof(NumeroDocumento) });
                }
            }
            else if (TipoDocumento == "6") // RUC
            {
                if (!Regex.IsMatch(doc, @"^(10|15|17|20)\d{9}$"))
                {
                    yield return new ValidationResult(
                        "El RUC debe tener 11 dígitos numéricos y empezar con 10, 15, 17 o 20.",
                        new[] { nameof(NumeroDocumento) });
                }
            }
            else if (TipoDocumento == "4") // Carnet de Extranjería
            {
                if (!Regex.IsMatch(doc, @"^[A-Za-z0-9]{8,12}$"))
                {
                    yield return new ValidationResult(
                        "El Carnet de Extranjería debe tener entre 8 y 12 caracteres alfanuméricos.",
                        new[] { nameof(NumeroDocumento) });
                }
            }
            else if (TipoDocumento == "7") // Pasaporte
            {
                if (!Regex.IsMatch(doc, @"^[A-Za-z0-9]{8,12}$"))
                {
                    yield return new ValidationResult(
                        "El Pasaporte debe tener entre 8 y 12 caracteres alfanuméricos.",
                        new[] { nameof(NumeroDocumento) });
                }
            }
            else
            {
                yield return new ValidationResult(
                    "El tipo de documento seleccionado no es válido.",
                    new[] { nameof(TipoDocumento) });
            }
        }
    }
}

