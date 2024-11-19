using AutoMapper;
using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos;
using SCCD.Services.Interfaces;

namespace SCCD.Controllers
{
    [ApiController]
    public class PersonasController : Controller
    {    
        private IPersonaRepositorie _personaRepositorie;
        private IUsuarioRepositorie _userRepositorie;
        private IInstitucionRepositorie _institucionRepositorie;
        private IAulaRepositorie _aulaRepositorie;
        private IGrupoRepositorie _gruposRepositorie;
        private IArchivosService _archivosService;
        private readonly IMapper _mapper;
        private Session _session = Session.GetInstance();

        public PersonasController(IPersonaRepositorie personasRepositorie, IUsuarioRepositorie usuarioRepositorie,
            IInstitucionRepositorie institucionRepositorie,IAulaRepositorie aulaRepositorie, 
            IGrupoRepositorie gruposRepositorie, IMapper mapper, IArchivosService archivosService)
        {

            _personaRepositorie = personasRepositorie;
            _userRepositorie = usuarioRepositorie;
            _institucionRepositorie = institucionRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _gruposRepositorie = gruposRepositorie;
            _mapper = mapper;
            _archivosService = archivosService;
        }
        
        [HttpGet]
        [Route("/[controller]/[action]")]
        public IEnumerable<Alumno> ObtenerMisHijos()
        {
            List<Alumno> hijosPadre = new List<Alumno>();
            string IdUserLogueado = _session.IdUserLogueado;
            var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(IdUserLogueado));
            if (personaLogueada!= null)
            {
                foreach (var hijo in _personaRepositorie.ObtenerHijos(personaLogueada.Id))
                {
                    hijosPadre.Add(hijo as Alumno);
                }
                
                return hijosPadre;
            }
            return null;
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAlumno}")]
        public IEnumerable<Persona> ObtenerPadresDeAlumno(Guid idAlumno)
        {
            try
            {
                List<Persona> padres = new List<Persona>();
                var padresDeAlumno = _personaRepositorie.ObtenerPadresDeAlumno(idAlumno);
                if (padresDeAlumno != null && padresDeAlumno.Count() > 0)
                {
                    return padresDeAlumno;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }                        
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerAlumnosSinPadreAsignado()
        {
            try
            {
                var alumnosDelSistema = _personaRepositorie.ObtenerAlumnosSistema();
                var personasDelSistema = _personaRepositorie.ObtenerPersonasSistema();                
                List<Persona> alumnosSinPadreAsignado = new List<Persona>();                           
                bool flag = false;

                if (alumnosDelSistema != null && personasDelSistema != null)
                {
                    foreach (var alumnoSistema in alumnosDelSistema)
                    {
                        var cont = 0;
                        flag = false;
                        foreach (var personaSistema in personasDelSistema)
                        {
                            if (personaSistema is Padre)
                            {
                                flag = ((Padre)personaSistema).Hijos.Any(x => x.Id == alumnoSistema.Id);
                            }else if (personaSistema is Docente)
                            {
                                flag = ((Docente)personaSistema).Hijos.Any(x => x.Id == alumnoSistema.Id);
                            }
                            else if (personaSistema is Directivo)
                            {
                                flag = ((Directivo)personaSistema).Hijos.Any(x => x.Id == alumnoSistema.Id);
                            }

                            if (flag) 
                            {
                                break;
                            }
                            else
                            {
                                cont += 1;
                            }
                        }

                        if (cont == personasDelSistema.Count())
                        {
                            alumnosSinPadreAsignado.Add(alumnoSistema);
                        }
                    }
                    return Ok(alumnosSinPadreAsignado);
                }
                else
                {
                    return NotFound(false);
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(false);           
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{IdPersona}")]
        public IActionResult ObtenerHijosDePersona(Guid IdPersona)
        {
            try
            {                
                List<Persona> hijosDePersona = new List<Persona>();
                var hijosPersona = _personaRepositorie.ObtenerHijos(IdPersona);
                if (hijosPersona != null)
                {
                    hijosDePersona.AddRange(hijosPersona); //devolvemos tambien los hijos, por si hay que desasignarlo de la Persona
                }
                else
                {
                    return NotFound(false);
                }
                return Ok(hijosDePersona);
            }
            catch (Exception ex)
            {
                return BadRequest(false);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerPersonasSistema()
        {
            try
            {
                var personas = _personaRepositorie.ObtenerTodosAsync();
                List<Persona> personasARetornar = new List<Persona>();
                if (personas != null)
                {                    
                    foreach (var persona in personas)
                    {
                        if ((!(persona is Alumno)))
                        {
                            if (persona.Usuario != null && (!persona.Usuario.Grupos.Any(x => x.Tipo == "Admin")))
                            {
                                personasARetornar.Add(persona);
                            }
                            else if (persona.Usuario == null)
                            {
                                personasARetornar.Add(persona);
                            }
                        }                       
                    }
                }
                return Ok(personasARetornar);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult AgregarPersona([FromBody] PersonaAAgregar personaAAgregar)
        {
            try
            {
                if (personaAAgregar != null)
                {
                    var personas = _personaRepositorie.ObtenerTodosAsync();
                    if (personas != null)
                    {
                        if (personas.Any(x => x.Email == personaAAgregar.Email))
                        {
                            return BadRequest("Ya existe una Persona con ese Email registrado");
                        }else if (personas.Any(x => x.DNI == Convert.ToInt32(personaAAgregar.DNI)))
                        {
                            return BadRequest("Ya existe una Persona con ese DNI registrado");
                        }
                    }
                    
                    if (personaAAgregar.Usuario != null && personaAAgregar.TipoPersona != "")
                    {
                        var user = _userRepositorie.ObtenerAsync(personaAAgregar.Usuario.Id);
                        if (user != null)
                        {
                            if (!this.AdministrarGruposUsuario(personaAAgregar.TipoPersona, user))
                            {
                                return BadRequest(false);
                            }
                        }
                        else
                        {
                            return NotFound(false);
                        }
                    }
                   
                    if (personaAAgregar.TipoPersona == "Padre")
                    {                        
                        Padre nuevoPadre = new Padre
                        {
                         Nombre = personaAAgregar.Nombre,
                         Apellido = personaAAgregar.Apellido,
                         DNI = Convert.ToInt32(personaAAgregar.DNI),
                         Email = personaAAgregar.Email,
                         Telefono = personaAAgregar.Telefono,
                         Domicilio = personaAAgregar.Domicilio
                        };
                        if (personaAAgregar.Institucion != null)
                        {
                            var institucion = _institucionRepositorie.ObtenerAsync(personaAAgregar.Institucion.Id);
                            if (institucion != null)
                            {
                                nuevoPadre.Institucion = institucion;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }                        
                        if (personaAAgregar.Usuario != null)
                        {
                            var user = _userRepositorie.ObtenerAsync(personaAAgregar.Usuario.Id);
                            if (user != null)
                            {
                                nuevoPadre.Usuario = user;                                
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        _personaRepositorie.Agregar(nuevoPadre);
                    }
                    else if (personaAAgregar.TipoPersona == "Docente")
                    {
                        Docente nuevaDocente = new Docente
                        {
                            Nombre = personaAAgregar.Nombre,
                            Apellido = personaAAgregar.Apellido,
                            DNI = Convert.ToInt32(personaAAgregar.DNI),
                            Email = personaAAgregar.Email,
                            Telefono = personaAAgregar.Telefono,
                            Domicilio = personaAAgregar.Domicilio
                        };
                        if (personaAAgregar.Usuario != null)
                        {
                            var user = _userRepositorie.ObtenerAsync(personaAAgregar.Usuario.Id);
                            if (user != null)
                            {
                                nuevaDocente.Usuario = user;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        if (personaAAgregar.Institucion != null)
                        {
                            var institucion = _institucionRepositorie.ObtenerAsync(personaAAgregar.Institucion.Id);
                            if (institucion != null)
                            {
                                nuevaDocente.Institucion = institucion;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        _personaRepositorie.Agregar(nuevaDocente);
                    }
                    else if (personaAAgregar.TipoPersona == "Directivo") 
                    {
                        Directivo nuevoDirectivo = new Directivo
                        {
                            Nombre = personaAAgregar.Nombre,
                            Apellido = personaAAgregar.Apellido,
                            DNI = Convert.ToInt32(personaAAgregar.DNI),
                            Email = personaAAgregar.Email,
                            Telefono = personaAAgregar.Telefono,
                            Domicilio = personaAAgregar.Domicilio
                        };
                        if (personaAAgregar.Usuario != null)
                        {
                            var user = _userRepositorie.ObtenerAsync(personaAAgregar.Usuario.Id);
                            if (user != null)
                            {
                                nuevoDirectivo.Usuario = user;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        if (personaAAgregar.Institucion != null)
                        {
                            var institucion = _institucionRepositorie.ObtenerAsync(personaAAgregar.Institucion.Id);
                            if (institucion != null)
                            {
                                nuevoDirectivo.Institucion = institucion;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        _personaRepositorie.Agregar(nuevoDirectivo);
                    }
                    else
                    {
                        return BadRequest(false);
                    }                    
                    return Ok(true);
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [NonAction]
        public bool AdministrarGruposUsuario(string tipoPersonaAAgregar, Usuario usuario)
        {
            try
            {
                if (tipoPersonaAAgregar == "" || usuario == null)
                {
                    return false;
                }
                else
                {
                    var usuarios = _userRepositorie.ObtenerTodosAsync();
                    var user = usuarios.Where(x => x.Email == usuario.Email).FirstOrDefault();
                    if (user != null)
                    {
                        var gruposUserContieneTipo = user.Grupos.Any(x => x.Tipo == tipoPersonaAAgregar);
                        if (!gruposUserContieneTipo)
                        {
                            var grupos = _gruposRepositorie.ObtenerTodosAsync();
                            var grupoAAgregar = grupos.Where(x => x.Tipo == tipoPersonaAAgregar).FirstOrDefault();
                            if (grupoAAgregar != null)
                            {
                                user.Grupos.Add(grupoAAgregar);
                                _userRepositorie.AgregarGrupoDefaultUser(user);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idPersona}")]
        public IActionResult EditarPersona(Guid IdPersona, [FromBody] PersonaModificar personaModificar)
        {
            try
            {
                if (IdPersona != null && IdPersona != Guid.Empty)
                {
                    var personas = _personaRepositorie.ObtenerTodosAsync();
                    if (personas != null)
                    {
                        if (personas.Any(x => x.Id != IdPersona && x.Email == personaModificar.Email))
                        {
                            return BadRequest("Ya existe una Persona con ese Email registrado");
                        }
                        else if (personas.Any(x => x.Id != IdPersona && x.DNI == personaModificar.DNI))
                        {
                            return BadRequest("Ya existe una Persona con ese DNI registrado");
                        }
                    }
                    var persona = _personaRepositorie.ObtenerAsync(IdPersona);
                    if (persona != null)
                    {
                        var hijos = _personaRepositorie.ObtenerHijos(persona.Id);
                        persona.Nombre = personaModificar.Nombre;
                        persona.Apellido = personaModificar.Apellido;
                        persona.DNI = Convert.ToInt32(personaModificar.DNI);
                        persona.Email = personaModificar.Email;
                        persona.Telefono = personaModificar.Telefono;
                        persona.Domicilio = personaModificar.Domicilio;

                        if (personaModificar.UsuarioSeleccionado != null)
                        {
                            var user = _userRepositorie.ObtenerAsync(personaModificar.UsuarioSeleccionado.Id);
                            if (user != null)
                            {
                                persona.Usuario = user;
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        else
                        {
                            persona.Usuario = null;
                        }
                        if (personaModificar.InstitucionSeleccionada != null)
                        {
                            var institucion = _institucionRepositorie.ObtenerAsync(personaModificar.InstitucionSeleccionada.Id);
                            if (institucion != null)
                            {
                                if (persona.Institucion != null && institucion != persona.Institucion)
                                {
                                    persona.Institucion = institucion;
                                }else if (persona.Institucion == null)
                                {
                                    persona.Institucion = institucion;
                                }                                
                            }
                            else
                            {
                                return NotFound(false);
                            }
                        }
                        if (personaModificar.HijosSeleccionados.Count() > 0)
                        {
                            if (hijos != null && hijos.Count() > 0)
                            {
                                foreach (var hijo in hijos)
                                {
                                    if (!personaModificar.HijosSeleccionados.Any(x => x == hijo.Id))
                                    {
                                        this.EliminarAusenciasAlumno(hijo.Id);
                                        this.ResetearFirmaHistorialesAlumno(hijo.Id);
                                        _personaRepositorie.ModificarHijosAsignados(persona, hijo.Id.ToString(), "Desasignar");
                                    }
                                }
                            }
                                
                            foreach (var hijoSeleccionado in personaModificar.HijosSeleccionados)
                            {                                
                                var alumno = _personaRepositorie.GetAlumno(hijoSeleccionado);
                                if (alumno != null)
                                {
                                    _personaRepositorie.ModificarHijosAsignados(persona, alumno.Id.ToString(), "Asignar");
                                }
                                else
                                {
                                    return NotFound(false);
                                }
                            }                           
                        }else if (personaModificar.HijosSeleccionados.Count() == 0)
                        {
                            if (hijos != null && hijos.Count() > 0)
                            {
                                foreach (var hijo in hijos)
                                {
                                    this.EliminarAusenciasAlumno(hijo.Id);
                                    this.ResetearFirmaHistorialesAlumno(hijo.Id);
                                }
                            }

                            if (persona is Padre)
                            {
                                ((Padre)persona).Hijos = new List<Alumno>();
                                _personaRepositorie.ModificarHijosAsignados(persona, "00000000-0000-0000-0000-000000000000", "");
                            }
                            else if (persona is Docente)
                            {
                                ((Docente)persona).Hijos = new List<Alumno>();
                                _personaRepositorie.ModificarHijosAsignados(persona, "00000000-0000-0000-0000-000000000000", "");
                            }
                            else if (persona is Directivo)
                            {
                                ((Directivo)persona).Hijos = new List<Alumno>();
                                _personaRepositorie.ModificarHijosAsignados(persona, "00000000-0000-0000-0000-000000000000", "");
                            }
                        }
                        _personaRepositorie.Modificar(persona);
                        return Ok(true);
                    }
                    else
                    {
                        return NotFound(false);
                    }
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(false);
            }
        }

        [NonAction]
        private Task<bool> EliminarAusenciasAlumno(Guid idHijo)
        {
            var ausenciasHijo = _personaRepositorie.ObtenerAusenciasAlumno(idHijo);
            if (ausenciasHijo != null && ausenciasHijo.Count() > 0)
            {
                foreach (var ausencia in ausenciasHijo)
                {
                    _archivosService.EliminarArchivosAusencia(ausencia.Id);
                }
            }
           return _personaRepositorie.EliminarAusenciasAlumno(idHijo);
        }

        [NonAction]
        private Task<bool> ResetearFirmaHistorialesAlumno(Guid idHijo)
        {
            return _personaRepositorie.ResetearFirmasHistorialesAlumno(idHijo);   
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idAlumno}")]
        public IActionResult EditarAlumno(Guid idAlumno, [FromBody] NuevoAlumno alumnoAEditar)
        {
            try
            {
                if (idAlumno == Guid.Empty || alumnoAEditar == null)
                {
                    return BadRequest(false);
                }
                else
                {
                    var alumno = _personaRepositorie.GetAlumno(idAlumno);
                    if (alumno != null)
                    {
                        alumno.Nombre = alumnoAEditar.Nombre;
                        alumno.Apellido = alumnoAEditar.Apellido;
                        alumno.DNI = alumnoAEditar.DNI;
                        alumno.FechaNacimiento = alumnoAEditar.FechaNacimiento;

                        _personaRepositorie.Modificar(alumno);
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

        [HttpDelete]
        [Route("/[controller]/[action]/{idAlumno}")]
        public IActionResult EliminarAlumno(Guid idAlumno)
        {
            try
            {
                if (idAlumno == Guid.Empty)
                {
                    return BadRequest(false);
                }
                else
                {                                       
                    _personaRepositorie.Borrar(idAlumno);
                    return Ok(true);                                      
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Route("/[controller]/[action]/{idPersona}")]
        public IActionResult EliminarPersona(Guid IdPersona)
        {
            try
            {
                if (IdPersona != null && IdPersona != Guid.Empty)
                {
                    _personaRepositorie.Borrar(IdPersona);
                    return Ok(true);
                }
                else
                {
                    return BadRequest(false);
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }   
    }
}
