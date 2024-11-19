using Microsoft.AspNetCore.Http;
using Model.State;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities
{
    public class Ausencia
    {
        private IAusenciaState currentState;

        public Ausencia()
        {            
            currentState = new PendingState(this);
        }
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Motivo { get; set; }
        public DateTime FechaComienzo { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaEmision { get; set; }
        public string? Justificada { get; set; }

        public ICollection<Alumno>? HijosConAusencia = new List<Alumno>();

        [NotMapped]
        public ICollection<IFormFile>? Files { get; set; }

        public void SetState(IAusenciaState newState)
        {
            currentState = newState;
        }

        public void AceptarAusencia(Ausencia ausencia)
        {
            currentState.AceptarAusencia(ausencia);
        }

        public void DenegarAusencia(Ausencia ausencia)
        {
            currentState.DenegarAusencia(ausencia);
        }
    }

}
