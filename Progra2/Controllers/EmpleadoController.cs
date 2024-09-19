﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Progra2.Data;
using Progra2.Models;
using System.Text.RegularExpressions;

namespace Progra2.Controllers
{
    public class EmpleadoController : Controller
    {
        EmpleadoData _EmpleadoDatos = new EmpleadoData();

        public IActionResult Listar()
        {
            // la lista mostrara una lista de empleados
            var oLista = _EmpleadoDatos.Listar(""); // llama al metodo de listar y lo muestra
            return View(oLista);
        }
        [HttpPost]
        public IActionResult Listar(string campo)
        {
            if (campo == null) {
                campo = "";
            }
            var oLista = _EmpleadoDatos.Listar(campo);
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

            var resultado = _EmpleadoDatos.Editar(oEmpleado);

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
