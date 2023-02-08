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

        public CuentasController(IRepositorioTipoCuenta repositorioTipoCuenta
            ,IServicioUsuarios servicioUsuarios
            ,IRepositorioCuenta repositorioCuenta) 
        {
            this._repositorioTipoCuenta = repositorioTipoCuenta;
            this._servicioUsuarios = servicioUsuarios;
            this._repositorioCuenta = repositorioCuenta;
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

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await _repositorioTipoCuenta.Obtener(usuarioId);
            return tiposCuentas.Select(tc => new SelectListItem(tc.Nombre, tc.Id.ToString()));
        }


    }
}
