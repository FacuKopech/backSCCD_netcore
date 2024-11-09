using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Grupo
    {
        [Key]
        public int Id { get; set; }
        public string Tipo { get; set; }
        public ICollection<Usuario>? Usuarios = new List<Usuario>();
    }
}
