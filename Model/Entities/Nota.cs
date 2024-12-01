using Microsoft.AspNetCore.Http;
using Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class Nota
    {
        [Key]
        
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public Persona Emisor { get; set; }
        public ICollection<NotaPersona> NotaPersonas { get; set; } = new List<NotaPersona>();
        public string Cuerpo { get; set; }
        public TipoNota Tipo { get; set; }
        public Alumno? Referido { get; set; } //Alumno al cual la nota refiere, en caso que sea de tipo Particular
        public ICollection<Aula>? AulasDestinadas { get; set; }
        [NotMapped]
        public ICollection<IFormFile>? Files { get; set; }

    }
}
