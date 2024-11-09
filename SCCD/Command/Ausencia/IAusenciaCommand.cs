using Microsoft.AspNetCore.Mvc;
using Dtos;

namespace SCCD.Command.Ausencia
{
    public interface IAusenciaCommand
    {
        bool AgregarAusencia(int idHijo, [FromBody] AusenciaModificar nuevaAusencia);
    }
}
