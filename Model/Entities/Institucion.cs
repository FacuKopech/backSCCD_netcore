using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Institucion
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
    }
}
