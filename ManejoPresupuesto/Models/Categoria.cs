using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="El campor {0} es requerido")]
        [StringLength(maximumLength:50, ErrorMessage = "No puede ser mayor a {1} caracteres")]
        public string Nombre { get; set; }

        [DisplayName("Tipo de Operacion")]
        public TipoOperacion TipoOperacionId { get; set; }

        public int UsuarioId { get; set; }
    }
}
