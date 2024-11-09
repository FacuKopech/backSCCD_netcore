using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.State
{
    public interface IAusenciaDataLayerRepo
    {
        void AceptarAusencia(Ausencia ausencia);
        void DenegarAusencia(Ausencia ausencia);
    }
}
