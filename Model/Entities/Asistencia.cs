using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Asistencia
    {
        [Key]
        public int Id { get; set; }
        public DateTime FechaAsistenciaTomada { get; set; }
        public ICollection<AsistenciaAlumno>? AsistenciaAlumno = new List<AsistenciaAlumno>();        
    }
}
