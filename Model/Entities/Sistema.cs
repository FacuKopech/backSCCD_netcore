namespace Model.Entities
{
    public class Sistema
    {
        public ICollection<Persona> Personas { get; set; }
        public ICollection<Aula> Aulas { get; set; }
        public ICollection<Institucion> Instituciones { get; set; }

    }
}
