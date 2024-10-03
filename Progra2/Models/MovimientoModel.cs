using System.ComponentModel.DataAnnotations;

namespace Progra2.Models
{
    public class MovimientoModel
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int IdTipoMovimieto { get; set; }
        public int Monto { get; set; }
        public string? NombreTipoMovimiento { get; set; }
        public string? PostIP { get; set; }
        public DateTime PostTime { get; set; }
        public EmpleadoModel? Empleado { get; set; } // Esta clase sirve para almacenar el Empleado correspondiente al Movimiento
        public List<TipoMovimientoModel>? TipoMovimieto { get;} // Para mostrar los tipos de movimientos para insertar
    }
}
