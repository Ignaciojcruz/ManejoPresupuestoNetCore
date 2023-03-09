using ManejoPresupuestoNetCore.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestoNetCore.Models
{
    public class TipoCuenta //: IValidatableObject  //para validaciones a nivel de modelo
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //[PrimeraLetraMayuscula]   --> Con las validaciones a nivel de modelo no se indican los attributos acá
        [Remote(action: "VerificaTipoCuenta", controller: "TiposCuentas",
                    AdditionalFields = nameof(Id))]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }



        ////Pruebas de otras validaciones por defecto ***********************************************

        //[Required(ErrorMessage = "El campo {0} es requerido")]
        //[EmailAddress(ErrorMessage = "El campo debe ser un correo válido")]
        //public string? Email { get; set; }

        //[Range(minimum: 18, maximum: 130, ErrorMessage = "El valor debe estar entre {1} y {2}")]
        //public int Edad{ get; set; }

        //[Url(ErrorMessage = "El campo debe ser una URL válida")]
        //public string URL{ get; set; }

        //[CreditCard(ErrorMessage = "La tarjeta de crédito no es válida")]
        //[DisplayName("Tarjeta de Crédito")]
        //public string? TarjetaDeCredito{ get; set; }
        ////Pruebas de otras validaciones por defecto ***********************************************
        ///


        //////Validaciones a nivel de modelo ***********************************************
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
            
        //    if(Nombre != null && Nombre.Length > 0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();

        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayúscula",
        //                                                new[] {nameof(Nombre)});
        //        }
        //    }
        //}
        //////Validaciones a nivel de modelo ***********************************************
    }
}
