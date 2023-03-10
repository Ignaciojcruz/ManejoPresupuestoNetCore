using ManejoPresupuestoNetCore.Models;

namespace ManejoPresupuestoNetCore.Interfaces
{
    public interface IRepositorioCuenta
    {
        Task Actualizar(Cuenta cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
}
