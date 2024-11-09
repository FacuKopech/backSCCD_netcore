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
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();
        public void Agregar(Historial entity)
        {
            _context.Historiales.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(int id)
        {
            var historial = _context.Historiales.Where(x => x.IdHistorial == id).FirstOrDefault();
            if (historial != null)
            {
                _context.Historiales.Remove(historial);
                _context.SaveChanges();
            }
        }

        public Task<bool> FirmarHistorial(Historial historial)
        {
            var historialAModificar = _context.Historiales.Where(x => x.IdHistorial == historial.IdHistorial).FirstOrDefault();
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
            var historial = _context.Historiales.Where(x => x.IdHistorial == entity.IdHistorial).FirstOrDefault();
            if (historial != null)
            {
                historial.Descripcion = entity.Descripcion;
                historial.Calificacion = entity.Calificacion;
                historial.Estado = entity.Estado;
                _context.Entry(historial).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public Historial ObtenerAsync(int id)
        {
            return _context.Historiales.Where(x => x.IdHistorial == id).FirstOrDefault();
        }

        public IEnumerable<Historial> ObtenerTodosAsync()
        {
            return _context.Historiales.ToList();
        }
    }
}
