using ManejoPresupuestoNetCore.Models;

namespace ManejoPresupuestoNetCore.Interfaces
{
    public interface IRepositorioCuenta
    {
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
    }
}
