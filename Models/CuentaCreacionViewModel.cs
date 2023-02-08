using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestoNetCore.Models
{
    public class CuentaCreacionViewModel : Cuenta
    {
        public IEnumerable<SelectListItem> listaTipoCuenta{ get; set; }
    }
}
