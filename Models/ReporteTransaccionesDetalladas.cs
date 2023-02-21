namespace ManejoPresupuestoNetCore.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public double BalanceDepositos => TransaccionesAgrupadas.Sum(t => t.BalanceDepositos);
        public double BalanceRetiros => TransaccionesAgrupadas.Sum(t => t.BalanceRetiros);
        public double Total => BalanceDepositos - BalanceRetiros;


        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }

            public IEnumerable<Transaccion> Transacciones { get; set; }

            public double BalanceDepositos =>
                Transacciones.Where(t => t.TipoOperacionId == TipoOperacion.Ingreso)
                .Sum(m => m.Monto);

            public double BalanceRetiros =>
                Transacciones.Where(t => t.TipoOperacionId == TipoOperacion.Gasto)
                .Sum(m => m.Monto);
        }
    }
}
