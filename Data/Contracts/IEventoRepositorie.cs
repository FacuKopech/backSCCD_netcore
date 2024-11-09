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
        IEnumerable<Persona> ObtenerPersonasQueAsistiranAlEvento(int idEvento);

        IEnumerable<Persona> ObtenerPersonasQueNoAsistiranAlEvento(int idEvento);

        IEnumerable<Persona> ObtenerPersonasQueTalVezAsistanAlEvento(int idEvento);

        IEnumerable<Evento> ObtenerEventosDeInstitucion(int idInstitucion);

        IEnumerable<Evento> ObtenerEventosConAulaDeDocente(int idDocente);

        IEnumerable<Evento> ObtenerEventosParaPadre(int idAula);

    }
}
