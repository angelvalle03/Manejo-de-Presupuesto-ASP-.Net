using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{

    //Hereda de la clase cuenta
    public class CuentaCreacionViewModel : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }

    }
}
