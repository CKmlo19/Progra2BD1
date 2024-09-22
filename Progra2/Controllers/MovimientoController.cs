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
            return View(oLista);
            
        }
    }
}
