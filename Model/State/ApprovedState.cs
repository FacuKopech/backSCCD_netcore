using Model.Entities;

namespace Model.State
{
    public class ApprovedState : IAusenciaState
    {
        private readonly IAusenciaDataLayerRepo _ausenciaRepositorie;

        public ApprovedState(IAusenciaDataLayerRepo ausenciaRepositorie)
        {
            _ausenciaRepositorie = ausenciaRepositorie;
        }

        public void AceptarAusencia(Ausencia ausencia)
        {
            _ausenciaRepositorie.AceptarAusencia(ausencia);
        }

        public void DenegarAusencia(Ausencia ausencia)
        {
            throw new InvalidOperationException("No se puede denegar una Ausencia en Estado Aprobado");
        }
    }
}
