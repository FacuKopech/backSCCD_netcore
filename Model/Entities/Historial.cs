﻿using Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Historial
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public decimal? Calificacion { get; set; }
        public Estado Estado { get; set; } //Aprobado, Entregado, No aprobado, etc.
        public bool Firmado { get; set; }
    }
}
