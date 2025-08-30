using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepostiroyTiposCuentas repostiroyTiposCuentas;
        private readonly IServicioUsuario servicioUsuarios;

        //Constructor
        public TiposCuentasController(IRepostiroyTiposCuentas repostiroyTiposCuentas, IServicioUsuario servicioUsuarios)
        {
            this.repostiroyTiposCuentas = repostiroyTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            //connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.GetUsuarioId();
            var tiposCuentas = await repostiroyTiposCuentas.GetTiposCuentas(usuarioId);
            return View(tiposCuentas);
        }

        public IActionResult Crear()
        {
            //No es buena practica colocar el codigo de acceso a datos en el controlador
            //using (var connection = new SqlConnection(connectionString))
            //{
            //    var query = connection.Query("SELECT 1").FirstOrDefault();
            //}
            //var modelo = new TipoCuenta(){ Nombre = "Angel"};
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuarios.GetUsuarioId();

            var alreadyExist = await repostiroyTiposCuentas.Exist(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (alreadyExist) {

                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre de Tipo Cuenta {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await repostiroyTiposCuentas.Create(tipoCuenta);
            
            return RedirectToAction("Index");
            
        }


        //Se ejecuta cuando se le da en el boton editar de la vista
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            Console.WriteLine("Editar desde la vista...");
            var usuarioId = servicioUsuarios.GetUsuarioId();
            var tipoCuenta = await repostiroyTiposCuentas.GetPorId(id, usuarioId);
           

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);

        }


        //Se ejecuta cuando se le da click al boton de guardar en el formulario
        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            Console.WriteLine("Editar desde el formulario...");
            var usuarioId = servicioUsuarios.GetUsuarioId();
            var tipoCuentaExiste = await repostiroyTiposCuentas.GetPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repostiroyTiposCuentas.UpdateTipoCuenta(tipoCuenta);
            return RedirectToAction("Index");
        }


        //Borrar

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.GetUsuarioId();
            var tipoCuenta = await repostiroyTiposCuentas.GetPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.GetUsuarioId();
            var tipoCuenta = await repostiroyTiposCuentas.GetPorId(id, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");

            }
            await repostiroyTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        //Metodo para validar si existe Tipos cuentas

        [HttpGet]
        public async Task<IActionResult> ValidateTipoCuentaExistente(string Nombre)
        {

            var usuarioId = servicioUsuarios.GetUsuarioId();

            var existTipoCuenta = await repostiroyTiposCuentas.Exist(Nombre, usuarioId);

            if (existTipoCuenta)
            {
                return Json($"El nombre {Nombre} ya existe en Tipos Cuenta");
            }

            return Json(true);

        }


        //Accion cuando se arrastra una fila de la tabla vista tipos cuentas

        [HttpPost]

        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.GetUsuarioId();
            var tipoCuenta = await repostiroyTiposCuentas.GetTiposCuentas(usuarioId);
            var idsTiposCuentas = tipoCuenta.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repostiroyTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();

        }

    }
}
