using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Progra2.Data;
using Progra2.Models;
using System.Security.Claims;

namespace Progra2.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly EmpleadoData _EmpleadoDatos;
        private const int MaxLoginAttempts = 3; // Número máximo de intentos

        public UsuarioController(EmpleadoData empleadoDatos)
        {
            _EmpleadoDatos = empleadoDatos; // Usa inyección de dependencias si es posible
        }

        // Acción para mostrar la vista de Login
        public IActionResult Login()
        {
            int? loginAttempts = HttpContext.Session.GetInt32("LoginAttempts");

            // Verificar si el usuario está bloqueado
            if (loginAttempts.HasValue && loginAttempts.Value >= MaxLoginAttempts)
            {
                TempData["ErrorMessage"] = "Su cuenta ha sido bloqueada debido a demasiados intentos fallidos.";
                return View();
            }

            return View("Login");
        }

        // Acción para manejar el envío del formulario de Login
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioModel usuario)
        {
            int loginAttempts = HttpContext.Session.GetInt32("LoginAttempts") ?? 0;

            // Si excede los intentos permitidos
            if (loginAttempts >= MaxLoginAttempts)
            {
                TempData["ErrorMessage"] = "Su cuenta está bloqueada.";
                return RedirectToAction("Login");
            }

            // Verificar el usuario en la base de datos (mejora el método para optimizar rendimiento)
            int esUsuarioValido = _EmpleadoDatos.VerificarUsuario(usuario);
           
            if (esUsuarioValido == 0)
            {
                // Reiniciar el contador de intentos fallidos
                HttpContext.Session.SetInt32("LoginAttempts", 0);

                // Crear claims para la autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.username),
                    new Claim(ClaimTypes.Role, "Usuario") // Puedes agregar más claims según sea necesario
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Listar", "Empleado");
            }
            else
            {
                // Incrementar los intentos fallidos
                loginAttempts++;
                HttpContext.Session.SetInt32("LoginAttempts", loginAttempts);

                // Si se ha alcanzado el número máximo de intentos, bloquear cuenta
                if (loginAttempts >= MaxLoginAttempts)
                {
                    TempData["ErrorMessage"] = "Su cuenta ha sido bloqueada debido a demasiados intentos fallidos.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Usuario o contraseña incorrectos. Intento {loginAttempts} de {MaxLoginAttempts}.";
                }

                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Limpiar la sesión
            return RedirectToAction("Login");
        }
    }
}
