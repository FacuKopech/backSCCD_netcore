using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                .Include(evento => evento.EventoPersonas)
                .Include(aula => aula.AulaDestinada)
                .Include(creador => creador.Creador)
                .FirstOrDefault();
            if (evento != null)
            {
                _context.Entry(evento).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void ModificarEventoPorConfirmacion(EventoPersona eventoPersona)
        {
            if (eventoPersona != null)
            {
                _context.EventoPersona.Add(eventoPersona);
                _context.SaveChanges();
            }
        }

        public Evento ObtenerAsync(Guid id)
        {
            var evento = _context.Eventos.Where(evento => evento.Id == id)
                .Include(evento => evento.EventoPersonas)
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
                .Include(evento => evento.EventoPersonas)
                .Include(aula => aula.AulaDestinada).ThenInclude(docente => docente.Docente)
                .Where(x => x.AulaDestinada.Docente.Id == idDocente).ToList();
        }

        public IEnumerable<Evento> ObtenerEventosDeInstitucion(Guid idInstitucion)
        {
            return _context.Eventos
                .Include(evento => evento.EventoPersonas)
                .Include(creador => creador.Creador)
                .Include(aula => aula.AulaDestinada)
                .ThenInclude(institucion => institucion.Institucion)
                .Where(x => x.AulaDestinada.Institucion.Id == idInstitucion).ToList();
        }

        public IEnumerable<Evento> ObtenerEventosParaPadre(Guid idAula)
        {
            return _context.Eventos
             .Include(creador => creador.Creador)
             .Include(evento => evento.EventoPersonas)
             .Include(aula => aula.AulaDestinada)
             .Where(x => x.AulaDestinada.Id == idAula).ToList();
        }

        public IEnumerable<Persona> ObtenerPersonasQueAsistiranAlEvento(Guid idEvento)
        {
            var personasQueAsistiran = _context.EventoPersona
                .Where(ep => ep.EventoId == idEvento && ep.Asistira == true)
                .Select(ep => ep.Persona)
                .ToList();

            return personasQueAsistiran;
        }

        public IEnumerable<Persona> ObtenerPersonasQueNoAsistiranAlEvento(Guid idEvento)
        {
            var personasQueNoAsistiran = _context.EventoPersona
               .Where(ep => ep.EventoId == idEvento && ep.NoAsistira == true)
               .Select(ep => ep.Persona)
               .ToList();

            return personasQueNoAsistiran;
        }

        public IEnumerable<Persona> ObtenerPersonasQueTalVezAsistanAlEvento(Guid idEvento)
        {
            var personasQueTalVezAsistan = _context.EventoPersona
               .Where(ep => ep.EventoId == idEvento && ep.TalVezAsista == true)
               .Select(ep => ep.Persona)
               .ToList();

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
