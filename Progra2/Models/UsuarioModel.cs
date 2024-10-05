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
        private static UsuarioModel _instance;
        public static UsuarioModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UsuarioModel();
            }
            return _instance;
        }

        public void SetUsuario(UsuarioModel usuario)
        {
            _instance = usuario;
        }
    }
}