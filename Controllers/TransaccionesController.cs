using AutoMapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using ManejoPresupuestoNetCore.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuestoNetCore.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioTransacciones _repositorioTransacciones;
        private readonly IRepositorioCuenta _repositorioCuentas;
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IMapper _mapper;
        private readonly IServicioReportes _servicioReportes;

        public TransaccionesController(IServicioUsuarios servicioUsuarios
                                        , IRepositorioTransacciones repositorioTransacciones
                                        , IRepositorioCuenta repositorioCuenta
                                        , IRepositorioCategorias repositorioCategorias
                                        , IMapper mapper
                                        , IServicioReportes servicioReportes)
        {
            this._servicioUsuarios = servicioUsuarios;
            this._repositorioTransacciones = repositorioTransacciones;
            this._repositorioCuentas = repositorioCuenta;
            this._repositorioCategorias = repositorioCategorias;
            this._mapper = mapper;
            this._servicioReportes = servicioReportes;

        }
        public async Task<IActionResult> Index(int mes, int año)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();

            var modelo = await _servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioId, mes, año, ViewBag);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();

            if(!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);
            if (cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            var categoria = await _repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);
            if(categoria is null) return RedirectToAction("NoEncontrado", "Home");

            modelo.UsuarioId= usuarioId;

            if(modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await _repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var transaccion = await _repositorioTransacciones.ObtenerPorId(id, usuarioId);

            if(transaccion is null) return RedirectToAction("NoEncontrado", "Home");

            var modelo = _mapper.Map<TransaccionActualizacionViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;
            if (modelo.TipoOperacionId == TipoOperacion.Gasto) modelo.MontoAnterior *= modelo.Monto - 1;

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno= urlRetorno;

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();

            if(!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);
            if (cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            var categoria = await _repositorioCategorias.ObtenerPorId(modelo.CategoriaId, usuarioId);
            if (categoria is null) return RedirectToAction("NoEncontrado", "Home");

            var transaccion = _mapper.Map<Transaccion>(modelo);
                        
            if (modelo.TipoOperacionId == TipoOperacion.Gasto) transaccion.Monto *= -1;

            await _repositorioTransacciones.Actualizar(transaccion,
                                                        modelo.MontoAnterior,
                                                        modelo.CuentaAnteriorId);

            if(string.IsNullOrEmpty(modelo.UrlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();

            var transaccion = await _repositorioTransacciones.ObtenerPorId(id, usuarioId);
            if (transaccion is null) return RedirectToAction("NoEncontrado", "Home");

            await _repositorioTransacciones.Borrar(id);
            
            if (string.IsNullOrEmpty(urlRetorno))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno);
            }

        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);

        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await _repositorioCuentas.Buscar(usuarioId);
            return cuentas.Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await _repositorioCategorias.Obtener(usuarioId, tipoOperacion);
            return categorias.Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }
        
    
    }
}
