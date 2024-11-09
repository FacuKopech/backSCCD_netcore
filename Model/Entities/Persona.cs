using System.ComponentModel.DataAnnotations;


namespace Model.Entities
{
    public abstract class Persona
    {
        public Persona()
        {

        }
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Domicilio { get; set; }        
        public Usuario? Usuario { get; set; }
        public Institucion? Institucion { get; set; }

        public ICollection<Nota> NotasRecibidas = new List<Nota>();

        public ICollection<Nota> NotasLeidas = new List<Nota>();

        public ICollection<Nota> NotasFirmadas = new List<Nota>();

        public ICollection<Evento> EventosAsistire = new List<Evento>();

        public ICollection<Evento> EventosNoAsistire = new List<Evento>();

        public ICollection<Evento> EventosTalVezAsista = new List<Evento>();

    }
}
