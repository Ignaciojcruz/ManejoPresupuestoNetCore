namespace ManejoPresupuestoNetCore.Models
{
    public class TransaccionActualizacionViewModel : TransaccionCreacionViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public double MontoAnterior { get; set; }
        public string UrlRetorno { get; set; }
    }
}
