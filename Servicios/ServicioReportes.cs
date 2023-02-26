using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class ServicioReportes : IServicioReportes
    {
        private readonly IRepositorioTransacciones _repositorioTransacciones;
        private readonly HttpContext _httpContext;

        public ServicioReportes(IRepositorioTransacciones repositorioTransacciones
                                , IHttpContextAccessor httpContextAccessor) 
        {
            this._repositorioTransacciones = repositorioTransacciones;
            this._httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerReporteSemanal(int usuarioId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);
            
            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            AsignarValoresAlViewBag(ViewBag, fechaInicio);
            var modelo = await _repositorioTransacciones.ObtenerPorSemana(parametro);
            return modelo;
        }

        public async Task<ReporteTransaccionesDetalladas>
            ObtenerReporteTransaccionesDetalladas(int usuarioId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var parametro = new ParametroObtenerTransaccionesPorUsuario()
            {
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await _repositorioTransacciones.ObtenerPorUsuarioId(parametro);

            var modelo = GeneerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);

            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }


        public async Task<ReporteTransaccionesDetalladas> 
            ObtenerReporteTransaccionesDetalladasPorCuenta(int usuarioId, int cuentaId, int mes, int año, dynamic ViewBag)
        {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFechaInicioYFin(mes, año);

            var transaccionPorCuenta = new TransaccionPorCuenta()
            {
                CuentaId = cuentaId,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await _repositorioTransacciones
                                        .ObtenerPorCuentaId(transaccionPorCuenta);
            var modelo = GeneerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);

            AsignarValoresAlViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        private void AsignarValoresAlViewBag(dynamic ViewBag, DateTime fechaInicio)
        {
            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = _httpContext.Request.Path + _httpContext.Request.QueryString;
        }

        private static ReporteTransaccionesDetalladas GeneerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFin, IEnumerable<Transaccion> transacciones)
        {
            var modelo = new ReporteTransaccionesDetalladas();

            var transaccionesPorFecha = transacciones.OrderByDescending(t => t.FechaTransaccion)
                            .GroupBy(t => t.FechaTransaccion)
                            .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
                            {
                                FechaTransaccion = grupo.Key,
                                Transacciones = grupo.AsEnumerable()
                            });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            return modelo;
        }

        private (DateTime fechaInicio, DateTime fechaFin) GenerarFechaInicioYFin(int mes, int año)
        {
            DateTime fechaInicio, fechaFin;

            if (mes <= 0 || mes > 12 || año < 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
