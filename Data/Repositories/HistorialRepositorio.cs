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
    public class HistorialRepositorio : IHistorialRepositorie
    {
        private readonly ApplicationDbContext _context;

        public HistorialRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Agregar(Historial entity)
        {
            _context.Historiales.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            var historial = _context.Historiales.Where(x => x.Id == id).FirstOrDefault();
            if (historial != null)
            {
                _context.Historiales.Remove(historial);
                _context.SaveChanges();
            }
        }

        public Task<bool> FirmarHistorial(Historial historial)
        {
            var historialAModificar = _context.Historiales.Where(x => x.Id == historial.Id).FirstOrDefault();
            if (historialAModificar != null)
            {
                historialAModificar.Firmado = true;
                _context.Entry(historialAModificar).State = EntityState.Modified;
                _context.SaveChanges();

                return new Task<bool>(() => true);
            }
            return new Task<bool>(() => false);
        }

        public void Modificar(Historial entity)
        {
            var historial = _context.Historiales.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (historial != null)
            {
                historial.Descripcion = entity.Descripcion;
                historial.Calificacion = entity.Calificacion;
                historial.Estado = entity.Estado;
                _context.Entry(historial).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public Historial ObtenerAsync(Guid id)
        {
            var historial = _context.Historiales.Where(x => x.Id == id).FirstOrDefault();
            if (historial != null)
            {
                return historial;
            }
            
            return null;
        }

        public IEnumerable<Historial> ObtenerTodosAsync()
        {
            return _context.Historiales.ToList();
        }
    }
}
