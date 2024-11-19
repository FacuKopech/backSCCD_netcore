using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Institucion
    {
        [Key]
       
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
    }
}
