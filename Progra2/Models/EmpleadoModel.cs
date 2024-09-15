﻿using System.ComponentModel.DataAnnotations;

namespace Progra2.Models
{
    public class EmpleadoModel
    {
        internal decimal Salario;

        public int Id { get; set; }
        public int IdPuesto { get; set; }
        public string? NombrePuesto { get; set; }
        [Required(ErrorMessage = "El campo ValorDocumentoIdentidad es obligatorio")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Solo se permiten caracteres numéricos.")]
        public string? ValorDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\- ]+$", ErrorMessage = "Solo se permiten caracteres alfabéticos")]
        public string? Nombre { get; set; }
        public DateTime FechaContratacion { get; set; }
        public short SaldoVacaciones { get; set; }
        public bool EsActivo { get; set; }
    }
}
