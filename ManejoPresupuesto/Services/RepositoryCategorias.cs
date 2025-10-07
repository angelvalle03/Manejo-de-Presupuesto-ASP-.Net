using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{
    public interface IrepositoryCategorias
    {
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> GetCategorias(int usuarioid);
    }
    public class RepositoryCategorias : IrepositoryCategorias
    {
        private readonly string connectionString;
        public RepositoryCategorias(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear( Categoria categoria)
        {
            using var connection =  new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Categorias (Nombre, TipoOperacionId, UsuarioId) 
                                                           VALUES (@Nombre, @TipoOperacionId, @UsuarioId);
                                                            SELECT SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias(int usuarioid)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>("SELECT * FROM Categorias WHERE UsuarioId = @usuarioid", new { usuarioid });
        }
    }
}
