namespace ManejoPresupuesto.Services
{
    public interface IServicioUsuario
    {
        int GetUsuarioId();
    }
    public class ServicioUsuarios : IServicioUsuario
    {
        //trae el id de usuario 1 hardcodeado
        public int GetUsuarioId()
        {
            return 1;
        }
    }
}
