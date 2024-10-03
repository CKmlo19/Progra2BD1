using System.ComponentModel.DataAnnotations;
namespace Progra2.Models
{
    public class UsuarioModel
    {
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public string? username { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        public string? password { get; set; }
        public string? PostIP { get; set; }
    }
}