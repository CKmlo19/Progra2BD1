using Microsoft.AspNetCore.Mvc;
using Progra2.Models;
using Progra2.Data;

namespace Progra2.Controllers
{
    public class EmpleadoController : Controller
    {
        EmpleadoDatos _EmpleadoDatos = new EmpleadoDatos();

        public IActionResult Listar()
        {
            // la lista mostrara una lista de empleados
            var oLista = _EmpleadoDatos.Listar(); // llama al metodo de listar y lo muestra
            return View(oLista);
        }
        public IActionResult Insertar()
        {
            // muestra el formulario para insertar
            return View();
        }
        [HttpPost]
        public IActionResult Insertar(EmpleadoModel oEmpleado)
        {
            // este otro es para capturar los datos y enviarlo a la base de datos

            //validacion de los campos
            if (!ModelState.IsValid)
            { // funcion propia que sirve para saber si un campo esta vacio, true si todo bien, false si hay algo malo
                return View();
            }

            var resultado = _EmpleadoDatos.Insertar(oEmpleado);

            if (resultado == 0)
            {
                TempData["ShowModal"] = true; // Indicador para mostrar el modal
                return RedirectToAction("Listar");
            }
            else
            {
                ViewBag.ShowErrorModal = true; // Indicador para mostrar el modal
                //return RedirectToAction("Fracaso");
                return View();
            }
        }
    }
}
