using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }  

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                                                            INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId)
                                                            Values (@Nombre, @TipoOperacionId, @UsuarioId);
                                                            SELECT SCOPE_IDENTITY();
                                                            ", categoria);
            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>($@"
                                SELECT * 
                                FROM Categorias     
                                WHERE usuarioId = @usuarioId 
                                ORDER BY Nombre
                                OFFSET {paginacion.RecordsASaltar } ROWS FETCH NEXT {paginacion.RecordsPorPagina}
                                ROWS ONLY"
                                , new { usuarioId });            
        }

        public async Task<int> Contar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM Categorias WHERE UsuarioId = @usuarioId", new {usuarioId});
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@"
                                SELECT * FROM Categorias where usuarioId = @usuarioId and TipoOperacionId = @TipoOperacionId "
                                , new { usuarioId, tipoOperacionId });
        }

        public async Task<Categoria> ObtenerPorId(int Id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"
                                                        SELECT * FROM Categorias 
                                                        where Id = @Id and usuarioId = @usuarioId; ",
                                                        new { Id, usuarioId });
            
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Categorias
                                            SET Nombre = @Nombre, TipoOperacionId = @TipoOperacionId
                                            WHERE Id = @Id and UsuarioId = @UsuarioId ",
                                            categoria);
        }

        public async Task Borrar(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Delete from Categorias
                                            where Id = @Id and usuarioId = @usuarioId",
                                            new {id, usuarioId});
        }
    }
}
