using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories
{
    public class AsistenciaRepositorio : IAsistenciaRepositorie
    {
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();
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

        public void Borrar(int id)
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

        public Asistencia ObtenerAsistencia(int idAsistencia)
        {
            return _context.AsistenciasTomadas.Where(x => x.Id == idAsistencia).FirstOrDefault();
        }

        public Asistencia ObtenerAsync(int id)
        {
            return _context.AsistenciasTomadas.Where(asistencia => asistencia.Id == id).FirstOrDefault();
        }

        public IEnumerable<Asistencia> ObtenerTodosAsync()
        {
            return _context.AsistenciasTomadas.Include(presentes => presentes.AsistenciaAlumno).ToList();
        }

        public Asistencia ObtenerUltimaAsistenciaAgregada()
        {
            return _context.AsistenciasTomadas.OrderByDescending(x => x.Id).Include(asistenciaAlumno => asistenciaAlumno.AsistenciaAlumno).FirstOrDefault();
        }
    }
}
