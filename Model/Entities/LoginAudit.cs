using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class LoginAudit
    {
        [Key]
        public int Id { get; set; }
        public Usuario UsuarioLogueado { get; set; }
        public DateTime FechaYHoraLogin { get; set; }
        public DateTime FechaYHoraLogout { get; set; }
    }
}
