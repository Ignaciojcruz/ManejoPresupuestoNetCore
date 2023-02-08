using ManejoPresupuestoNetCore.Models;

namespace ManejoPresupuestoNetCore.Interfaces
{
    public interface IRepositorioCuenta
    {
        Task Crear(Cuenta cuenta);
    }
}
