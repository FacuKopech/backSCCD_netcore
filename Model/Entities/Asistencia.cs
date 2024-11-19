using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Asistencia
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime FechaAsistenciaTomada { get; set; }
        public ICollection<AsistenciaAlumno>? AsistenciaAlumno = new List<AsistenciaAlumno>();        
    }
}
