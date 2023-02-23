using AutoMapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuestoNetCore.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTipoCuenta _repositorioTipoCuenta;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioCuenta _repositorioCuenta;
        private readonly IRepositorioTransacciones _repositorioTransacciones;
        private readonly IMapper _mapper;

        public CuentasController(IRepositorioTipoCuenta repositorioTipoCuenta
            ,IServicioUsuarios servicioUsuarios
            ,IRepositorioCuenta repositorioCuenta
            ,IRepositorioTransacciones repositorioTransacciones
            ,IMapper mapper)  
        {
            this._repositorioTipoCuenta = repositorioTipoCuenta;
            this._servicioUsuarios = servicioUsuarios;
            this._repositorioCuenta = repositorioCuenta;
            this._repositorioTransacciones = repositorioTransacciones;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var cuentaConTipoCuenta = await _repositorioCuenta.Buscar(usuarioId);

            var modelo = cuentaConTipoCuenta
                        .GroupBy(x => x.TipoCuenta)
                        .Select(grupo => new IndiceCuentasViewModel
                        {
                            TipoCuenta = grupo.Key,
                            Cuentas = grupo.AsEnumerable()
                        }).ToList();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tiposCuentas = await _repositorioTipoCuenta.Obtener(usuarioId);
            var modelo = new CuentaCreacionViewModel();
            modelo.listaTipoCuenta = tiposCuentas.Select(tc => new SelectListItem(tc.Nombre, tc.Id.ToString()));

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tipoCuenta = await _repositorioTipoCuenta.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.listaTipoCuenta = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);

            }

            await _repositorioCuenta.Crear(cuenta);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerPorId(id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //var modelo = new CuentaCreacionViewModel()
            //{
            //    Id = cuenta.Id,
            //    Nombre = cuenta.Nombre,
            //    TipoCuentaId = cuenta.TipoCuentaId,
            //    Descripcion = cuenta.Descripcion,
            //    Balance = cuenta.Balance,
            //    TipoCuenta = cuenta.TipoCuenta
            //};

            var modelo = _mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.listaTipoCuenta = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);

        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if(cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            var tipoCuenta = _repositorioTipoCuenta.ObtenerPorId(cuentaEditar.Id, usuarioId);

            if (tipoCuenta is null) return RedirectToAction("NoEncontrado", "Home");
                        
            await _repositorioCuenta.Actualizar(cuentaEditar);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            await _repositorioCuenta.Borrar(id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id, int mes, int año)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerPorId(id, usuarioId);

            if (cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            DateTime fechaInicio, fechaFin;

            if(mes <= 0 || mes > 12 || año < 1900)
            {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                fechaInicio = new DateTime(año, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var transaccionPorCuenta = new TransaccionPorCuenta()
            {
                CuentaId = id,
                UsuarioId = usuarioId,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await _repositorioTransacciones
                                        .ObtenerPorCuentaId(transaccionPorCuenta);

            var modelo = new ReporteTransaccionesDetalladas();
            ViewBag.Cuenta = cuenta.Nombre;

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

            ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;

            return View(modelo);                

        }


        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await _repositorioTipoCuenta.Obtener(usuarioId);
            return tiposCuentas.Select(tc => new SelectListItem(tc.Nombre, tc.Id.ToString()));
        }

        
    }
}
