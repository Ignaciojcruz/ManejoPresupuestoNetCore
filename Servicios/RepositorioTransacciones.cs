﻿using Dapper;
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

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(TransaccionPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
                                SELECT t.Id, t.Monto, t.FechaTransaccion, ca.Nombre as Categoria
		                                ,cu.Nombre as Cuenta, ca.TipoOperacionId
                                FROM Transacciones t
                                INNER JOIN Categorias ca on
	                                t.CategoriaId = ca.Id
                                INNER JOIN Cuentas cu on
	                                t.CuentaId = cu.Id
                                WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin"
                                , modelo);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
                                SELECT t.Id, t.Monto, t.FechaTransaccion, ca.Nombre as Categoria
		                                ,cu.Nombre as Cuenta, ca.TipoOperacionId
                                FROM Transacciones t
                                INNER JOIN Categorias ca on
	                                t.CategoriaId = ca.Id
                                INNER JOIN Cuentas cu on
	                                t.CuentaId = cu.Id
                                WHERE t.UsuarioId = @UsuarioId
                                AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                                ORDER BY t.FechaTransaccion DESC"
                                , modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(
            ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"
                                select  datediff(d, @fechaInicio, FechaTransaccion) / 7 + 1 as semana,
                                sum(monto) as monto, cat.TipoOperacionId
                                from Transacciones t
                                inner join Categorias cat
                                on cat.id = t.CategoriaId
                                where t.UsuarioId = @usuarioId and
                                FechaTransaccion between @fechaInicio and @fechaFin
                                group by datediff(d, @fechaInicio, FechaTransaccion) / 7, cat.TipoOperacionId 
                                ", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"
                                select month(t.FechaTransaccion) as mes,
                                sum(monto) as monto, cat.TipoOperacionId
                                from Transacciones t
                                inner join Categorias cat on
	                                t.categoriaId = cat.id
                                where t.UsuarioId = @usuarioId and year(FechaTransaccion) = @año
                                group by month(FechaTransaccion), cat.TipoOperacionId
                                ", new {usuarioId, año});
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
