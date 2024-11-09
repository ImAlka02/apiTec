using FluentValidation;
using apiTec.Models.DTO_s;

namespace apiTec.Models.Validators
{
    public class datosCredencialDTOValidator : AbstractValidator<datosCredencialDTO>
    {
        public datosCredencialDTOValidator()
        {
            RuleFor(x => x.Vigencia)
                .NotEmpty().WithMessage("La Vigencia es requerida.");

            RuleFor(x => x.NumControl)
                .NotEmpty().WithMessage("El Número de Control es requerido.");

            RuleFor(x => x.NombreAlumno)
                .NotEmpty().WithMessage("El Nombre del Alumno es requerido.");

            RuleFor(x => x.Carrera)
                .NotEmpty().WithMessage("La Carrera es requerida.");

            RuleFor(x => x.NSS)
                .NotEmpty().WithMessage("El NSS es requerido.");

            RuleFor(x => x.CURP)
                .NotEmpty().WithMessage("El CURP es requerido.");

            RuleFor(x => x.Periodo)
                .NotEmpty().WithMessage("El Periodo es requerido.");
        }
    }
}
