using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Progra2.Data;
using Progra2.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Progra2.Controllers
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        EmpleadoData _EmpleadoDatos = new EmpleadoData();


        public IActionResult Listar()
        {
            // la lista mostrara una lista de empleados
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario
            var oLista = _EmpleadoDatos.Listar("", UsuarioModel.GetInstance().id, ipAddress); // llama al metodo de listar y lo muestra
            return View(oLista);
        }
        [HttpPost]
        public IActionResult Listar(string campo)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario

            if (campo == null) {
                campo = "";
            }
            var oLista = _EmpleadoDatos.Listar(campo, UsuarioModel.GetInstance().id, ipAddress);
            return View(oLista);
        }
        public IActionResult Insertar()
        {
            // muestra el formulario para insertar
            var puestos = _EmpleadoDatos.ListarPuesto();
            var model = new EmpleadoModel
            {
                Puestos = puestos // Llenamos la lista de puestos para el ComboBox
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Insertar(EmpleadoModel oEmpleado)
        {

            //validacion de los campos
            if (!ModelState.IsValid)
            { // funcion propia que sirve para saber si un campo esta vacio, true si todo bien, false si hay algo malo
                return View(listarPuestos());
            }
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario
            var resultado = _EmpleadoDatos.Insertar(oEmpleado, UsuarioModel.GetInstance().id, ipAddress);

            if (resultado == 0)
            {
                TempData["ShowModal"] = true; // Indicador para mostrar el modal
                return RedirectToAction("Listar");
            }
            else
            {
                TempData["ModalMessage"] = "Error al intentar insertar el empleado.";
                return View(listarPuestos());
            }
        }

        // Funcion para editar los empleados
        public IActionResult Editar(int Id)
        {
            // muestra el formulario para editar
            var model = _EmpleadoDatos.Obtener(Id);
            model.Puestos = _EmpleadoDatos.ListarPuesto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Editar(EmpleadoModel oEmpleado)
        {
            

            //validacion de los campos
            if (!ModelState.IsValid)
            { // funcion propia que sirve para saber si un campo esta vacio, true si todo bien, false si hay algo malo
                return View(listarPuestos());
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario
            var resultado = _EmpleadoDatos.Editar(oEmpleado, UsuarioModel.GetInstance().id, ipAddress);

            if (resultado == 0)
            {
                TempData["ModalMessage"] = "Error al intentar editar el empleado.";
                return RedirectToAction("Listar");
            }
            else
            {
                ViewBag.ShowErrorModal = true; // Indicador para mostrar el modal
                //return RedirectToAction("Fracaso");
                return View(listarPuestos());
            }
        }

        // Funcion para eliminar los empleados
        public IActionResult Eliminar(int Id)
        {
            // muestra el formulario para editar
            var model = _EmpleadoDatos.Obtener(Id);
            model.Puestos = _EmpleadoDatos.ListarPuesto();
            return View(model);
        }

        [HttpPost]
        public IActionResult Eliminar(EmpleadoModel oEmpleado)
        { 

            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario
            var resultado = _EmpleadoDatos.Eliminar(oEmpleado.Id, UsuarioModel.GetInstance().id, ipAddress, 1);

            if (resultado == 0)
            {
                TempData["ShowModal"] = true; // Indicador para mostrar el modal
                return RedirectToAction("Listar");
            }
            else
            {
                ViewBag.ShowErrorModal = true; // Indicador para mostrar el modal
                //return RedirectToAction("Fracaso");
                return View(listarPuestos());
            }
        }

        // Funcion privada para no repetir codigo
        private EmpleadoModel listarPuestos()
        {
            // este otro es para capturar los datos y enviarlo a la base de datos
            var puestos = _EmpleadoDatos.ListarPuesto();
            var model = new EmpleadoModel
            {
                Puestos = puestos // Llenamos la lista de puestos para el ComboBox
            };
            return model;
        }
    }
}
