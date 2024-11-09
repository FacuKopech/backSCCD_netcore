using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories
{
    public class AulaRepositorio : IAulaRepositorie
    {
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();
        private readonly IPersonaRepositorie _personaRepositorie;
        public AulaRepositorio(IPersonaRepositorie personaRepositorie)
        {
            _personaRepositorie = personaRepositorie;
        }
        public void Agregar(Aula entity)
        {
            _context.Aulas.Add(entity);
            //_context.Entry(entity.Institucion).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void AgregarAlumnosAAula(int idAula, Persona alumno)
        {
            var aula = _context.Aulas.Where(x => x.Id == idAula)
                .FirstOrDefault();
            if (aula != null)
            {
                aula.Alumnos.Add((Alumno)alumno);
                aula.CantidadAlumnos += aula.CantidadAlumnos + 1;
                _context.Entry(aula).State = EntityState.Modified;
                _context.SaveChanges();

            }
        }

        public void AsignarDocenteAAula(int idAula, Persona docente)
        {
            var aula = _context.Aulas.Where(x => x.Id == idAula).FirstOrDefault();
            if (aula != null)
            {
                aula.Docente = (Docente)docente;
                _context.Entry(aula).State = EntityState.Modified;
                _context.SaveChanges();
                
            }
        }

        public void Borrar(int id)
        {
            var aulaAEliminar = this.ObtenerAsync(id);
            if (aulaAEliminar != null)
            {
                _context.Aulas.Remove(aulaAEliminar);
                _context.Entry(aulaAEliminar).State = EntityState.Deleted;
                _context.SaveChanges();                
            }
        }

        public void Modificar(Aula entity)
        {
            var aula = _context.Aulas.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (aula != null)
            {
                _context.Entry(aula).State = EntityState.Modified;
                _context.SaveChanges();

            }            
        }

    

        public Aula ObtenerAsync(int id)
        {            
            return _context.Aulas.Where(x => x.Id == id)
                .Include(i => i.Institucion)
                .Include(d => d.Docente)
                .Include(a => a.Asistencias).ThenInclude(asistencia => asistencia.AsistenciaAlumno)
                .Include(alumnos => alumnos.Alumnos)
                .FirstOrDefault();
        }
        public IEnumerable<Alumno> ObtenerAlumnosAula(int id)
        {
            var aulaWAlumnos = _context.Aulas.Where(x => x.Id == id)
                .Include(a => a.Alumnos).ThenInclude(ausenciaAlumno => ausenciaAlumno.Ausencias)
                .Include(a => a.Alumnos).ThenInclude(historialAlumno => historialAlumno.Historiales)
                .FirstOrDefault();

            return aulaWAlumnos.Alumnos;
        }
        public Aula ObtenerAulaDeAlumno(int idAlumno)
        {
            var aulasWAlumnos = _context.Aulas
                .Include(a => a.Alumnos)
                .Include(docente => docente.Docente)
                .Include(aula => aula.Asistencias)
                .ThenInclude(asistencia => asistencia.AsistenciaAlumno)
                .ToList();
            foreach (var aula in aulasWAlumnos)
            {
                foreach (var alumno in aula.Alumnos)
                {
                    if (alumno.Id == idAlumno)
                    {
                        return aula;
                    }
                }
            }
            return null;
        }

        public IEnumerable<Aula> ObtenerAulasDocente(int id)
        {
            var aulasWDocente = _context.Aulas.Where(x => x.Docente != null)
                .Include(d => d.Docente)
                .Include(a => a.Alumnos)
                .Include(a => a.Asistencias)
                .ToList();
            List<Aula> aulasDeDocente = new List<Aula>();
            foreach (var aula in aulasWDocente)
            {
                if (aula.Docente.Id == id)
                {
                    aulasDeDocente.Add(aula);
                }
            }
            return aulasDeDocente;
        }

        public Persona ObtenerDocenteDeAula(int AulaId)
        {
            var aulaWDocente = _context.Aulas.Where(x => x.Id == AulaId)
                .Include(d => d.Docente)
                .FirstOrDefault();
            
            return aulaWDocente.Docente;
        }

        public IEnumerable<Aula> ObtenerTodosAsync()
        {
            return _context.Aulas.ToList();
        }

        public bool VerificarAlumnoEnAula(int idAlumno)
        {
            var aulasWAlumnos = _context.Aulas
                .Include(a => a.Alumnos)
                .ToList();
            foreach (var aula in aulasWAlumnos)
            {
                foreach (var alumno in aula.Alumnos)
                {
                    if (alumno.Id == idAlumno)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void AgregarAsistenciaTomadaAula(int idAula, Asistencia nuevaAsistenciaTomada)
        {
            var aula = this.ObtenerAsync(idAula);
            if (aula!=null)
            {
                var asistenciaTomada = _context.AsistenciasTomadas.Where(x => x.Id == nuevaAsistenciaTomada.Id).FirstOrDefault();
                if (asistenciaTomada != null)
                {
                    aula.Asistencias.Add(asistenciaTomada);
                    _context.Entry(aula).State = EntityState.Modified;
                    _context.SaveChanges();
                }                
            }
        }        

        public ICollection<Aula> ObtenerAulasDeInstitucion(int id)
        {
            ICollection<Aula> aulasInstitucion = _context.Aulas.Where(x => x.Institucion.Id == id)
                .Include(alumnos => alumnos.Alumnos).ThenInclude(x => x.Historiales)
                .Include(docente => docente.Docente).ToList();

            return aulasInstitucion;
        }

        public Aula ObtenerAulaDeHijo(int idHijo)
        {
            var aulas = _context.Aulas
                .Include(x => x.Alumnos)
                .Include(docente => docente.Docente).ToList();
            foreach (var aula in aulas)
            {
                foreach (var alumnoDeAula in aula.Alumnos)
                {
                    if (alumnoDeAula.Id == idHijo)
                    {
                        return aula;
                    }
                }
            }
            return null;
        }

        public Aula ObtenerAulaConAsistencias(int idAula)
        {
            return _context.Aulas.Where(x => x.Id == idAula).Include(asistencias => asistencias.Asistencias).FirstOrDefault();
        }

        public IEnumerable<Alumno> ObtenerAlumnosSinAula(int id)
        {
            var alumnosInstitucion = _context.Personas.Where(x => x is Alumno && ((Alumno)x).Institucion.Id == id).ToList();
            if (alumnosInstitucion != null && alumnosInstitucion.Count() > 0)
            {
                var aulasInstitucion = _context.Aulas.Where(x => x.Institucion.Id == id);
                if (aulasInstitucion != null && aulasInstitucion.Count() > 0)
                {
                    List<Alumno> alumnosEnAulasInstitucion = new List<Alumno>();
                    foreach (var aula in aulasInstitucion)
                    {
                        var alumnos = this.ObtenerAlumnosAula(aula.Id);
                        if (alumnos != null && alumnos.Count() > 0)
                        {
                            alumnosEnAulasInstitucion.AddRange(alumnos);
                        }                        
                    }
                    List<Alumno> alumnosSinAulaAsignadaInstitucion = new List<Alumno>();
                    if (alumnosEnAulasInstitucion.Count() == alumnosInstitucion.Count())
                    {
                        return alumnosSinAulaAsignadaInstitucion;
                    }
                    else if(alumnosEnAulasInstitucion.Count() < alumnosInstitucion.Count())
                    {
                        foreach (var alumnoEnInstitucion in alumnosInstitucion)
                        {
                            var counter = 0;
                            foreach (var alumnoEnAulaInstitucion in alumnosEnAulasInstitucion)
                            {
                                if (((Alumno)alumnoEnInstitucion) == alumnoEnAulaInstitucion)
                                {
                                    break;
                                }
                                else
                                {
                                    counter += 1;
                                }
                            }
                            if (counter == alumnosEnAulasInstitucion.Count())
                            {
                                alumnosSinAulaAsignadaInstitucion.Add(((Alumno)alumnoEnInstitucion));
                            }
                        }
                        return alumnosSinAulaAsignadaInstitucion;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                List<Alumno> alumnosSinAulaAsignadaInstitucion = new List<Alumno>();
                return alumnosSinAulaAsignadaInstitucion;
            }
        }

        public IEnumerable<Persona> ObtenerDocentesSinAulaAsignada(int id)
        {
            var usuarios = _context.Usuarios.Include(usuario => usuario.Grupos).ToList();
            if (usuarios.Count() > 0 && usuarios != null)
            {
                List<Persona> docentesInstitucion = new List<Persona>();
                List<Persona> docentesConAulaInstitucion = new List<Persona>();
                

                foreach (var usuario in usuarios)
                {
                    if (usuario.Grupos != null)
                    {
                        foreach (var rol in usuario.Grupos)
                        {
                            if (rol.Tipo == "Docente")
                            {
                                var personaDeUsuario = this._personaRepositorie.ObtenerPersonaDeUsuario(usuario.Id);
                                if (personaDeUsuario != null)
                                {
                                    if (personaDeUsuario.Institucion.Id == id)
                                    {
                                        docentesInstitucion.Add(personaDeUsuario);
                                    }
                                }                                
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }                                       
                }

                var aulasInstitucion = this.ObtenerAulasDeInstitucion(id);
                if (aulasInstitucion != null && aulasInstitucion.Count() > 0)
                {
                    foreach (var aula in aulasInstitucion)
                    {
                        docentesConAulaInstitucion.Add(aula.Docente);
                    }
                }
                else
                {
                    return null;
                }

                List<Persona> docentesSinAulaInstitucion = new List<Persona>();
                foreach (var docenteInstitucion in docentesInstitucion)
                {
                    var counter = 0;
                    foreach (var docenteConAula in docentesConAulaInstitucion)
                    {
                        if (docenteInstitucion == docenteConAula)
                        {
                            break;
                        }
                        else
                        {
                            counter += 1;
                        }
                    }

                    if (counter == docentesConAulaInstitucion.Count())
                    {
                        docentesSinAulaInstitucion.Add(docenteInstitucion);
                    }
                }
                return docentesSinAulaInstitucion;
            }
            else
            {
                return null;
            }
        }

        public string CheckearValorRepetido(int idInstitucion, string nombreAula, string gradoAula, string DivisionAula)
        {
            foreach (var aula in _context.Aulas.Where(x => x.Institucion.Id == idInstitucion).Include(x => x.Docente).ToList())
            {
                if (nombreAula == aula.Nombre && gradoAula == aula.Grado && DivisionAula == aula.Division)
                {
                    return "Existente";
                }
            }
            return "";
        }

        public string CheckearValorRepetidoEdicionAula(int idAula, string nombreAula, string gradoAula, string DivisionAula)
        {
            foreach (var aula in _context.Aulas.Where(x => x.Id != idAula).ToList())
            {
                if (nombreAula == aula.Nombre && gradoAula == aula.Grado && DivisionAula == aula.Division)
                {
                    return "Existente";
                }                           
            }
            return "";
        }

        public bool EliminarAlumnoDeAula(int idAlumno)
        {           
            var aulaDeAlumno = this.ObtenerAulaDeAlumno(idAlumno);            
            if (aulaDeAlumno != null)
            {
                foreach (var alumno in aulaDeAlumno.Alumnos)
                {
                    if (alumno.Id == idAlumno)
                    {
                        foreach (var asistenciaAula in aulaDeAlumno.Asistencias)
                        {
                            foreach (var asistenciaTomada in asistenciaAula.AsistenciaAlumno)
                            {
                                if (asistenciaTomada.AlumnoId == idAlumno)
                                {
                                    asistenciaAula.AsistenciaAlumno.Remove(asistenciaTomada);
                                    _context.Entry(asistenciaAula).State = EntityState.Modified;                                    
                                }
                            }                            
                        }
                        
                        aulaDeAlumno.Alumnos.Remove(alumno);
                        aulaDeAlumno.CantidadAlumnos = aulaDeAlumno.CantidadAlumnos - 1;
                        _context.Entry(aulaDeAlumno).State = EntityState.Modified;
                    
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        public void CheckearAulasAsignadasAPersona(Persona persona)
        {                      
            foreach (var rol in persona.Usuario.Grupos)
            {
                if (rol.Tipo == "Docente")
                {
                    var aulasDelDocente = this.ObtenerAulasDocente(persona.Id);
                    if (aulasDelDocente.Count() > 0)
                    {
                        foreach (var aula in aulasDelDocente)
                        {
                            aula.Docente = null;
                            _context.Entry(aula).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                }
            }            
        }

        public void EliminarDocenteDeAulasAsignadas(int idDocente)
        {
            var aulasDocente = this.ObtenerAulasDocente(idDocente);
            if (aulasDocente != null)
            {
                for (int i = 0; i < aulasDocente.Count(); i++)
                {
                    var aula = aulasDocente.ElementAt(i);
                    if (aula != null)
                    {
                        aula.Docente = null;
                        _context.Entry(aula).State = EntityState.Modified;                        
                    }
                }
                _context.SaveChanges();
            }
            
        }        
    }
}
