﻿using Dapper;
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

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@"
                                SELECT * FROM Categorias where usuarioId = @usuarioId "
                                , new { usuarioId });            
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