namespace Model.Entities
{
    public class Docente : Persona
    {     
        public ICollection<Alumno>? Hijos = new List<Alumno>();
    }
}
