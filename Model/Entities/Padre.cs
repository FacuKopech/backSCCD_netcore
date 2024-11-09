namespace Model.Entities
{
    public class Padre : Persona
    {     
        public ICollection<Alumno> Hijos = new List<Alumno>();
    }
}
