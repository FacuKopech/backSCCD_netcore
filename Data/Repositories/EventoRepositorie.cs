using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class EventoRepositorie : IEventoRepositorie
    {
        private readonly ApplicationDbContext _context;

        public EventoRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Agregar(Evento entity)
        {
            _context.Eventos.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            var evento = _context.Eventos.FirstOrDefault(evento => evento.Id == id);
            if (evento != null)
            {
                _context.Eventos.Remove(evento);
                _context.SaveChanges();
            }
        }

        public void Modificar(Evento entity)
        {
            var evento = _context.Eventos.Where(x => x.Id == entity.Id)
                .Include(evento => evento.Asistiran)
                .Include(evento => evento.NoAsistiran)
                .Include(evento => evento.TalVezAsistan)
                .Include(aula => aula.AulaDestinada)
                .Include(creador => creador.Creador)
                .FirstOrDefault();
            if (evento != null)
            {
                _context.Entry(evento).State = EntityState.Modified;
                _context.SaveChanges();

            }
        }

        public Evento ObtenerAsync(Guid id)
        {
            var evento = _context.Eventos.Where(evento => evento.Id == id)
                .Include(evento => evento.Asistiran)
                .Include(evento => evento.NoAsistiran)
                .Include(evento => evento.TalVezAsistan)
                .Include(aula => aula.AulaDestinada)
                .Include(creador => creador.Creador).FirstOrDefault();

            if (evento != null)
            {
                return evento;
            }
            return null;
        }

        public IEnumerable<Evento> ObtenerEventosConAulaDeDocente(Guid idDocente)
        {
            return _context.Eventos
                .Include(creador => creador.Creador)
                .Include(evento => evento.Asistiran)
                .Include(evento => evento.NoAsistiran)
                .Include(evento => evento.TalVezAsistan)
                .Include(aula => aula.AulaDestinada).ThenInclude(docente => docente.Docente)
                .Where(x => x.AulaDestinada.Docente.Id == idDocente).ToList();
        }

        public IEnumerable<Evento> ObtenerEventosDeInstitucion(Guid idInstitucion)
        {
            return _context.Eventos
                .Include(evento => evento.Asistiran)
                .Include(evento => evento.NoAsistiran)
                .Include(evento => evento.TalVezAsistan)
                .Include(creador => creador.Creador)
                .Include(aula => aula.AulaDestinada)
                .ThenInclude(institucion => institucion.Institucion)
                .Where(x => x.AulaDestinada.Institucion.Id == idInstitucion).ToList();
        }

        public IEnumerable<Evento> ObtenerEventosParaPadre(Guid idAula)
        {
            return _context.Eventos
             .Include(creador => creador.Creador)
             .Include(evento => evento.Asistiran)
             .Include(evento => evento.NoAsistiran)
             .Include(evento => evento.TalVezAsistan)
             .Include(aula => aula.AulaDestinada)
             .Where(x => x.AulaDestinada.Id == idAula).ToList();
        }

        public IEnumerable<Persona> ObtenerPersonasQueAsistiranAlEvento(Guid idEvento)
        {
            var personas = _context.Personas
                .Include(persona => persona.EventosAsistire)
                .ToList();
            List<Persona> personasQueAsistiran = new List<Persona>();
            foreach (var persona in personas)
            {
                if (persona.EventosAsistire.FirstOrDefault(evento => evento.Id == idEvento) != null)
                {
                    personasQueAsistiran.Add(persona);
                }
            }
            return personasQueAsistiran;
        }

        public IEnumerable<Persona> ObtenerPersonasQueNoAsistiranAlEvento(Guid idEvento)
        {
            var personas = _context.Personas
                .Include(persona => persona.EventosNoAsistire)
                .ToList();
            List<Persona> personasQueNoAsistiran = new List<Persona>();
            foreach (var persona in personas)
            {
                if (persona.EventosNoAsistire.FirstOrDefault(evento => evento.Id == idEvento) != null)
                {
                    personasQueNoAsistiran.Add(persona);
                }
            }
            return personasQueNoAsistiran;
        }

        public IEnumerable<Persona> ObtenerPersonasQueTalVezAsistanAlEvento(Guid idEvento)
        {
            var personas = _context.Personas
                .Include(persona => persona.EventosTalVezAsista)
                .ToList();
            List<Persona> personasQueTalVezAsistan = new List<Persona>();
            foreach (var persona in personas)
            {
                if (persona.EventosTalVezAsista.FirstOrDefault(evento => evento.Id == idEvento) != null)
                {
                    personasQueTalVezAsistan.Add(persona);
                }
            }
            return personasQueTalVezAsistan;
        }

        public IEnumerable<Evento> ObtenerTodosAsync()
        {
            return _context.Eventos
                .Include(c => c.Creador)
                .Include(a => a.AulaDestinada)
                .ToList();
        }
    }
}
