using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IrepositoryCategorias repositoryCategorias;
        private readonly IServicioUsuario servicioUsuario;

        public CategoriasController(IrepositoryCategorias repositoryCategorias, IServicioUsuario servicioUsuario)
        {
            this.repositoryCategorias = repositoryCategorias;
            this.servicioUsuario = servicioUsuario;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioid = servicioUsuario.GetUsuarioId();
            var categorias = await repositoryCategorias.GetCategorias(usuarioid);
            return View(categorias);
        }

        [HttpGet]
        public IActionResult Crear() {
        
            return View();
        }

        public async Task<IActionResult> Crear(Categoria categoria)
        {
           

            if (!ModelState.IsValid) {

                return View(categoria);     
                
            }

            var usuarioId = servicioUsuario.GetUsuarioId();
            categoria.UsuarioId = usuarioId;
            await repositoryCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }
    }
}
