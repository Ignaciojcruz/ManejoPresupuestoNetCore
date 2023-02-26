namespace ManejoPresupuestoNetCore.Models
{
    public class ReporteSemanalViewModel
    {
        public int Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public int Gastos => TransaccionesPorSemana.Sum(x => x.Gastos);
        public int Total => Ingresos - Gastos;
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ResultadoObtenerPorSemana> TransaccionesPorSemana { get; set; }
    }
}
