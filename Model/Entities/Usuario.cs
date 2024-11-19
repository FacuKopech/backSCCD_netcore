using System.ComponentModel.DataAnnotations;


namespace Model.Entities
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public ICollection<Grupo> Grupos = new List<Grupo>();
        public string Email { get; set; }
        public string Username { get; set; }
        public string Clave { get; set; }       
    }
}
