using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories
{
    public class NotaRepositorio : INotaRepositorie
    {
        private readonly ApplicationDbContext _context;

        public NotaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> ActualizarNotaLeida(Nota nota, string emailLogueado)
        {
            var notaActualizar = _context.NotaPersona.Where(x => x.NotaId == nota.Id && x.Persona.Email == emailLogueado)
                .FirstOrDefault();

            if (notaActualizar != null)
            {
                notaActualizar.Leida = true;
                notaActualizar.FechaLectura = DateTime.Now;
                _context.Entry(notaActualizar).State = EntityState.Modified;
                _context.SaveChanges();
                
                return new Task<bool>(() => true);
            }
            return new Task<bool>(() => false);
        }

        public void Agregar(Nota entity)
        {
            var existingEntity = _context.ChangeTracker.Entries<Nota>()
                .FirstOrDefault(e => e.Entity.Id == entity.Id);

            if (existingEntity != null)
            {
                _context.Entry(existingEntity.Entity).State = EntityState.Detached;
            }

            _context.Notas.Add(entity);
            _context.SaveChanges();
        }


        public void Borrar(Guid id)
        {
            var nota = _context.Notas.Where(x => x.Id == id)
                .FirstOrDefault();
            if (nota != null)
            {
                _context.Notas.Remove(nota);
                _context.SaveChanges();                
            }            
        }

        public Task<bool> FirmaDeNota(Nota nota, string emailLogueado)
        {
            var notaFirmada = _context.NotaPersona.Where(x => x.NotaId == nota.Id && x.Persona.Email == emailLogueado)
                .FirstOrDefault();

            if (notaFirmada != null)
            {
                notaFirmada.Firmada = true;
                notaFirmada.FechaFirma = DateTime.Now;
                _context.Entry(notaFirmada).State = EntityState.Modified;
                _context.SaveChanges();
                return new Task<bool>(() => true);
            }
            return new Task<bool>(() => false);
        }

        public IEnumerable<Nota> GetNotasEmitidasPersona(Guid id)
        {
            var notasEmitidas = _context.Notas
            .Where(x => x.Id != Guid.Empty)
            .Include(e => e.Emisor)
            .Include(r => r.Referido)
            .Include(a => a.AulasDestinadas)
            .Include(np => np.NotaPersonas).ThenInclude(npP => npP.Persona)
            .ToList();


            List<Nota> notas = new List<Nota>();
            foreach (var nota in notasEmitidas)
            {
                if (nota.Emisor != null)
                {
                    if (nota.Emisor.Id == id)
                    {
                        notas.Add(nota);
                    }
                }
            }
                
            return notas;
        }

        public (IEnumerable<Nota> NotasRecibidas, IEnumerable<Nota> NotasFirmadas) GetNotasRecibidasYFirmadas(Guid id)
        {
            var persona = _context.Personas
                .Where(x => x.Id == id)
                .Include(p => p.NotaPersonas)
                    .ThenInclude(np => np.Nota)
                        .ThenInclude(n => n.Emisor)
                .Include(p => p.NotaPersonas)
                    .ThenInclude(np => np.Nota)
                        .ThenInclude(n => n.Referido)
                .Include(p => p.NotaPersonas)
                    .ThenInclude(np => np.Nota)
                        .ThenInclude(n => n.AulasDestinadas)
                .FirstOrDefault();

            if (persona == null)
            {
                return (Enumerable.Empty<Nota>(), Enumerable.Empty<Nota>());
            }

            var notasRecibidas = persona.NotaPersonas
                .Select(np => np.Nota)
                .Distinct(); 

            var notasFirmadas = persona.NotaPersonas
                .Where(np => np.Firmada == true)
                .Select(np => np.Nota)
                .Distinct(); 

            return (notasRecibidas, notasFirmadas);
        }


        public void Modificar(Nota entity)
        {
            var nota = _context.Notas.Where(x => x.Id == entity.Id).Include(a => a.AulasDestinadas).FirstOrDefault();
            if (nota != null)
            {
                _context.Entry(nota).State = EntityState.Modified;
                _context.SaveChanges();

            }
        }

        public Nota ObtenerAsync(Guid id)
        {
            var nota = _context.Notas.OfType<Nota>().Where(x => x.Id == id)
                .Include(e => e.Emisor)
                .Include(r => r.Referido)
                .Include(a => a.AulasDestinadas).ThenInclude(aula => aula.Docente)
                .Include(d => d.NotaPersonas).ThenInclude(u => u.Persona.Usuario).FirstOrDefault();

            if (nota != null)
            {
                return nota;
            }

            return null;
        }

        public IEnumerable<Nota> ObtenerTodosAsync()
        {
            return _context.Notas;
        }

        public Nota ObtenerUltimaNotaAgregada()
        {
            return _context.Notas.OrderByDescending(x => x.Id).FirstOrDefault();
        }
    }
}
