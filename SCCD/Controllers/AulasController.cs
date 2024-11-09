using Data;
using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos;
using SCCD.Command.Ausencia;
using AutoMapper;
using System.Collections.Generic;

namespace SCCD.Controllers
{
    [ApiController]
    public class AulasController : Controller
    {
        private readonly ApplicationDbContext _context;

        private IAusenciaCommand _ausenciaCommand;
        private IAulaRepositorie _aulaRepositorie;
        private IPersonaRepositorie _personaRepositorie;
        private IUsuarioRepositorie _usuarioRepositorie;
        private IInstitucionRepositorie _institucionRepositorie;
        private IAsistenciaRepositorie _asistenciaRepositorie;
        private INotaRepositorie _notaRepositorie;
        private int MaxAlumnosXAula = 20;
        private readonly IMapper _mapper;
        private Session _session = Session.GetInstance();
   
        public AulasController(IAulaRepositorie aulasRepositorie, IInstitucionRepositorie institucionRepositorie, 
           IPersonaRepositorie personaRepositorie, IUsuarioRepositorie usuarioRepositorie, IAsistenciaRepositorie asistenciaRepositorie,
           IMapper mapper, IAusenciaCommand ausenciaCommand, INotaRepositorie notaRepositorie)
        {

            _aulaRepositorie = aulasRepositorie;
            _personaRepositorie = personaRepositorie;
            _usuarioRepositorie = usuarioRepositorie;
            _institucionRepositorie = institucionRepositorie;
            _asistenciaRepositorie = asistenciaRepositorie;
            _mapper = mapper;
            _ausenciaCommand = ausenciaCommand;
            _notaRepositorie = notaRepositorie;
        }
       
        [HttpGet]
        [Route("/[controller]/[action]/{idHijo}")]        
        public IActionResult ObtenerAulaDeHijo(int idHijo)
        {
            try
            {
                var aula = _aulaRepositorie.ObtenerAulaDeHijo(idHijo);               
                return Ok(aula);                
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }            

        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerAulasDeHijos()
        {
            try
            {
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                var hijosPadreLogueado = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                List<Aula> aulasHijos = new List<Aula>();
                if (hijosPadreLogueado.Count() > 0)
                {
                    foreach (var hijo in hijosPadreLogueado)
                    {
                        var aula = _aulaRepositorie.ObtenerAulaDeHijo(hijo.Id);
                        if (aula != null)
                        {
                            if (aulasHijos.Count() > 0)
                            {
                                var aulaRepetida = aulasHijos.FirstOrDefault(x => x.Id == aula.Id);
                                if (aulaRepetida == null)
                                {
                                    aulasHijos.Add(aula);                                    
                                }
                                continue;
                            }
                            else
                            {
                                aulasHijos.Add(aula);
                            }                                                        
                        }
                        else
                        {
                            return NotFound($"Aula de {hijo.Apellido}, {hijo.Nombre} no encontrada");
                        }
                    }                    
                }
                else
                {
                    return NotFound("Hijos no encontrados");
                }

                return Ok(aulasHijos);                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idNota}")]
        public IActionResult ObtenerAulasDestinadasNota(int idNota)
        {
            try
            {
                List<Aula> aulasARetornar = new List<Aula>();
                var nota = _notaRepositorie.ObtenerAsync(idNota);
                if (nota != null && nota.AulasDestinadas != null && nota.AulasDestinadas.Count() > 0)
                {
                    foreach (var aulaEnNota in nota.AulasDestinadas)
                    {
                        aulasARetornar.Add(aulaEnNota);
                    }
                    return Ok(aulasARetornar);
                }
                else
                {
                    return NotFound(false);
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerAulasDocente()
        {
            try
            {                
                var docenteLogueado = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                if (docenteLogueado == null)
                {
                    return NotFound(false);
                }
                else
                {
                    var aulasDocenteLogueado = _aulaRepositorie.ObtenerAulasDocente(docenteLogueado.Id);
                    return Ok(aulasDocenteLogueado);
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }           
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerAulasInstitucion()
        {
            try
            {
                var directivoLogueado = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                if (directivoLogueado == null)
                {
                    return NotFound(false);
                }
                else
                {
                    var aulasInstitucion = _aulaRepositorie.ObtenerAulasDeInstitucion(directivoLogueado.Institucion.Id);
                    return Ok(aulasInstitucion);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAula}")]
        public IActionResult ObtenerAsistenciasAula(int idAula)
        {
            try
            {
                if (idAula == 0)
                {
                    return BadRequest(false);
                }
                else
                {
                    var aula = _aulaRepositorie.ObtenerAsync(idAula);
                    if (aula != null && aula.Asistencias != null)
                    {
                        return Ok(aula.Asistencias);
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAula}/{idAsistencia}/{esPresentes}")]
        public IActionResult ObtenerAsistenciaAlumnos(int idAula, int idAsistencia, bool esPresentes)
        {
            try
            {
                if (idAula == 0 || idAsistencia == 0 || esPresentes == null)
                {
                    return BadRequest(false);
                }
                else
                {                    
                    var aula = _aulaRepositorie.ObtenerAsync(idAula);
                    if (aula != null && aula.Asistencias != null)
                    {
                        var asistencia = aula.Asistencias.Where(x => x.Id == idAsistencia).FirstOrDefault();
                        if (asistencia != null)
                        {
                            if (asistencia.AsistenciaAlumno.Count() > 0 && asistencia.AsistenciaAlumno != null)
                            {
                                List<Alumno> alumnosARetornar = new List<Alumno>();
                                if (esPresentes)
                                {
                                    foreach (var asistenciaAlumno in asistencia.AsistenciaAlumno)
                                    {
                                        if (asistenciaAlumno.Estado == "Presente")
                                        {
                                            var alumno = _personaRepositorie.GetAlumno(asistenciaAlumno.AlumnoId);
                                            if (alumno != null)
                                            {
                                                alumnosARetornar.Add(alumno);
                                            }
                                            else
                                            {
                                                return NotFound(false);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var asistenciaAlumno in asistencia.AsistenciaAlumno)
                                    {
                                        if (asistenciaAlumno.Estado == "Ausente")
                                        {
                                            var alumno = _personaRepositorie.GetAlumno(asistenciaAlumno.AlumnoId);
                                            if (alumno != null)
                                            {
                                                alumnosARetornar.Add(alumno);
                                            }
                                            else
                                            {
                                                return NotFound(false);
                                            }
                                        }
                                    }
                                }
                                return Ok(alumnosARetornar);
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        else
                        {
                            return NotFound(false);
                        }
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IEnumerable<AlumnoConEnumerables> ObtenerAlumnosAula([FromBody] Dictionary<string, int> aulaData)
        {
            try
            {
                int idAula = aulaData["idAula"];
                var alumnosDeAula = _aulaRepositorie.ObtenerAlumnosAula(idAula);
                if (alumnosDeAula.Count() == 0)
                {
                    return null;
                }
                else
                {
                    ICollection<AlumnoConEnumerables> alumnosConEnumerables = new List<AlumnoConEnumerables>();
                    foreach (var alumno in alumnosDeAula)
                    {
                        AlumnoConEnumerables alumnoAAgregar = new AlumnoConEnumerables
                        {
                            Id = alumno.Id,
                            Nombre = alumno.Nombre,
                            Apellido = alumno.Apellido,
                            DNI = alumno.DNI,
                            Email = alumno.Email,
                            Telefono = alumno.Telefono,
                            Domicilio = alumno.Domicilio,
                            Institucion = alumno.Institucion,
                            FechaNacimiento = alumno.FechaNacimiento,                            
                            Asistencia = alumno.Asistencia,
                            Ausencias = new List<Ausencia>(alumno.Ausencias),
                            Historiales = new List<Historial>(alumno.Historiales)
                        };
                        alumnosConEnumerables.Add(alumnoAAgregar);
                    }
                    IEnumerable<AlumnoConEnumerables> alumnosARetornar = alumnosConEnumerables;
                    return alumnosARetornar;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idInstitucion}")]
        public IActionResult ObtenerPorcentajesAsistenciaAulas(int idInstitucion)
        {
            try
            {
                var aulasInstitucion = _aulaRepositorie.ObtenerAulasDeInstitucion(idInstitucion);
                if (aulasInstitucion.Count() == 0)
                {
                    return NotFound("Institucion sin Aulas");
                }
                else
                {
                    decimal contadorPorcentajes = 0;
                    List<decimal> porcentajesAsistencias = new List<decimal>(); 
                    foreach (var aula in aulasInstitucion)
                    {
                        if (aula.Alumnos != null && aula.Alumnos.Count() > 0)
                        {
                            foreach (var alumno in aula.Alumnos)
                            {
                                contadorPorcentajes += alumno.Asistencia;
                            }
                            var porcentajeAsistenciaAula = (contadorPorcentajes / aula.Alumnos.Count());
                            porcentajesAsistencias.Add(porcentajeAsistenciaAula);
                            contadorPorcentajes = 0;
                        }
                        else
                        {
                            porcentajesAsistencias.Add(0);
                        }                       
                    }
                    return Ok(porcentajesAsistencias);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idInstitucion}")]
        public IActionResult ObtenerAlumnosSinAula(int idInstitucion)
        {
            try
            {
                var alumnosSinAula = _aulaRepositorie.ObtenerAlumnosSinAula(idInstitucion);
                if (alumnosSinAula == null)
                {
                    return NotFound();
                }
                else
                {                    
                    return Ok(alumnosSinAula);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idInstitucion}")]
        public IActionResult ObtenerDocentesSinAulaAsignada(int idInstitucion)
        {
            try
            {
                var docentesSinAula = _aulaRepositorie.ObtenerDocentesSinAulaAsignada(idInstitucion);
                if (docentesSinAula == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(docentesSinAula);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idInstitucion}")]
        public IActionResult ObtenerDocentesDeInstitucion(int idInstitucion)
        {
            try
            {
                var docentesDeInstitucion = _personaRepositorie.ObtenerDocentesDeInstitucion(idInstitucion);
                if (docentesDeInstitucion == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(docentesDeInstitucion);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Route("/[controller]/[action]/")]
        public IActionResult AgregarAula([FromBody] AulaAAgregar aulaAAgregar)
        {
            try
            {
                if (aulaAAgregar == null || aulaAAgregar.InstitucionId <= 0 || aulaAAgregar.DocenteId < 0)
                {
                    return BadRequest();
                }
                else
                {
                    var valorRepetido = _aulaRepositorie.CheckearValorRepetido(aulaAAgregar.InstitucionId, aulaAAgregar.NombreAula, aulaAAgregar.GradoAula, aulaAAgregar.DivisionAula);
                    if (valorRepetido == "")
                    {
                        Aula nuevaAula = new Aula
                        {
                            Nombre = aulaAAgregar.NombreAula,
                            Grado = aulaAAgregar.GradoAula,
                            Division= aulaAAgregar.DivisionAula,
                            CantidadAlumnos = aulaAAgregar.AlumnosSeleccionados.Count(),
                            Asistencias = new List<Asistencia>(),
                            NotasParaAula = new List<Nota>(),
                            Eventos = new List<Evento>(),
                        };

                        var institucion = _institucionRepositorie.ObtenerAsync(aulaAAgregar.InstitucionId);
                        if (institucion != null)
                        {
                            nuevaAula.Institucion = institucion;
                        }
                        else
                        {
                            return NotFound();
                        }

                        if (aulaAAgregar.DocenteId == 0)
                        {
                            nuevaAula.Docente = null;
                        }
                        else{
                            var docente = _personaRepositorie.ObtenerAsync(aulaAAgregar.DocenteId);
                            if (docente != null)
                            {
                                nuevaAula.Docente = (Docente)docente;
                            }
                            else
                            {
                                return NotFound();                                
                            }
                        }

                        if (aulaAAgregar.AlumnosSeleccionados.Count() > 0)
                        {
                            nuevaAula.Alumnos = new List<Alumno>();
                            foreach (var alumnoSeleccionado in aulaAAgregar.AlumnosSeleccionados)
                            {
                               var alumno = _personaRepositorie.GetAlumno(alumnoSeleccionado);
                                if (alumno != null)
                                {
                                    nuevaAula.Alumnos.Add(alumno);
                                }
                                else
                                {
                                    return NotFound();
                                }
                            }
                        }
                        else
                        {
                            nuevaAula.Alumnos = new List<Alumno>();
                        }

                        _aulaRepositorie.Agregar(nuevaAula);                        
                    }
                    else if(valorRepetido == "Existente")
                    {
                        return BadRequest("Aula existente");
                    }                    
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idAula}")]
        public IActionResult EditarAula(int idAula, [FromBody]AulaAAgregar aulaAEditar)
        {
            try
            {
                var aula = _aulaRepositorie.ObtenerAsync(idAula);
                if (aula != null)
                {
                    var valorRepetido = _aulaRepositorie.CheckearValorRepetidoEdicionAula(aula.Id, aulaAEditar.NombreAula, aulaAEditar.GradoAula, aulaAEditar.DivisionAula);
                    if (valorRepetido == "Existente")
                    {
                        return BadRequest("Aula existente");
                    }
                    
                    if(aulaAEditar.DocenteId > 0)
                    {   
                        var docente = _personaRepositorie.ObtenerAsync(aulaAEditar.DocenteId);
                        if (docente != null)
                        {
                            aula.Docente = (Docente)docente;
                        }
                        else
                        {
                            return NotFound(false);
                        }
                    }else if (aula.Docente != null && aulaAEditar.DocenteId == 0)
                    {
                        aula.Docente = null;
                    }

                    aula.Nombre = aulaAEditar.NombreAula;
                    aula.Grado = aulaAEditar.GradoAula;
                    aula.Division = aulaAEditar.DivisionAula;
                    _aulaRepositorie.Modificar(aula);
                    return Ok(true);
                }
                else
                {
                    return NotFound(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idAlumno}")]
        public IActionResult EliminarAlumnoDeAula(int idAlumno)
        {
            try
            {
                return Ok(_aulaRepositorie.EliminarAlumnoDeAula(idAlumno));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idAula}")]
        public IActionResult AgrgarAlumnoExistenteAAula(int idAula, [FromBody]AlumnosExistentesAAgregar alumnosAAgregar)
        {
            try
            {
                if (idAula <= 0 || alumnosAAgregar.AlumnosSeleccionados.Count() == 0 || alumnosAAgregar == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    var aula = _aulaRepositorie.ObtenerAsync(idAula);
                    if (aula != null)
                    {
                        foreach (var alumnoSeleccionado in alumnosAAgregar.AlumnosSeleccionados)
                        {
                            var alumno = _personaRepositorie.GetAlumno(alumnoSeleccionado);
                            if (alumno != null)
                            {
                                aula.Alumnos.Add(alumno);
                                aula.CantidadAlumnos += 1;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        _aulaRepositorie.Modificar(aula);
                        return Ok(true);
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idAula}")]
        public IActionResult AgrgarAlumnoNuevoAAula(int idAula, [FromBody]NuevoAlumno nuevoAlumnoAAgregar)
        {
            try
            {
                if (idAula <= 0 || nuevoAlumnoAAgregar == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    var aula = _aulaRepositorie.ObtenerAsync(idAula);
                    if (aula != null)
                    {
                        var alumnosInstitucion = _personaRepositorie.ObtenerAlumnosInstitucion(aula.Institucion.Id);
                        if (alumnosInstitucion != null)
                        {
                            foreach (var alumno in alumnosInstitucion)
                            {
                                if (nuevoAlumnoAAgregar.DNI == alumno.DNI)
                                {
                                    return BadRequest("El DNI de Alumno que intenta agregar ya existe");
                                }
                            }
                            Alumno nuevoAlumno = new Alumno
                            {
                                Nombre = nuevoAlumnoAAgregar.Nombre,
                                Apellido = nuevoAlumnoAAgregar.Apellido,
                                DNI = nuevoAlumnoAAgregar.DNI,
                                Email = null,
                                Telefono = null,
                                Domicilio = null,                                
                                Usuario = null,
                                Institucion = aula.Institucion,
                                NotasRecibidas = new List<Nota>(),
                                NotasLeidas = new List<Nota>(),
                                NotasFirmadas = new List<Nota>(),                                
                                FechaNacimiento = nuevoAlumnoAAgregar.FechaNacimiento,
                                Historiales = new List<Historial>(),
                                Ausencias = new List<Ausencia>(),
                                Asistencias = new List<AsistenciaAlumno>(),
                                Asistencia = 0
                            };

                            _personaRepositorie.Agregar(nuevoAlumno);
                            aula.Alumnos.Add(nuevoAlumno);
                            aula.CantidadAlumnos += 1;
                            _aulaRepositorie.Modificar(aula);
                            return Ok(true);
                        }
                        else
                        {
                            return NotFound(false);
                        }                       
                    }
                    else
                    {
                        return NotFound(false);
                    }                   
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Cuando se carga una nueva asistencia por parte del docente, se les pone presentes a los alumnos que llegan en la liste de Alumnos en el parametro

        //Si un alumno, para la fecha en la que se esta tomando asistencia, no esta presente en el aula, mas alla de que tenga ausencias cargadas o no, se le pone Ausente. Luego, 
        //dependiendo de las ausencias que tenga cargadas y si estan justificadas o no, se le descontara cierto % del porcentaje de asistencia total

        //Al alumno que, estando ausente en la toma de asistencia, si no tiene ninguna ausencia cargada para la fecha, se le crea automaticamente una ausencia con motivo "Toma de
        //asistencia - Hijo/a ausente"
        [HttpPost]
        [Route("/[controller]/[action]/{idAula}")]
        public IActionResult CargarNuevaAsistencia(int idAula, [FromBody] List<Alumno> alumnos)
        {
            try
            {
                if (alumnos == null || idAula == 0)
                {
                    return BadRequest(false);
                }
                else
                {
                    Asistencia nuevaAsistencia = new Asistencia
                    {
                        FechaAsistenciaTomada = DateTime.Today
                    };
                    _asistenciaRepositorie.Agregar(nuevaAsistencia); //Generamos la nueva asistencia y la agregamos a la DB
                    var ultimaAsistenciaAgregada = _asistenciaRepositorie.ObtenerUltimaAsistenciaAgregada(); //Obtenemos esa ultima Asistencia
                    _aulaRepositorie.AgregarAsistenciaTomadaAula(idAula, ultimaAsistenciaAgregada); //Agregamos la asistencia a la lista de Asistencias en el Aula
                    if (ultimaAsistenciaAgregada != null)
                    {
                        foreach (var alumnoPresente in alumnos) //De cada alumno presente, le agregamos la AsistenciaAlumno a la lista de asistencias en Alumno (linea 340)
                        {
                            AsistenciaAlumno asistenciaAlumno = new AsistenciaAlumno
                            {
                                AsistenciaId = ultimaAsistenciaAgregada.Id,
                                AlumnoId = alumnoPresente.Id,
                                Estado = "Presente"
                            };
                            _asistenciaRepositorie.AgregarAsistenciaAlumno(asistenciaAlumno); //Tambien agregamos la AsistenciaAlumno en su tabla                            
                            _personaRepositorie.AgregarAsistenciaAlumno(alumnoPresente.Id, asistenciaAlumno);
                        }

                        var alumnosAula = _aulaRepositorie.ObtenerAlumnosAula(idAula);
                        foreach (var alumnoDeAula in alumnosAula)
                        {
                            var alumnoEncontrado = alumnos.Where(x => x.Id == alumnoDeAula.Id).FirstOrDefault();
                            if (alumnoEncontrado == null)
                            {
                                AsistenciaAlumno asistenciaAlumno = new AsistenciaAlumno
                                {
                                    AsistenciaId = ultimaAsistenciaAgregada.Id,
                                    AlumnoId = alumnoDeAula.Id,
                                    Estado = "Ausente"
                                };
                                _asistenciaRepositorie.AgregarAsistenciaAlumno(asistenciaAlumno);
                                _personaRepositorie.AgregarAsistenciaAlumno(alumnoDeAula.Id, asistenciaAlumno);

                                var ausenciaParaLaFecha = alumnoDeAula.Ausencias.Where(x => x.FechaComienzo <= DateTime.Today && x.FechaFin >= DateTime.Today).FirstOrDefault();
                                if (ausenciaParaLaFecha == null)
                                {
                                    AusenciaModificar nuevaAusencia = new AusenciaModificar { 
                                        FechaComienzo= DateTime.Today,
                                        FechaFin= DateTime.Today,
                                        Motivo = "Toma de asistencia - Hijo/a ausente"
                                    };
                                    _ausenciaCommand.AgregarAusencia(alumnoDeAula.Id, nuevaAusencia);
                                }                                
                            }
                        }
                        CalcularYActualizarPorcentajesDeAsistenciaAlumnos(idAula);
                    }
                    else
                    {
                        return NotFound(false);
                    }

                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Calculo de % de Asistencia:
        //Se hace regla de 3. Se consideran la cantidad total de asistencias como el 100%, y se saca el porcentaje para la cantidad de presentes y ausentes sobre ese total
        //Se van sumando al porcentaje
        //Si el alumno esta presente en todas, le queda el 100%
        //Si el alumno esta ausente y no tiene ninguna Ausencia cargada o si la tiene pero la misma NO esta justificada, no se le suma nada al %, se deja como esta, por Ausente
        //Si el alumno esta ausente pero tiene una Ausencia cargada y ademas la misma SI esta justificada, se lo toma como presente pero al % se le suma la mitad de esa asistencia
        //De lo contrario, no seria justo para aquel que siempre va a clases, por mas que el que falte este justificado

        //Ademas, aquel alumno que esta ausente y no tiene ausencia cargada o si la tiene pero la misma NO esta justificada, luego de tomar asistencia, el sistema le genera una nueva
        //Ausencia con motivo "Toma de asistencia - Hijo/a ausente"
        [NonAction]
        public void CalcularYActualizarPorcentajesDeAsistenciaAlumnos(int idAula)
        {
            var aulaConAsistencias = _aulaRepositorie.ObtenerAulaConAsistencias(idAula);
            var alumnosAula = _aulaRepositorie.ObtenerAlumnosAula(idAula);
            var cantidadDeAsistenciasTomadasEnAula = aulaConAsistencias.Asistencias.Count();

            foreach (var alumno in alumnosAula)
            {
                var cantidadPresentes = alumno.Asistencias.Where(x => x.Estado == "Presente").Count();
                alumno.Asistencia = (cantidadPresentes * 100) / cantidadDeAsistenciasTomadasEnAula;

                foreach (var asistenciaAusente in alumno.Asistencias.Where(x => x.Estado == "Ausente").ToList())
                {
                    var asistenciaTomada = _asistenciaRepositorie.ObtenerAsistencia(asistenciaAusente.AsistenciaId);
                    var ausenciaParaEsaFecha = alumno.Ausencias.Where(x => x.FechaComienzo <= asistenciaTomada.FechaAsistenciaTomada 
                    && x.FechaFin >= asistenciaTomada.FechaAsistenciaTomada).FirstOrDefault();

                    if (ausenciaParaEsaFecha != null && ausenciaParaEsaFecha.Justificada == "Si")
                    {
                        alumno.Asistencia = alumno.Asistencia + ((100 / cantidadDeAsistenciasTomadasEnAula) / 2);
                    }
                }
                _personaRepositorie.ActualizarAsistenciaAlumno(alumno.Id);
            }            
        }              

        [HttpDelete]
        [Route("/[controller]/[action]/{idAula}")]
        public IActionResult EliminarAula(int idAula)
        {
            try
            {
                _aulaRepositorie.Borrar(idAula);
                return Ok(true);                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
