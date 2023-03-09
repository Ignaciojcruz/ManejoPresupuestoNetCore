using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoNetCore.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTipoCuenta repositorio;
        private readonly IServicioUsuarios _servicioUsuarios;

        public TiposCuentasController(IRepositorioTipoCuenta repositorioTipoCuenta,
                                        IServicioUsuarios servicioUsuarios) 
        {
            this.repositorio = repositorioTipoCuenta;     
            this._servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tiposCuentas = await repositorio.Obtener(usuarioId);

            return View(tiposCuentas);

        }

        [HttpGet]
        public IActionResult Crear()
        {                        
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid) return View(tipoCuenta);

            tipoCuenta.UsuarioId = _servicioUsuarios.obtenerUsuarioId();

            bool yaExiste = await repositorio.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if(yaExiste)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                                        $"El nombre {tipoCuenta.Nombre} ya existe. ");
                return View(tipoCuenta);
            }

            await repositorio.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificaTipoCuenta(string nombre, int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorio.Existe(nombre, usuarioId, id);

            if(yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tipoCuenta = await repositorio.ObtenerPorId(id, usuarioId);

            if(tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tipoCuentaExiste = await repositorio.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if(tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorio.Actualizar(tipoCuenta);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tipoCuenta = await repositorio.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tipoCuentaExiste = await repositorio.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorio.Eliminar(tipoCuenta.Id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var tiposCuentas = await repositorio.Obtener(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(t => t.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if(idsTiposCuentasNoPertenecenAlUsuario.Count > 0) return Forbid();

            var tiposCuentasOrdenadas = ids.Select((valor, indice) =>
                                                    new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorio.Ordenar(tiposCuentasOrdenadas);

            return Ok();
        }
    }
}
