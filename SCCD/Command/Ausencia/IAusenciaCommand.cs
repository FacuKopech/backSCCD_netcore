using Microsoft.AspNetCore.Mvc;
using Dtos;

namespace SCCD.Command.Ausencia
{
    public interface IAusenciaCommand
    {
        bool AgregarAusencia(Guid idHijo, [FromBody] AusenciaModificar nuevaAusencia);
    }
}
