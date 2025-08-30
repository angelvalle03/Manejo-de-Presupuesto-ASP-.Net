using ManejoPresupuesto.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ManejoPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        [Remote(action: "ValidateTipoCuentaExistente", controller: "TiposCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        //validaciones por modelos
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Nombre!= null && Nombre.Length > 0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();
        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayuscula", new[] {nameof(Nombre)});
        //        }
        //    }
        //}

        //Prueba otras validaciones
        //[Required(ErrorMessage ="El campo {0} es requerido")]
        //[EmailAddress(ErrorMessage ="El campo {0} debe ser un Email valido")]
        //public string Email { get; set; }

        //[Range(minimum:18, maximum:130, ErrorMessage ="El edad debe estar entre {1} y {2}")]
        //public int Edad { get; set; }

        //[Url(ErrorMessage ="El campo {0} debe ser una URL valida")]
        //public string URL { get; set; }


        //[CreditCard(ErrorMessage ="El campo {0} debe ser una tarjeta de credito valida")]
        //[DisplayName("Tarjeta de credito")]
        //public int TarjetaDeCredito { get; set; }
    }
}
