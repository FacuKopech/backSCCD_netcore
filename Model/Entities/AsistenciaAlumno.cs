namespace Model.Entities
{
    public class AsistenciaAlumno
    {      
        public Guid AsistenciaId { get; set; }        
        public Guid AlumnoId { get; set; }        
        public string Estado { get; set; } //presente o ausente
    }
}
