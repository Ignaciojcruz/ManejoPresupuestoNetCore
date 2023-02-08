namespace ManejoPresupuestoNetCore.Models
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta { get; set; }

        public IEnumerable<Cuenta> Cuentas { get; set; }

        public double Balance => Cuentas.Sum(x => x.Balance);

    }
}
