using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepostiroyTiposCuentas repositoryTiposCuentas;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IRepositoryCuentas repositoryCuentas;

        public CuentaController(IRepostiroyTiposCuentas repositoryTiposCuentas, IServicioUsuario servicioUsuario, IRepositoryCuentas repositoryCuentas)
        {
            this.repositoryTiposCuentas = repositoryTiposCuentas;
            this.servicioUsuario = servicioUsuario;
            this.repositoryCuentas = repositoryCuentas;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var cuentasConTipoCuenta = await repositoryCuentas.Buscar(usuarioId);


            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel()
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var tiposCuentas = await repositoryTiposCuentas.GetTiposCuentas(usuarioId);
            var modelo = new CuentaCreacionViewModel();
            //x.Nombre, es el texto del option set y el x.Id.ToString() es el value
            modelo.TiposCuentas = await GetTiposCuentas(usuarioId);
                

            return View(modelo);
        }

        [HttpPost]

        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var tipoCuenta = await repositoryTiposCuentas.GetPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await GetTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await repositoryCuentas.Crear(cuenta);

            return RedirectToAction("Index");

        }



        private async Task<IEnumerable<SelectListItem>> GetTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositoryTiposCuentas.GetTiposCuentas(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
