using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class RepositorioTipoCuenta : IRepositorioTipoCuenta
    {
        private readonly string connectionString;
        public RepositorioTipoCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            
            using var connection = new SqlConnection(connectionString);            
            var id = await connection.QuerySingleAsync<int>($@"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden)
                                                    VALUES (@Nombre, @UsuarioId, 0);
                                                    SELECT SCOPE_IDENTITY()", tipoCuenta);
            tipoCuenta.Id = id;
            
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"
                                                    SELECT 1 FROM TiposCuentas
                                                    WHERE Nombre = @Nombre AND usuarioId = @UsuarioId",
                                                    new { nombre, usuarioId });
            return existe == 1;
        }
    }
}
