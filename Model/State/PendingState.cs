using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.State
{
    public class PendingState : IAusenciaState
    {

        public PendingState(Ausencia ausencia)
        {
            if (ausencia.Motivo == "Toma de asistencia - Hijo/a ausente")
            {
                ausencia.Justificada = "No";
            }
            else
            {
                ausencia.Justificada = "";
            }            
        }

        public void AceptarAusencia(Ausencia ausencia)
        {
            throw new InvalidOperationException("No se puede aceptar una Ausencia en Estado Pendiente");
        }

        public void DenegarAusencia(Ausencia ausencia)
        {
            throw new InvalidOperationException("No se puede denegar una Ausencia en Estado Pendiente");
        }
    }
}
