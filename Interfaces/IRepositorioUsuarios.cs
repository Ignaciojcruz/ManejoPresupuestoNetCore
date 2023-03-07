using ManejoPresupuestoNetCore.Models;
using ManejoPresupuestoNetCore.Servicios;

namespace ManejoPresupuestoNetCore.Interfaces
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }
}
