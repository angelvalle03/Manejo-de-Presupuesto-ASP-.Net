using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{

    public interface IRepositoryCuentas
    {
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
    }
    public class RepositoryCuentas : IRepositoryCuentas
    {
        private readonly string connectionString;
        

        public RepositoryCuentas(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        

        public async Task Crear(Cuenta cuenta)
        {
            var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Cuentas(Nombre, TipoCuentaId, Descripcion,Balance)
                                                        VALUES (@Nombre, @TipoCuentaId, @Descripcion, @Balance)

                                                        SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }
    
        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                                                            FROM Cuentas
                                                            INNER JOIN TiposCuentas tc 
                                                            on tc.Id = Cuentas.TipoCuentaId
                                                            WHERE tc.UsuarioId = @UsuarioId
                                                            ORDER BY tc.Orden", new { usuarioId });
        }
    
    }
}
