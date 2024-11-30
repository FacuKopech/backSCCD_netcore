using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using System.Linq;

namespace Data.Repositories
{
    public class AsistenciaRepositorio : IAsistenciaRepositorie
    {
        private readonly ApplicationDbContext _context;

        public AsistenciaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Agregar(Asistencia entity)
        {
            _context.AsistenciasTomadas.Add(entity);
            _context.SaveChanges();
        }

        public void AgregarAsistenciaAlumno(AsistenciaAlumno nuevaAsistenciaAlumno)
        {
            _context.AsistenciaAlumno.Add(nuevaAsistenciaAlumno);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Modificar(Asistencia asistenciaAModificar)
        {
            var asistencia = _context.AsistenciasTomadas.Where(x => x.Id == asistenciaAModificar.Id).FirstOrDefault();
            if (asistencia != null)
            {
                _context.Entry(asistenciaAModificar).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public Asistencia ObtenerAsistencia(Guid idAsistencia)
        {
            return _context.AsistenciasTomadas.Where(x => x.Id == idAsistencia).FirstOrDefault();
        }

        public Asistencia ObtenerAsync(Guid id)
        {
            var asistencia = _context.AsistenciasTomadas.Where(asistencia => asistencia.Id == id).FirstOrDefault();
            if (asistencia != null)
            {
                return asistencia;
            }

            return null;
        }

        public IEnumerable<Asistencia> ObtenerTodosAsync()
        {
            return _context.AsistenciasTomadas.Include(presentes => presentes.AsistenciaAlumno).ToList();
        }

        public Asistencia ObtenerUltimaAsistenciaAgregada()
        {
            var today = DateTime.Today; 
            return _context.AsistenciasTomadas
                .Where(x => x.FechaAsistenciaTomada.Date == today)
                .Include(asistenciaAlumno => asistenciaAlumno.AsistenciaAlumno)
                .OrderByDescending(x => x.FechaAsistenciaTomada)
                .FirstOrDefault();
        }

    }
}
