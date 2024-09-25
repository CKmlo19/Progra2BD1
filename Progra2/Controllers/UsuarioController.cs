using Microsoft.AspNetCore.Mvc;
using Progra2.Data;
using Progra2.Models;
using System.Data;
using System.Data.SqlClient;

namespace Progra2.Controllers
{
    public class UsuarioController : Controller
    {
        EmpleadoData _EmpleadoDatos = new EmpleadoData();

        // Acción para mostrar la vista de Login
        public IActionResult Login()
        {
            return View("Login");
        }

        // Acción para manejar el envío del formulario de Login
        [HttpPost]
        public IActionResult VerificarUsuario(string username, string password, string campo)
        {
            var usuarioData = new EmpleadoData(); 
            int resultado = usuarioData.VerificarUsuario(username, password); // método para verificar el usuario

            // Si el usuario es válido, crea el modelo y pásalo a la vista.
            if (resultado == 0) // 0 significa un usuario válido
            {
                if (campo == null)
                {
                    campo = "";
                }
                var oLista = _EmpleadoDatos.Listar(campo);
                return View(oLista);

            }
            else
            {
                TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("Login"); // Redirige a la vista de login
            }
        }
    }
}
