using Dapper;
using ManejoPresupuestoNetCore.Interfaces;
using ManejoPresupuestoNetCore.Models;
using Microsoft.Data.SqlClient;

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
    }
}
