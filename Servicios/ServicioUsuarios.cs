using ManejoPresupuestoNetCore.Interfaces;
using System.Security.Claims;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly HttpContext httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor) 
        {
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public int obtenerUsuarioId()
        {
            if(httpContext.User.Identity.IsAuthenticated) 
            {
                var idClaim = httpContext.User
                    .Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
            }

            throw new ApplicationException("El usuario no esta autenticado");
            
            
        }
    }
}
