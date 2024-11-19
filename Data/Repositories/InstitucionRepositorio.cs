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
    public class InstitucionRepositorio : IInstitucionRepositorie
    {
        private readonly ApplicationDbContext _context;

        public InstitucionRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public InstitucionRepositorio()
        {
        }
        public void Agregar(Institucion entity)
        {
            _context.Instituciones.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            var institucion = _context.Instituciones.Where(x => x.Id == id).FirstOrDefault();
            _context.Instituciones.Remove(institucion);
            _context.SaveChanges();
        }

        public void Modificar(Institucion entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Institucion ObtenerAsync(Guid id)
        {
            var institucion = _context.Instituciones.Where(x => x.Id == id).FirstOrDefault();

            if (institucion != null)
            {
                return institucion;
            }
            return null;
        }

        public IEnumerable<Institucion> ObtenerTodosAsync()
        {
            return _context.Instituciones.ToList();
        }
    }
}
