using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Progra2.Data;
using Progra2.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;

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
        public async Task<IActionResult> Login(UsuarioModel usuario)
        {
            int resultado = _EmpleadoDatos.VerificarUsuario(usuario); // método para verificar el usuario

            // Si el usuario es válido, crea el modelo y pásalo a la vista.
            if (resultado == 0) // 0 significa un usuario válido
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, usuario.username),
                    };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Listar", "Empleado");

            }
            else
            {
                TempData["ErrorMessage"] = "Usuario o contraseña incorrectos.";
                return RedirectToAction("Login"); // Redirige a la vista de login
            }
        }
        public async Task<IActionResult> Salir()
        {
            // se elimina la cookie al salir
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return View("Login");
        }
    }
}
