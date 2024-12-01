using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IEventoRepositorie : IGenericRepositorie<Evento>
    {
        IEnumerable<Persona> ObtenerPersonasQueAsistiranAlEvento(Guid idEvento);

        IEnumerable<Persona> ObtenerPersonasQueNoAsistiranAlEvento(Guid idEvento);

        IEnumerable<Persona> ObtenerPersonasQueTalVezAsistanAlEvento(Guid idEvento);

        IEnumerable<Evento> ObtenerEventosDeInstitucion(Guid idInstitucion);

        IEnumerable<Evento> ObtenerEventosConAulaDeDocente(Guid idDocente);

        IEnumerable<Evento> ObtenerEventosParaPadre(Guid idAula);

        void ModificarEventoPorConfirmacion(EventoPersona eventoPersona);
    }
}
