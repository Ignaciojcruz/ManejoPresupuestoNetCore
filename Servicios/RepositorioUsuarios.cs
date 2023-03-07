using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;

        public RepositorioUsuarios(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var UsuarioId = await connection.QuerySingleAsync<int>(@"
                            INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                            VALUES (@Email, @EmailNormalizado, @PasswordHash);
                            SELECT SCOPE_IDENTITY();
                            ", usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo"
                                            , new { UsuarioId }
                                            , commandType: System.Data.CommandType.StoredProcedure);

            return UsuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var connection = new SqlConnection(this.connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                    "SELECT * FROM Usuarios where EmailNormalizado = @emailNormalizado",
                    new { emailNormalizado });
        }
    }
}
