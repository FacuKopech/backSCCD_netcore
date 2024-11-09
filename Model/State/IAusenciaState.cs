using Model.Entities;

namespace Model.State
{
    public interface IAusenciaState
    {
        void AceptarAusencia(Ausencia ausencia);
        void DenegarAusencia(Ausencia ausencia);
    }
}
