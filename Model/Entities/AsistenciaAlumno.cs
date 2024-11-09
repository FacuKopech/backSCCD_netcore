namespace Model.Entities
{
    public class AsistenciaAlumno
    {      
        public int AsistenciaId { get; set; }        
        public int AlumnoId { get; set; }        
        public string Estado { get; set; } //presente o ausente
    }
}
