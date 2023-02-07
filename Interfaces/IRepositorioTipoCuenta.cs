using ManejoPresupuestoNetCore.Models;

namespace ManejoPresupuestoNetCore.Interfaces
{
    public interface IRepositorioTipoCuenta
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Eliminar(int id);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
}
