using Model.Entities;

namespace Model.State
{
    public class DeniedState : IAusenciaState
    {
        private readonly IAusenciaDataLayerRepo _ausenciaRepositorie;

        public DeniedState(IAusenciaDataLayerRepo ausenciaRepositorie)
        {
            _ausenciaRepositorie = ausenciaRepositorie;
        }

        public void AceptarAusencia(Ausencia ausencia)
        {
            throw new InvalidOperationException("No se puede aceptar una Ausencia en Estado Denegado");
        }

        public void DenegarAusencia(Ausencia ausencia)
        {
            _ausenciaRepositorie.DenegarAusencia(ausencia);
        }
    }
}
