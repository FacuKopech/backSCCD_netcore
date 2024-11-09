using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{
    public class Evento
    {
        public Evento()
        {

        }
        [Key]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }                
        public string Localidad { get; set; }
        public string Motivo { get; set; }
        public string Descripcion { get; set; }
        public Aula AulaDestinada { get; set; }
        public Persona Creador { get; set; }

        public ICollection<Persona>? Asistiran = new List<Persona>();
        public ICollection<Persona>? NoAsistiran = new List<Persona>();
        public ICollection<Persona>? TalVezAsistan = new List<Persona>();
    }
}
