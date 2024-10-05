using Microsoft.AspNetCore.Mvc;
using Progra2.Data;
using Progra2.Models;

namespace Progra2.Controllers
{
    public class MovimientoController : Controller
    {
        MovimientoData _movimientoData = new MovimientoData();
        EmpleadoData _empleadoData = new EmpleadoData();

        public IActionResult Listar(int id)
        {
            var oLista = _movimientoData.Listar(id);
            EmpleadoModel.GetInstance().Id = id;

            return View(oLista);
            
        }

        public IActionResult Insertar() 
        {
            var tiposMovimientosList = _movimientoData.ListarTiposMovimientos();
            var oModel = new MovimientoModel();
            oModel.Empleado = EmpleadoModel.GetInstance();
            oModel.TipoMovimiento = tiposMovimientosList;
           
            return View(oModel);
        }
      
        [HttpPost]
        public IActionResult Insertar(MovimientoModel oMovimiento)
        {
            //validacion de los campos
            if (!ModelState.IsValid)
            { // funcion propia que sirve para saber si un campo esta vacio, true si todo bien, false si hay algo malo
                return View(ListarMovimientos());
            }
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString(); // Obtiene la IP del usuario
            var resultado = _movimientoData.Insertar(oMovimiento, ipAddress);

            if (resultado == 0)
            {
                TempData["ShowModal"] = true; // Indicador para mostrar el modal
                return RedirectToAction("Listar", new { id = EmpleadoModel.GetInstance().Id });
            }
            else
            {
                ViewBag.ShowErrorModal = true; // Indicador para mostrar el modal
                //return RedirectToAction("Fracaso");
                return View(ListarMovimientos());
            }
        }

        // Funcion privada para no repetir codigo
        private MovimientoModel ListarMovimientos()
        {
            // este otro es para capturar los datos y enviarlo a la base de datos
            var tipoMovimientos = _movimientoData.ListarTiposMovimientos();
            var model = new MovimientoModel
            {
                TipoMovimiento = tipoMovimientos // Llenamos la lista de puestos para el ComboBox
            };
            return model;
        }
    }
}
