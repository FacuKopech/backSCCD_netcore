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
            var notaActualizar = _context.Notas.Where(x => x.Id == nota.Id)
                .Include(lp => lp.LeidaPor)
                .FirstOrDefault();
            if (notaActualizar != null)
            {
                notaActualizar.Leida = true;
                var persona = _context.Personas.Where(x => x.Email == emailLogueado)
                    .Include(u => u.Usuario)
                    .Include(i => i.Institucion)
                    .Include(nr => nr.NotasRecibidas)
                    .Include(nl => nl.NotasLeidas)
                    .FirstOrDefault();
                if (persona != null)
                {
                    notaActualizar.LeidaPor.Add(persona);
                    _context.Entry(notaActualizar).State = EntityState.Modified;
                    //persona.NotasLeidas.Add(notaActualizar);
                    //_context.Entry(persona).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    return new Task<bool>(() => false);
                }

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
            var notaFirmada = _context.Notas.Where(x => x.Id == nota.Id)
                .Include(fp => fp.FirmadaPor)
                .FirstOrDefault();
            if (notaFirmada != null)
            {
                var persona = _context.Personas.Where(x => x.Email == emailLogueado)
                    .Include(u => u.Usuario)
                    .Include(i => i.Institucion)
                    .Include(nr => nr.NotasRecibidas)
                    .Include(nf => nf.NotasFirmadas)
                    .FirstOrDefault();
                if (persona != null)
                {
                    notaFirmada.FirmadaPor.Add(persona);
                    _context.Entry(notaFirmada).State = EntityState.Modified;
                    persona.NotasFirmadas.Add(notaFirmada);
                    _context.Entry(persona).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    return new Task<bool>(() => false);
                }
                return new Task<bool>(() => true);
            }
            return new Task<bool>(() => false);
        }

        public IEnumerable<Nota> GetNotasEmitidasPersona(Guid id)
        {
            var notasEmitidas = _context.Notas.Where(x => x.Id != Guid.Empty)
                .Include(e => e.Emisor)
                .Include(r => r.Referido)
                .Include(a => a.AulasDestinadas)
                .Include(lp => lp.LeidaPor)
                .Include(d => d.Destinatarios);

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

        public IEnumerable<Nota> GetNotasRecibidasPersona(Guid id)
        {
            var persona = _context.Personas.Where(x => x.Id == id)
                .Include(p => p.NotasRecibidas)
                .ThenInclude(e => e.Emisor)
                .Include(p => p.NotasRecibidas)
                .ThenInclude(referido => referido.Referido)
                .Include(notasFirmadas => notasFirmadas.NotasFirmadas)
                .Include(p => p.NotasRecibidas).ThenInclude(aulas => aulas.AulasDestinadas)
                .FirstOrDefault();

            return persona.NotasRecibidas;
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
                .Include(d => d.Destinatarios).ThenInclude(u => u.Usuario).FirstOrDefault();

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
