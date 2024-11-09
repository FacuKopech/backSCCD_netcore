using System.ComponentModel.DataAnnotations;


namespace Model.Entities
{
    public class Aula
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? CantidadAlumnos { get; set; }
        public string Grado { get; set; }
        public string Division { get; set; }
        public Institucion Institucion { get; set; }

        public ICollection<Alumno>? Alumnos = new List<Alumno>();
        public Docente? Docente { get; set; }

        public ICollection<Asistencia>? Asistencias = new List<Asistencia>();

        public ICollection<Nota>? NotasParaAula = new List<Nota>();

        public ICollection<Evento>? Eventos = new List<Evento>();

    }
}
