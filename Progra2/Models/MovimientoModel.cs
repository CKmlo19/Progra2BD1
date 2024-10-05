using System.ComponentModel.DataAnnotations;

namespace Progra2.Models
{
    public class MovimientoModel
    {
        public int Id { get; set; }
        public int IdEmpleado { get; set; }
        public int IdTipoMovimiento { get; set; }
        public DateTime Fecha { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "El monto debe ser un valor positivo.")]
        public int Monto { get; set; }
        public int NuevoSaldo { get; set; }
        public string? NombreTipoMovimiento { get; set; }
        public string? NombreUsuario { get; set; }
        public string? PostIP { get; set; }
        public DateTime PostTime { get; set; }
        public EmpleadoModel? Empleado { get; set; } // Esta clase sirve para almacenar el Empleado correspondiente al Movimiento
        public List<TipoMovimientoModel>? TipoMovimiento { get; set; } // Para mostrar los tipos de movimientos para insertar
    }
}
