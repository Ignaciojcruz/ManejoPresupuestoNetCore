using AutoMapper;
using ManejoPresupuestoNetCore.Models;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
        }
    }
}
