using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services
{

    public interface IRepositoryCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> GetPorId(int id, int Usuarioid);
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

            //Esta consulta me va a traer todas las cuentas que tengan un tipo cuenta asociado y que a su vez el usuarioid sea el que se le pasa por parametro
            return await connection.QueryAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.TipoCuentaId, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                                                            FROM Cuentas
                                                            INNER JOIN TiposCuentas tc 
                                                            on tc.Id = Cuentas.TipoCuentaId
                                                            WHERE tc.UsuarioId = @usuarioId
                                                            ORDER BY tc.Orden", new { usuarioId });
        }
        


        //Este metodo permite obtener el id de la cuenta que se va a editar
        public async Task<Cuenta> GetPorId(int id, int Usuarioid)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"SELECT Cuentas.Id, Cuentas.TipoCuentaId ,Cuentas.Nombre, Balance, Descripcion, tc.Id
                                                            FROM Cuentas
                                                            INNER JOIN TiposCuentas tc 
                                                            on tc.Id = Cuentas.TipoCuentaId
                                                            WHERE tc.UsuarioId = @Usuarioid AND Cuentas.Id = @id", new { id, Usuarioid });

        }



        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE Cuentas
                                        SET Nombre= @Nombre, Balance= @Balance, Descripcion= @Descripcion, 
                                        TipoCuentaId = @tipoCuentaId
                                        WHERE Id = @Id;", cuenta);

        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE Cuentas WHERE Id = @Id", new {id});

        }

    }
}
