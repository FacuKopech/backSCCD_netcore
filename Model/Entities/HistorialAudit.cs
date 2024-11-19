using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class HistorialAudit
    {
        [Key]
        
        public Guid Id { get; set; } = Guid.NewGuid();
        [ForeignKey("IdPersona")]
        public Guid IdPersona { get; set; }
        [ForeignKey("IdHistorial")]
        public Guid IdHistorial { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
