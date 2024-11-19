using Data.Contracts;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class HistorialAuditRepositorio : IHistorialAuditRepositorie
    {
        private readonly ApplicationDbContext _context;

        public HistorialAuditRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Agregar(HistorialAudit entity)
        {
            _context.HistorialAudit.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Modificar(HistorialAudit entity)
        {
            throw new NotImplementedException();
        }

        public HistorialAudit ObtenerAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HistorialAudit> ObtenerTodosAsync()
        {
            return _context.HistorialAudit.ToList();
        }
    }
}
