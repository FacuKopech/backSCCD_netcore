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
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();
        public InstitucionRepositorio()
        {
        }
        public void Agregar(Institucion entity)
        {
            _context.Instituciones.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(int id)
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

        public Institucion ObtenerAsync(int id)
        {
            return _context.Instituciones.FirstOrDefault(x => x.Id == id);
                
        }

        public IEnumerable<Institucion> ObtenerTodosAsync()
        {
            return _context.Instituciones.ToList();
        }
    }
}
