using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Grupo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Tipo { get; set; }
        public ICollection<Usuario>? Usuarios = new List<Usuario>();
    }
}
