using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Data.Contracts
{
    public interface INotaRepositorie : IGenericRepositorie<Nota>
    {
        (IEnumerable<Nota> NotasRecibidas, IEnumerable<Nota> NotasFirmadas) GetNotasRecibidasYFirmadas(Guid id);
        IEnumerable<Nota> GetNotasEmitidasPersona(Guid id);        
        Task<bool> ActualizarNotaLeida(Nota nota, string emailLogueado);
        Task<bool> FirmaDeNota(Nota nota, string emailLogueado);
        Nota ObtenerUltimaNotaAgregada();
    }
}
