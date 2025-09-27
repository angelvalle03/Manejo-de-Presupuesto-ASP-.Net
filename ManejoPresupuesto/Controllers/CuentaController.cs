using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using AutoMapper;

namespace ManejoPresupuesto.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepostiroyTiposCuentas repositoryTiposCuentas;
        private readonly IServicioUsuario servicioUsuario;
        private readonly IRepositoryCuentas repositoryCuentas;
        private readonly IMapper mapper;

        public CuentaController(IRepostiroyTiposCuentas repositoryTiposCuentas, IServicioUsuario servicioUsuario, IRepositoryCuentas repositoryCuentas , IMapper mapper)
        {
            this.repositoryTiposCuentas = repositoryTiposCuentas;
            this.servicioUsuario = servicioUsuario;
            this.repositoryCuentas = repositoryCuentas;
            this.mapper = mapper;
        }

        //Este metdod nos prmite mostrar las cuentas agrupadas por Tipo cuenta
        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var cuentasConTipoCuenta = await repositoryCuentas.Buscar(usuarioId);


            //Agrupa el resultado de la query por Tipo cuenta (ahorro, credito etc..)

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)  //Agrupa el resultado de la query por Tipo cuenta (ahorro, credito etc..)
                .Select(grupo => new IndiceCuentasViewModel()  // Por cada grupo, crea un objeto de tipo IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key, //La clave del grupo, el nombre del tipo cuenta
                    Cuentas = grupo.AsEnumerable() // todas las cuentas dentro de ese grupo
                }).ToList(); //Convierte el resultado en una lista

            return View(modelo);
        }


        //Este metodo crear se ejecuta cuando se le da Click al boton Crear desde el index, permite traer los tipos cuentas
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



        public async Task<IActionResult> Editar(int id)
        {
            var usuarioid = servicioUsuario.GetUsuarioId();
            var cuenta = await repositoryCuentas.GetPorId(id, usuarioid);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            //Mapeo sin Automapper
            /*
             var modelo = new CuentaCreacionViewModel()
            {
                Id = cuenta.Id,
                Nombre = cuenta.Nombre,
                TipoCuentaId = cuenta.TipoCuentaId,
                Descripcion = cuenta.Descripcion,
                Balance = cuenta.Balance
            };
            */


            modelo.TiposCuentas = await GetTiposCuentas(usuarioid);

            return View(modelo);
        }

        [HttpPost]

        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var cuenta = await repositoryCuentas.GetPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = repositoryTiposCuentas.GetPorId(cuentaEditar.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");

        }


        //Este se ejecuta cunado le den clik al boton de delete en la lista de cuentas
        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var cuenta = await repositoryCuentas.GetPorId(id, usuarioId);

            if (cuenta is null)
            {
               return  RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);

        }

        [HttpPost]

        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuario.GetUsuarioId();
            var cuenta = await repositoryCuentas.GetPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositoryCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        private async Task<IEnumerable<SelectListItem>> GetTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositoryTiposCuentas.GetTiposCuentas(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    
        

    
    
    }
}
