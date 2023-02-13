using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration) 
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");    
        }

        public async Task Crear(Transaccion transaccion)
        {
            var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                },
                commandType: System.Data.CommandType.StoredProcedure);
            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, double montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                                            new
                                            {
                                                transaccion.Id,
                                                transaccion.FechaTransaccion,
                                                transaccion.Monto,
                                                transaccion.CategoriaId,
                                                transaccion.CuentaId,
                                                transaccion.Nota,
                                                montoAnterior,
                                                cuentaAnteriorId
                                            },
                                            commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"
                                SELECT t.*, cat.TipoOperacionId
                                from transacciones t
                                inner join Categorias cat on
                                t.CategoriaId = cat.Id
                                where t.Id = @Id and t.usuarioId = @usuarioId",
                                new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar",
                                            new { id },
                                            commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
