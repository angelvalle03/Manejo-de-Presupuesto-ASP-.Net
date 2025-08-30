using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;

namespace ManejoPresupuesto.Services
{
    //Todo lo que tiene que ver con consultas a la base de datos se hace aca

    public interface IRepostiroyTiposCuentas
    {
        Task Borrar(int id);
        Task Create(TipoCuenta tipoCuenta);
        Task<bool> Exist(string Nombre, int UsuarioId);
        Task<TipoCuenta> GetPorId(int Id, int UsuarioId);
        Task<IEnumerable<TipoCuenta>> GetTiposCuentas(int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenar);
        Task UpdateTipoCuenta(TipoCuenta tipoCuenta);
    }
    public class RepositoryTiposCuentas : IRepostiroyTiposCuentas
    {
        private readonly string connectionString;

        public RepositoryTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(TipoCuenta tipoCuenta)
        {
            //Extraer el id del registro que recien se creó QuerySingle
            //await espera asincrona del query
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", new {usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre}, commandType: System.Data.CommandType.StoredProcedure);  

            tipoCuenta.Id= id;
        }

        public async Task<bool> Exist(string Nombre, int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            var exists = await connection.QueryFirstOrDefaultAsync<int>(
                                                            @"SELECT 1 
                                                            FROM TiposCuentas
                                                            WHERE Nombre = @Nombre AND UsuarioId = @UsuarioId;", new { Nombre, UsuarioId });

            return exists == 1;

        }

        
        public async Task<IEnumerable<TipoCuenta>> GetTiposCuentas(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                            FROM TiposCuentas
                                                            Where UsuarioId = @UsuarioId
                                                            ORDER BY Orden", new {usuarioId});             
        }

        public async Task UpdateTipoCuenta(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id", tipoCuenta);
        }


        public async Task<TipoCuenta> GetPorId(int Id, int UsuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                        FROM TiposCuentas
                                                                        WHERE Id = @Id AND UsuarioId = @UsuarioId", new {Id,UsuarioId});
        }
    

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE TiposCuentas WHERE Id = @Id", new {id});
        } 


        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenar)
        {
            //Se actuliza cada campo Orden de cada tipo cuenta que traiga
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenar);
        }
    }
}
