using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace ManejoPresupuestoNetCore.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IServicioUsuarios _servicioUsuarios;

        public CategoriasController(IRepositorioCategorias repositorioCategorias,
                                    IServicioUsuarios servicioUsuarios)
        {
            this._repositorioCategorias = repositorioCategorias;
            this._servicioUsuarios = servicioUsuarios;
        }

        [HttpGet]
        public IActionResult Crear() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if(!ModelState.IsValid) return View(categoria);

            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            categoria.UsuarioId = usuarioId;    
            await _repositorioCategorias.Crear(categoria);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var categoria = await _repositorioCategorias.Obtener(usuarioId);

            return View(categoria);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerPorId(id, usuarioId);

            if(categoria is null) 
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerPorId(categoriaEditar.Id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.UsuarioId= usuarioId;
            await _repositorioCategorias.Actualizar(categoriaEditar);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerPorId(id, usuarioId);

            if (categoria is null) return RedirectToAction("NoEncontrado", "Home");

            return View(categoria);

        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = _servicioUsuarios.obtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerPorId(id, usuarioId);

            if (categoria is null) return RedirectToAction("NoEncontrado", "Home");

            await _repositorioCategorias.Borrar(id, usuarioId);

            return RedirectToAction("Index");
        }

    }
}
