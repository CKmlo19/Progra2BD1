using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Progra2.Data;
using Progra2.Models;
using System.Security.Claims;
using System.Net;

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
            
            int esUsuarioValido = _EmpleadoDatos.VerificarUsuario(usuario); // Verificar el usuario en la base de datos 

            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario

            
            if (esUsuarioValido == 0) //Login Exitoso
            { 
                // Reiniciar el contador de intentos fallidos
                HttpContext.Session.SetInt32("LoginAttempts", 0);

                // Crear claims para la autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.username),
                    new Claim(ClaimTypes.Role, "Usuario") 
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                              new ClaimsPrincipal(claimsIdentity));

                // Llamada para registrar la trazabilidad
                
                int resultadoTrazabilidad = _EmpleadoDatos.Trazabilidad(
                    1,                      // 1 Login Exitoso 
                    esUsuarioValido,        // ID del usuario que generó el evento
                    "",                     // Descripción del evento login exitoso
                    ipAddress               
                );

                return RedirectToAction("Listar", "Empleado");
            }
            else //Login No exitoso
            {
                // Llamada para registrar la trazabilidad
                int resultadoTrazabilidad = _EmpleadoDatos.Trazabilidad(
                    2,                     // 2 Login no exitoso 
                    esUsuarioValido,        
                    loginAttempts.ToString(), // cantidad de intentos
                    ipAddress               
                );

                // Incrementar los intentos fallidos
                loginAttempts++;
                HttpContext.Session.SetInt32("LoginAttempts", loginAttempts);

                
                if (loginAttempts >= MaxLoginAttempts) // Si se ha alcanzado el número máximo de intentos, bloquear cuenta
                {
                    TempData["ErrorMessage"] = "Su cuenta ha sido bloqueada debido a demasiados intentos fallidos.";

                    // Llamada para registrar la trazabilidad
                    resultadoTrazabilidad = _EmpleadoDatos.Trazabilidad(
                        3,                     // 3 Login deshabilitado
                        esUsuarioValido,        
                        "", // Descripción del evento login exitoso
                        ipAddress               
                    );
                }
                else
                {
                    TempData["ErrorMessage"] = $"Usuario o contraseña incorrectos. Intento {loginAttempts} de {MaxLoginAttempts}.";
                }

                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> Salir(UsuarioModel usuario)
        {
           
            int esUsuarioValido = _EmpleadoDatos.VerificarUsuario(usuario);
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario

            int resultadoTrazabilidad = _EmpleadoDatos.Trazabilidad(
                4,                     // 4 Logout 
                esUsuarioValido,
                "", // Descripción del evento login exitoso
                ipAddress
            );

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Limpiar la sesión

            return RedirectToAction("Login");
        }
    }
}
