namespace Model.Entities
{
    public class Directivo : Persona
    {
        public ICollection<Alumno>? Hijos = new List<Alumno>();
    }
}
