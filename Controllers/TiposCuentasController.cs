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

        public TiposCuentasController(IRepositorioTipoCuenta repositorioTipoCuenta) 
        {
            this.repositorio = repositorioTipoCuenta;     
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

            tipoCuenta.UsuarioId = 1;

            bool yaExiste = await repositorio.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if(yaExiste)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                                        $"El nombre {tipoCuenta.Nombre} ya existe. ");
                return View(tipoCuenta);
            }

            await repositorio.Crear(tipoCuenta);

            return View();
        }
    }
}
