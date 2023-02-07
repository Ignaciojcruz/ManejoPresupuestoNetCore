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
            var id = await connection.QuerySingleAsync<int>
                                                    ("TiposCuentas_Insertar",
                                                    new {usuarioId = tipoCuenta.UsuarioId,
                                                        nombre = tipoCuenta.Nombre},
                                                    commandType: System.Data.CommandType.StoredProcedure);
                                                    
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

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryAsync<TipoCuenta>(@"Select Id, Nombre, Orden
                                                    from TiposCuentas
                                                    where usuarioId = @usuarioId
                                                    order by orden;", new { usuarioId});
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"update TiposCuentas
                                    set nombre = @nombre
                                    where id = @id ", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var con = new SqlConnection(connectionString);
            return await con.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT id, nombre, orden
                                                                FROM TiposCuentas 
                                                                WHERE id = @id AND usuarioId = @usuarioId"
                                                                , new {id, usuarioId});            
        }

        public async Task Eliminar(int id)
        {
            var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(@"delete from TiposCuentas
                                     where id = @id"
                                    , new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden where Id =@Id;";
            using var con = new SqlConnection(connectionString);
            await con.ExecuteAsync(query, tipoCuentasOrdenados);
        }
    }
}
