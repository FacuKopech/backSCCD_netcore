using Microsoft.AspNetCore.Http;
using Model.Entities;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class NotaConEnumerables
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public bool? Leida { get; set; }
        public Persona Emisor { get; set; }        
        public IEnumerable<Persona> Destinatarios { get; set; }
        public IEnumerable<Persona> LeidaPor { get; set; }
        public IEnumerable<Persona> FirmadaPor { get; set; }
        public string Cuerpo { get; set; }
        public TipoNota Tipo { get; set; }
        public Alumno? Referido { get; set; } //Alumno al cual la nota refiere, en caso que sea de tipo Particular
        public IEnumerable<Aula>? AulasDestinadas { get; set; }        
        public IEnumerable<IFormFile>? Files { get; set; }
    }
}
