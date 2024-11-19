using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class LoginAudit
    {
        [Key]
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public Usuario UsuarioLogueado { get; set; }
        public DateTime FechaYHoraLogin { get; set; }
        public DateTime FechaYHoraLogout { get; set; }
    }
}
