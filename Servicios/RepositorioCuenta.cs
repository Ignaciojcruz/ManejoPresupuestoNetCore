using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace ManejoPresupuestoNetCore.Servicios
{
    public class RepositorioCuenta : IRepositorioCuenta
    {
        private readonly string connectionString;

        public RepositorioCuenta(IConfiguration configuration) 
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
                                                ("Cuentas_Insertar",
                                                new { Nombre = cuenta.Nombre,
                                                    TipoCuentaId = cuenta.TipoCuentaId,
                                                    Balance = cuenta.Balance,
                                                    Descripcion = cuenta.Descripcion},
                                                commandType: System.Data.CommandType.StoredProcedure);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"
                                                        SELECT c.Id, c.Nombre, Balance, tc.Nombre as TipoCuenta
                                                        From Cuentas c
                                                        inner join TiposCuentas tc on
                                                        tc.Id = c.TipoCuentaId
                                                        where tc.UsuarioId = @UsuarioId
                                                        order by tc.Orden",
                                                        new { UsuarioId });
        }
    }
}
