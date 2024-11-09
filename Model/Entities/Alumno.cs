namespace Model.Entities
{
    public class Alumno : Persona
    {
        public DateTime FechaNacimiento { get; set; }

        public ICollection<Historial>? Historiales = new List<Historial>();
        public ICollection<Ausencia>? Ausencias = new List<Ausencia>();   //Ausencias cargadas por parte del padre
        public ICollection<AsistenciaAlumno>? Asistencias = new List<AsistenciaAlumno>(); //Asistencias tomadas por parte del docente               
        public decimal Asistencia { get; set; }
    }
}
