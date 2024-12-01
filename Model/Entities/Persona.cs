using System.ComponentModel.DataAnnotations;


namespace Model.Entities
{
    public abstract class Persona
    {
        public Persona()
        {

        }
        [Key]
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Domicilio { get; set; }        
        public Usuario? Usuario { get; set; }
        public Institucion? Institucion { get; set; }
        public ICollection<NotaPersona> NotaPersonas { get; set; } = new List<NotaPersona>();

        public ICollection<EventoPersona> EventosPersona = new List<EventoPersona>();
    }
}
