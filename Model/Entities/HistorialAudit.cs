using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class HistorialAudit
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Persona")]
        public int IdPersona { get; set; }
        [ForeignKey("Historial")]
        public int IdHistorial { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
