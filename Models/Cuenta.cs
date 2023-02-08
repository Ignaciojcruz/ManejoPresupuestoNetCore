using ManejoPresupuestoNetCore.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuestoNetCore.Models
{
    public class Cuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la cuenta es requerido")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        [Display(Name = "Tipo Cuenta")]
        public int TipoCuentaId { get; set; }
        public double Balance { get; set; }

        [StringLength(maximumLength: 1000)]
        public string Descripcion { get; set; }
    }
}
