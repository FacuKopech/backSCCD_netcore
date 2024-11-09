using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories
{
    public class PersonaRepositorio : IPersonaRepositorie
    {
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();

        public PersonaRepositorio()
        {
        }

        public void ActualizarNotasRecibidas(int id, Nota nuevaNota)
        {
            var destinatario = _context.Personas.FirstOrDefault(x => x.Id == id);
            if (destinatario != null)
            {
                destinatario.NotasRecibidas.Add(nuevaNota);

                _context.Entry(destinatario).State = EntityState.Modified;
            }
        }

        public void Agregar(Persona entity)
        {            
            if (entity.Usuario != null)
            {
                var existingUsuario = _context.Usuarios.Local.FirstOrDefault(u => u.Id == entity.Usuario.Id)
                                ?? _context.Usuarios.Find(entity.Usuario.Id);

                if (existingUsuario == null)
                {                    
                    _context.Attach(entity.Usuario);
                    _context.Entry(entity.Usuario).Collection(u => u.Grupos).Load();
                }
                else
                {
                    entity.Usuario = existingUsuario;
                }
                foreach (var trackedUsuario in _context.Usuarios.Local.ToList())
                {
                    if (trackedUsuario.Id != entity.Usuario.Id)
                    {
                        _context.Entry(trackedUsuario).State = EntityState.Detached;
                    }
                }
            }

            _context.Personas.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(int id)
        {
            var persona = _context.Personas.Where(x => x.Id == id).FirstOrDefault();
            if (persona != null)
            {
                if (persona is Alumno)
                {
                    var padresAlumno = this.ObtenerPadresDeAlumno(persona.Id);
                    if (padresAlumno != null && padresAlumno.Count() > 0)
                    {
                        foreach (var padre in padresAlumno)
                        {
                            if (padre is Directivo)
                            {
                                ((Directivo)padre).Hijos.Remove((Alumno)persona);
                            }else if (padre is Docente)
                            {
                                ((Docente)padre).Hijos.Remove((Alumno)persona);
                            }else if (padre is Padre)
                            {
                                ((Padre)padre).Hijos.Remove((Alumno)persona);
                            }
                            
                            _context.Entry(padre).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                    ((Alumno)persona).Historiales.Clear();
                    ((Alumno)persona).Asistencias.Clear();
                    ((Alumno)persona).Ausencias.Clear();
                    _context.Entry((Alumno)persona).State = EntityState.Modified;
                }
                           
                _context.Personas.Remove(persona);
                _context.Entry(persona).State = EntityState.Deleted;
                _context.SaveChanges();
            }           
        }

        public Alumno GetAlumno(int id)
        {
            return (Alumno)_context.Personas.Where(x => x.Id == id)
                .Include(h => ((Alumno)h).Historiales)
                .Include(a => ((Alumno)a).Ausencias)
                .Include(i => i.Institucion)
                .Include(asistencias => ((Alumno)asistencias).Asistencias)
                .FirstOrDefault();
        }
       

        public IEnumerable<Persona> GetDirectivosDocentesPadres()
        {
            var personas = _context.Personas.Where(x => !(x is Alumno))
                .Include(p => p.Usuario)
                .Include(i => i.Institucion);

            return personas;
        }

        public void Modificar(Persona entity)
        {
            if (entity.Usuario != null)
            {
                var existingUsuario = _context.Usuarios.Find(entity.Usuario.Id);

                if (existingUsuario == null)
                {
                    _context.Attach(entity.Usuario);
                    _context.Entry(entity.Usuario).Collection(u => u.Grupos).Load();
                }
                else
                {
                    entity.Usuario = existingUsuario;
                }
                _context.Entry(entity).State = EntityState.Modified;
                //foreach (var trackedUsuario in _context.Usuarios.Local.ToList())
                //{
                //    if (trackedUsuario.Id != entity.Usuario.Id || flag)
                //    {
                //        _context.Entry(trackedUsuario).State = EntityState.Detached;
                //    }

                //    if (trackedUsuario.Id == entity.Usuario.Id && !flag)
                //    {
                //        flag = true;
                //    }
                //}
            }
            _context.SaveChanges();
        }

        public IEnumerable<Persona> ObtenerAlumnosInstitucion(int id)
        {
            List<Persona> alumnos = _context.Personas.Where(x => x is Alumno)
                .Include(p => p.Institucion).ToList();

            List<Persona> alumnosInstitucion = new List<Persona>();
            foreach (var alumno in alumnos)
            {
                if (alumno.Institucion.Id == id)
                {
                    alumnosInstitucion.Add(alumno);
                }
            }

            return alumnosInstitucion;
        }

      
        public IEnumerable<Persona> ObtenerHijos(int id)
        {            
            var persona = _context.Personas.Where(x => x.Id == id)
                .Include(i => i.Institucion)                                
                .FirstOrDefault();

            if (persona is Padre padre)
            {
                _context.Entry(padre).Collection(p => p.Hijos).Load();
                return ((Padre)persona).Hijos;
            }
            else if (persona is Docente docente)
            {
                _context.Entry(docente).Collection(d => d.Hijos).Load();
                return ((Docente)persona).Hijos;
            }
            else if (persona is Directivo directivo)
            {
                _context.Entry(directivo).Collection(di => di.Hijos).Load();
                return ((Directivo)persona).Hijos;
            }

            return null;
        }
        public Persona ObtenerAsync(int id)
        {
            return _context.Personas.Where(x => x.Id == id)
                .Include(p => p.Usuario).ThenInclude(user => user.Grupos)
                .Include(i => i.Institucion)
                .Include(nf => nf.NotasFirmadas)
                .Include(nl => nl.NotasLeidas)
                .Include(nr => nr.NotasRecibidas)
                .ThenInclude(e => e.Emisor)
                .FirstOrDefault();

        }

        public Persona ObtenerPersonaDeUsuario(int idUser)
        {
          var persona = _context.Personas.Where(x => x.Usuario.Id == idUser)
                    .Include(p => p.Usuario).ThenInclude(g => g.Grupos)
                    .Include(i => i.Institucion)
                    .Include(nl => nl.NotasLeidas)
                    .Include(nf => nf.NotasFirmadas)
                    .Include(nr => nr.NotasRecibidas).ThenInclude(e => e.Emisor)
                    .Include(ea => ea.EventosAsistire)
                    .Include(ena => ena.EventosNoAsistire)
                    .Include(eta => eta.EventosTalVezAsista)
                    .FirstOrDefault();

            return persona;
        }

        public IEnumerable<Persona> ObtenerTodosAsync()
        {
            return _context.Personas
                .Include(usuario => usuario.Usuario)
                .ThenInclude(grupos => grupos.Grupos)
                .Include(institucion => institucion.Institucion)
                .ToList();
        }

        public IEnumerable<Persona> ObtenerPadres()
        {
            var padresPadres = _context.Personas.Where(x => (x is Padre && ((Padre)x).Hijos.Count() > 0))
                .Include(h => ((Padre)h).Hijos)
                .ThenInclude(hist => hist.Historiales)
                .ToList();

            var docentesPadres = _context.Personas.Where(x => (x is Docente && ((Docente)x).Hijos.Count() > 0))
                .Include(h => ((Docente)h).Hijos)
                .ThenInclude(hist => hist.Historiales)
                .ToList();

            var directivosPadres = _context.Personas.Where(x => (x is Directivo && ((Directivo)x).Hijos.Count() > 0))
                .Include(h => ((Directivo)h).Hijos)
                .ThenInclude(hist => hist.Historiales)
                .ToList();

            List<Persona> padres = new List<Persona>();            
            padres.AddRange(padresPadres);
            padres.AddRange(docentesPadres);
            padres.AddRange(directivosPadres);

            return padres;
        }

        public IEnumerable<Persona> ObtenerAlumno(int idAlumno)
        {
            return _context.Personas.Where(x => x is Alumno)                
                .ToList();

        }

        public void AgregarHijosAPadre(int idPadre, Persona alumno)
        {
            var persona = _context.Personas.Where(x => x.Id == idPadre).FirstOrDefault();
            var alumnoAgregado = _context.Personas.FirstOrDefault(x => x.Id == alumno.Id);
            if (persona is Padre padre)
            {
                _context.Entry(padre).Collection(p => p.Hijos).Load();
                padre.Hijos.Add((Alumno)alumnoAgregado);
            }
            else if (persona is Docente docente)
            {
                _context.Entry(docente).Collection(d => d.Hijos).Load();
                docente.Hijos.Add((Alumno)alumnoAgregado);
            }
            else if (persona is Directivo directivo)
            {
                _context.Entry(directivo).Collection(di => di.Hijos).Load();
                directivo.Hijos.Add((Alumno)alumnoAgregado);
            }
            
            _context.Entry(persona).State = EntityState.Modified;
            _context.Entry(alumnoAgregado).State = EntityState.Modified;
            _context.SaveChanges();

        }

        public void ActualizarHistorialAlumno(int idAlumno, Historial nuevoHistorial)
        {
            var alumno = (Alumno)_context.Personas.Where(x => x.Id == idAlumno)
                .Include(h => ((Alumno)h).Historiales)
                .FirstOrDefault();
            if (alumno != null)
            {
                var historial = alumno.Historiales.FirstOrDefault(x => x.IdHistorial == nuevoHistorial.IdHistorial);
                if (historial == null)
                {
                    alumno.Historiales.Add(nuevoHistorial);
                    
                }
                else
                {
                    _context.Entry(alumno).State = EntityState.Modified;

                }

                _context.SaveChanges();
            }
        }

        public void ActualizarAusenciaAlumno(int idAlumno, Ausencia nuevaAusencia, string accion)
        {
            var alumno = (Alumno)_context.Personas.Where(x => x.Id == idAlumno)
                .Include(a => ((Alumno)a).Ausencias)
                .FirstOrDefault();
            if (alumno != null)
            {
                var ausencia = alumno.Ausencias.FirstOrDefault(x => x.Id == nuevaAusencia.Id);
                if (ausencia != null)
                {
                    if (accion == "M")
                    {
                        ausencia = nuevaAusencia;
                        _context.Entry(alumno).State = EntityState.Modified;
                        _context.Entry(ausencia).State = EntityState.Modified;
                    }
                    else
                    {
                        alumno.Ausencias.Remove(ausencia);
                    }                  
                }                
                _context.SaveChanges();
            }
        }

        public IEnumerable<Persona> ObtenerPersonasInstitucion(Persona tipoPersona, int idInstitucion)
        {
            IEnumerable<Persona> personas = new List<Persona>();
            List<Persona> docentesYPadres = new List<Persona>();
            if (tipoPersona is Alumno)
            {
                personas = _context.Personas.Where(x => x is Alumno)
                   .Include(i => i.Institucion).Where(d => d.Institucion.Id == idInstitucion);

                return personas;
            }
            else
            {                
                personas = _context.Personas.Where(x => !(x is Alumno))
                      .Include(i => i.Institucion)
                      .Include(u => u.Usuario).ThenInclude(g => g.Grupos)
                      .Where(d => d.Institucion.Id == idInstitucion);
                
                foreach (var persona in personas)
                {
                    foreach (var grupoPersona in persona.Usuario.Grupos)
                    {
                        if (grupoPersona.Tipo == "Docente" || grupoPersona.Tipo == "Padre")
                        {
                            docentesYPadres.Add(persona);
                        }
                    }
                }
                return docentesYPadres;
            }            
        }

        public Task<bool> EliminarHistorial(int idAlumno, Historial historial)
        {
            var alumno = (Alumno)_context.Personas.Where(x => x.Id == idAlumno)
                .Include(h => ((Alumno)h).Historiales)
                .FirstOrDefault();

            if (alumno != null)
            {
                var historialAEliminar = alumno.Historiales.FirstOrDefault(x => x.IdHistorial == historial.IdHistorial);
                if (historialAEliminar != null)
                {
                    alumno.Historiales.Remove(historialAEliminar);
                   
                    _context.Entry(alumno).State = EntityState.Modified;
                    _context.Entry(historialAEliminar).State = EntityState.Deleted;

                    _context.SaveChanges();
                    return new Task<bool>(() => true);
                }
                return new Task<bool>(() => false);
            }
            return new Task<bool>(() => false);
        }

        public IEnumerable<Persona> ObtenerPadresDeAlumno(int id)
        {
            List<Persona> padresDeAlumno = new List<Persona>();
            foreach (var persona in _context.Personas.Where(x => 
            (x is Padre && ((Padre)x).Hijos.Count() > 0) ||
            (x is Docente && ((Docente)x).Hijos.Count() > 0) ||
            (x is Directivo && ((Directivo)x).Hijos.Count() > 0)))
            {
                if (persona is Padre)
                {
                    foreach (var hijo in ((Padre)persona).Hijos)
                    {
                        if (hijo.Id == id)
                        {
                            padresDeAlumno.Add(persona);
                        }
                    }
                }else if (persona is Docente)
                {
                    foreach (var hijo in ((Docente)persona).Hijos)
                    {
                        if (hijo.Id == id)
                        {
                            padresDeAlumno.Add(persona);
                        }
                    }
                }else if (persona is Directivo)
                {
                    foreach (var hijo in ((Directivo)persona).Hijos)
                    {
                        if (hijo.Id == id)
                        {
                            padresDeAlumno.Add(persona);
                        }
                    }
                }
            }            
            return padresDeAlumno;
        }

        public Persona ObtenerDirectivoInstitucion(int id)
        {
            var personas = _context.Personas.Where(x => x.Institucion.Id == id && !(x is Alumno)).Include(u => u.Usuario).ThenInclude(g => g.Grupos);
            if (personas != null)
            {
                foreach (var persona in personas)
                {
                    foreach (var grupo in persona.Usuario.Grupos)
                    {
                        if (grupo.Tipo == "Directivo")
                        {
                            return persona;
                        }
                    }
                }
                return null;
            }
            return null;
        }

        public void ActualizarAsistenciaAlumno(int idAlumno)
        {
            var alumno = _context.Personas.Where(x => x.Id == idAlumno).FirstOrDefault();
            if (alumno != null)
            {                
                _context.Entry(alumno).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public List<string> ObtenerEmailsDeTodasLasPersonasDeUnaInstitucion(int id)
        {
            var destinatarios = _context.Personas.Where(x => x.Institucion.Id == id && x.Email != null);
            List<string> emails = new List<string>();
            foreach (var destinatario in destinatarios)
            {
                emails.Add(destinatario.Email);
            }

            return emails;
        }

        public List<string> ObtenerEmailsParaDocente(Persona docenteLogueada)
        {
            List<string> emailsDestinatarios = new List<string>();
            List<Persona> docentesYDirectivo = new List<Persona>();

            var personasInstitucion = _context.Personas.Where(x => !(x is Alumno) 
            && (x.Institucion.Id == docenteLogueada.Institucion.Id) 
            && (x.Id != docenteLogueada.Id)).Include(u => u.Usuario).ThenInclude(g => g.Grupos);
            
            foreach (var persona in personasInstitucion)
            {
                foreach (var grupo in persona.Usuario.Grupos)
                {
                    if (grupo.Tipo == "Directivo" || grupo.Tipo == "Docente")
                    {
                        docentesYDirectivo.Add(persona);
                    }
                }
            }
            
            foreach (var persona in docentesYDirectivo)
            {
                emailsDestinatarios.Add(persona.Email);
            }
            var aulasDocente = _context.Aulas.Where(x => x.Docente != null && x.Docente.Id == docenteLogueada.Id)
                .Include(alumnos => alumnos.Alumnos)
                .ToList();
            foreach (var aula in aulasDocente)
            {
                foreach (var alumno in aula.Alumnos)
                {
                    var padresAlumno = this.ObtenerPadresDeAlumno(alumno.Id);
                    if (padresAlumno != null)
                    {
                        foreach (var padre in padresAlumno)
                        {
                            emailsDestinatarios.Add(padre.Email);
                        }                        
                    }
                }
            }

            return emailsDestinatarios;
        }

        public List<string> ObtenerEmailsParaPadre(Persona padreLogueado)
        {
            List<string> emailsDestinatarios = new List<string>();
            var aulas = _context.Aulas
             .Include(alumnos => alumnos.Alumnos)
             .Include(docentre => docentre.Docente)
             .ToList();

            var persona = _context.Personas.Where(x => x.Id == padreLogueado.Id).FirstOrDefault();

            var personasConHijos = _context.Personas.Where(x =>
            (x is Padre && ((Padre)x).Hijos.Count() > 0) ||
            (x is Docente && ((Docente)x).Hijos.Count() > 0) ||
            (x is Directivo && ((Directivo)x).Hijos.Count() > 0)).ToList();

            if (persona is Padre padre)
            {
                _context.Entry(padre).Collection(p => p.Hijos).Load();
                foreach (var hijo in padre.Hijos)
                {
                    foreach (var aula in aulas)
                    {
                        var alumnoHijoPadreLoegueado = aula.Alumnos.FirstOrDefault(x => x.Id == hijo.Id);
                        if (alumnoHijoPadreLoegueado != null)
                        {
                            emailsDestinatarios.Add(aula.Docente.Email);
                            foreach (var otroPadre in personasConHijos)
                            {
                                if (otroPadre is Padre)
                                {
                                    foreach (var hijoDeOtroPadre in ((Padre)otroPadre).Hijos)
                                    {
                                        var hijoDeOtroPadreEnMismaAula = aula.Alumnos.FirstOrDefault(x => x.Id == hijoDeOtroPadre.Id);
                                        if (hijoDeOtroPadreEnMismaAula != null)
                                        {
                                            emailsDestinatarios.Add(otroPadre.Email);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            else if (persona is Docente docente)
            {
                _context.Entry(docente).Collection(d => d.Hijos).Load();
                foreach (var hijo in docente.Hijos)
                {
                    foreach (var aula in aulas)
                    {
                        var alumnoHijoPadreLoegueado = aula.Alumnos.FirstOrDefault(x => x.Id == hijo.Id);
                        if (alumnoHijoPadreLoegueado != null)
                        {
                            emailsDestinatarios.Add(aula.Docente.Email);
                            foreach (var otroPadre in personasConHijos)
                            {
                                if (otroPadre is Docente)
                                {
                                    foreach (var hijoDeOtroPadre in ((Docente)otroPadre).Hijos)
                                    {
                                        var hijoDeOtroPadreEnMismaAula = aula.Alumnos.FirstOrDefault(x => x.Id == hijoDeOtroPadre.Id);
                                        if (hijoDeOtroPadreEnMismaAula != null)
                                        {
                                            emailsDestinatarios.Add(otroPadre.Email);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            else if (persona is Directivo directivo)
            {
                _context.Entry(directivo).Collection(di => di.Hijos).Load();
                foreach (var hijo in directivo.Hijos)
                {
                    foreach (var aula in aulas)
                    {
                        var alumnoHijoPadreLoegueado = aula.Alumnos.FirstOrDefault(x => x.Id == hijo.Id);
                        if (alumnoHijoPadreLoegueado != null)
                        {
                            emailsDestinatarios.Add(aula.Docente.Email);
                            foreach (var otroPadre in personasConHijos)
                            {
                                if (otroPadre is Directivo)
                                {
                                    foreach (var hijoDeOtroPadre in ((Directivo)otroPadre).Hijos)
                                    {
                                        var hijoDeOtroPadreEnMismaAula = aula.Alumnos.FirstOrDefault(x => x.Id == hijoDeOtroPadre.Id);
                                        if (hijoDeOtroPadreEnMismaAula != null)
                                        {
                                            emailsDestinatarios.Add(otroPadre.Email);
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
             
            var directivoInstitucion = this.ObtenerDirectivoInstitucion(padreLogueado.Institucion.Id);
            if (directivoInstitucion != null)
            {
                emailsDestinatarios.Add(directivoInstitucion.Email);
            }            
            
            return emailsDestinatarios;
        }

        public void AgregarAsistenciaAlumno(int idAlumno, AsistenciaAlumno nuevaAsistenciaAlumno)
        {
            var alumno = (Alumno)_context.Personas.Where(x => x.Id == idAlumno).Include(asistenciaAlumno => ((Alumno)asistenciaAlumno).Asistencias)
                .FirstOrDefault();
            if (alumno != null)
            {
                var asistenciaCargada = alumno.Asistencias.Where(x => x.AsistenciaId == nuevaAsistenciaAlumno.AsistenciaId);
                if (asistenciaCargada == null)
                {
                    alumno.Asistencias.Add(nuevaAsistenciaAlumno);
                    _context.SaveChanges();
                }
            }
        }

        public IEnumerable<Persona> ObtenerAlumnosSistema()
        {
            return _context.Personas.Where(x => x is Alumno).ToList();
        }

        public IEnumerable<Persona> ObtenerPersonasSistema()
        {
            List<Persona> personasSistema = new List<Persona>();
            var padres = _context.Personas.Where(x => x is Padre && ((Padre)x).Hijos.Count() > 0).Include(hijos => (((Padre)hijos).Hijos)).ToList();
            var docentes = _context.Personas.Where(x => x is Docente && ((Docente)x).Hijos.Count() > 0).Include(hijos => (((Docente)hijos).Hijos)).ToList();
            var directivos = _context.Personas.Where(x => x is Directivo && ((Directivo)x).Hijos.Count() > 0).Include(hijos => (((Directivo)hijos).Hijos)).ToList();
            personasSistema.AddRange(padres);
            personasSistema.AddRange(docentes);
            personasSistema.AddRange(directivos);

            return personasSistema;
        }

        public IEnumerable<Persona> ObtenerPadresDocentesDirectivosInstitucion(int idInstitucion)
        {
            return _context.Personas.Where(x => (!(x is Alumno)) && x.Institucion.Id == idInstitucion).ToList();
        }

        public IEnumerable<Persona> ObtenerDocentesDeInstitucion(int id)
        {
            return _context.Personas.Where(x => x is Docente && x.Institucion.Id == id).ToList();
        }
    }
} 
