using Data.Contracts;
using Dtos;
using Dtos.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using SCCD.FacadePattern;
using SCCD.Helpers;
using SCCD.Services.Interfaces;

namespace SCCD.Controllers
{
    [ApiController]
    public class EventosController : Controller
    {
        private IEventoRepositorie _eventoRepositorie;
        private IPersonaRepositorie _personaRepositorie;
        private IAulaRepositorie _aulaRepositorie;
        private IWeatherService _weatherService;
        private Session _session = Session.GetInstance();
        private readonly Facade _facade;

        public EventosController(IEventoRepositorie eventoRepositorie, IPersonaRepositorie personasRepositorie, IAulaRepositorie aulaRepositorie, IWeatherService weatherService)
        {
            _eventoRepositorie = eventoRepositorie;
            _personaRepositorie = personasRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _weatherService = weatherService;
            _facade = new Facade(_personaRepositorie, _aulaRepositorie);
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/[action]/")]
        public IActionResult ObtenerEventos()
        {
            try
            {
                Persona personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(JwtHelper.GetClaimValueFromToken(_session.Token, "userId")));
                if (personaLogueada != null && personaLogueada.Usuario != null && personaLogueada.Usuario.Grupos != null)
                {
                    var tieneRolPadre = personaLogueada.Usuario.Grupos.Any(g => g.Tipo == "Padre");
                    List<EventoConEnumerables> eventosARetornar = new List<EventoConEnumerables>();
                    List<Evento> eventos = new List<Evento>();
                    if (personaLogueada is Directivo)
                    {
                        eventos.AddRange(_eventoRepositorie.ObtenerEventosDeInstitucion(personaLogueada.Institucion.Id));
                        if (tieneRolPadre)
                        {
                            var hijosDirectivo = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                            foreach (var hijo in hijosDirectivo)
                            {
                                var aulaDeHijo = _aulaRepositorie.ObtenerAulaDeHijo(hijo.Id);
                                if (aulaDeHijo != null)
                                {
                                    eventos.AddRange(_eventoRepositorie.ObtenerEventosParaPadre(aulaDeHijo.Id));
                                }
                            }
                        }
                    }
                    else if (personaLogueada is Docente)
                    {
                        eventos.AddRange(_eventoRepositorie.ObtenerEventosConAulaDeDocente(personaLogueada.Id));
                        if (tieneRolPadre)
                        {
                            var hijosDocente = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                            foreach (var hijo in hijosDocente)
                            {
                                var aulaDeHijo = _aulaRepositorie.ObtenerAulaDeHijo(hijo.Id);
                                if (aulaDeHijo != null)
                                {
                                    eventos.AddRange(_eventoRepositorie.ObtenerEventosParaPadre(aulaDeHijo.Id));
                                }
                            }
                        }
                    }
                    else if (personaLogueada is Padre)
                    {
                        var hijosDePadre = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                        if (hijosDePadre != null && hijosDePadre.Count() > 0)
                        {
                            foreach (var hijo in hijosDePadre)
                            {
                                var aulaDeHijo = _aulaRepositorie.ObtenerAulaDeHijo(hijo.Id);
                                if (aulaDeHijo != null)
                                {
                                    var eventosDeAula = _eventoRepositorie.ObtenerEventosParaPadre(aulaDeHijo.Id);
                                    foreach (var evento in eventosDeAula)
                                    {
                                        if (!eventos.Any(x => x == evento))
                                        {
                                            eventos.AddRange(eventosDeAula);
                                        }
                                        break;
                                    }
                                    
                                }
                            }
                        }
                    }

                    if (eventos != null && eventos.Count() > 0)
                    {
                        foreach (var evento in eventos)
                        {
                            var personasAsistiran = _eventoRepositorie.ObtenerPersonasQueAsistiranAlEvento(evento.Id);
                            var personasNoAsistiran = _eventoRepositorie.ObtenerPersonasQueNoAsistiranAlEvento(evento.Id);
                            var personasTalVezAsistan = _eventoRepositorie.ObtenerPersonasQueTalVezAsistanAlEvento(evento.Id);
                            var aulaDestinada = _aulaRepositorie.ObtenerAsync(evento.AulaDestinada.Id);
                            var creadorDeEvento = _personaRepositorie.ObtenerAsync(evento.Creador.Id);
                            if (personasAsistiran != null && personasNoAsistiran != null && personasTalVezAsistan != null && aulaDestinada != null)
                            {
                                EventoConEnumerables eventoARetornar = new EventoConEnumerables
                                {
                                    Id = evento.Id,
                                    Fecha = evento.Fecha,
                                    Localidad = evento.Localidad,
                                    Motivo = evento.Motivo,
                                    Descripcion = evento.Descripcion,
                                    Asistiran = new List<Persona>(personasAsistiran),
                                    NoAsistiran = new List<Persona>(personasNoAsistiran),
                                    TalVezAsistan = new List<Persona>(personasTalVezAsistan),
                                    AulaDestinada = aulaDestinada,
                                    Creador = creadorDeEvento,
                                };
                                eventosARetornar.Add(eventoARetornar);
                            }
                            else
                            {
                                return NotFound("Algunos datos no han sido encontrados");
                            }
                        }
                        return Ok(eventosARetornar);
                    }
                    else
                    {
                        return NoContent();
                    }
                }
                else
                {
                    return NotFound("Persona, Usuario o Grupos de Usuario no encontrados");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult AgregarEvento([FromBody] EventoACrear nuevoEvento)
        {
            try
            {
                if (nuevoEvento != null 
                    && nuevoEvento.Fecha != DateTime.MinValue 
                    && nuevoEvento.Descripcion != "" 
                    && nuevoEvento.Localidad != "" 
                    && nuevoEvento.Motivo != ""
                    && nuevoEvento.IdAulaDestinada != Guid.Empty
                    && nuevoEvento.IdCreador != Guid.Empty)
                {
                    Aula aulaDestinada = _aulaRepositorie.ObtenerAsync(nuevoEvento.IdAulaDestinada);
                    Persona creadorDeEvento = _personaRepositorie.ObtenerAsync(nuevoEvento.IdCreador);
                    IEnumerable<Evento> eventos = _eventoRepositorie.ObtenerTodosAsync();
                    var eventoParaFechaYaExiste = eventos.Any(evento => evento.AulaDestinada.Id == nuevoEvento.IdAulaDestinada
                    && evento.Fecha == nuevoEvento.Fecha);
                    var eventoYaExiste = eventos.Any(evento => evento.AulaDestinada.Id == nuevoEvento.IdAulaDestinada
                    && evento.Motivo == nuevoEvento.Motivo);
                    if (aulaDestinada != null && creadorDeEvento != null)
                    {
                        if (!eventoParaFechaYaExiste && !eventoYaExiste)
                        {
                            Evento eventoAAgregar = new Evento
                            {
                                Fecha = nuevoEvento.Fecha,
                                Descripcion = nuevoEvento.Descripcion,
                                Motivo = nuevoEvento.Motivo,
                                Localidad = nuevoEvento.Localidad,
                                EventoPersonas = new List<EventoPersona>(),
                                AulaDestinada = aulaDestinada,
                                Creador = creadorDeEvento
                            };
                            EventoPersona nuevoEventoCreador = new EventoPersona
                            {
                                Evento = eventoAAgregar,
                                EventoId = eventoAAgregar.Id,
                                Persona = creadorDeEvento,
                                PersonaId = creadorDeEvento.Id,
                                Asistira = true,
                                NoAsistira = false,
                                TalVezAsista = false,
                                FechaConfirmacion = DateTime.Now,
                            };
                            eventoAAgregar.EventoPersonas.Add(nuevoEventoCreador);
                            creadorDeEvento.EventosPersona.Add(nuevoEventoCreador);
                            if (creadorDeEvento is Directivo)
                            {
                                Persona docenteDeAula = _aulaRepositorie.ObtenerDocenteDeAula(aulaDestinada.Id);
                                EventoPersona nuevoEventoDocente = new EventoPersona
                                {
                                    Evento = eventoAAgregar,
                                    EventoId = eventoAAgregar.Id,
                                    Persona = docenteDeAula,
                                    PersonaId = docenteDeAula.Id,
                                    Asistira = true,
                                    NoAsistira = false,
                                    TalVezAsista = false,
                                    FechaConfirmacion = DateTime.Now,
                                };
                                eventoAAgregar.EventoPersonas.Add(nuevoEventoDocente);
                                docenteDeAula.EventosPersona.Add(nuevoEventoDocente);
                            }
                            _eventoRepositorie.Agregar(eventoAAgregar);
                            _facade.EnviarMailNuevoEvento(eventoAAgregar);
                            return Ok(true);
                        }
                        else
                        {
                            return BadRequest("El Aula ya contiene el Evento");
                        }
                    }
                    else {
                        return NotFound("El Aula o el creador del evento no fueron encontrados");
                    }
                }
                else
                {
                    return BadRequest("Falta completar campos del Evento");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [HttpPut]
        [Route("/[controller]/[action]/{id}")]
        public IActionResult ModificarEvento(Guid id, [FromBody] EventoAModificar eventoAModificar)
        {
            try
            {
                if (id != null)
                {
                    IEnumerable<Evento> eventos = _eventoRepositorie.ObtenerTodosAsync();
                    var eventoParaFechaYaExiste = eventos.Any(evento => evento.AulaDestinada.Id == eventoAModificar.IdAulaDestinada
                    && evento.Fecha == eventoAModificar.Fecha && evento.Id != id);
                    var eventoYaExiste = eventos.Any(evento => evento.AulaDestinada.Id == eventoAModificar.IdAulaDestinada
                    && evento.Motivo == eventoAModificar.Motivo && evento.Id != id);
                    if (!eventoParaFechaYaExiste && !eventoYaExiste)
                    {
                        Evento eventoEncontrado = _eventoRepositorie.ObtenerAsync(id);
                        eventoEncontrado.Fecha = eventoAModificar.Fecha;
                        eventoEncontrado.Localidad = eventoAModificar.Localidad;
                        eventoEncontrado.Motivo = eventoAModificar.Motivo;
                        eventoEncontrado.Descripcion = eventoAModificar.Descripcion;
                        if (eventoEncontrado.AulaDestinada.Id != eventoAModificar.IdAulaDestinada)
                        {
                            var aulaDestinada = _aulaRepositorie.ObtenerAsync(eventoAModificar.IdAulaDestinada);
                            if (aulaDestinada != null)
                            {
                                eventoEncontrado.AulaDestinada = aulaDestinada;
                            }
                            else
                            {
                                return NotFound("Aula para evento no encontrada");
                            }
                        }
                        _eventoRepositorie.Modificar(eventoEncontrado);
                        _facade.EnviarMailEventoModificado(eventoEncontrado);
                        return Ok(true);
                    }
                    else
                    {
                        return BadRequest("El Aula ya contiene el Evento");
                    }
                }
                else
                {
                    return BadRequest("El Id no existe");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        [Authorize]
        [HttpPut]
        [Route("/[controller]/[action]/{idEvento}/{confirmacion}")]
        public IActionResult ConfirmarAsistenciaEvento(Guid idEvento, string confirmacion, [FromBody] object body)
        {
            try
            {
                if (idEvento != null)
                {
                    Evento evento = _eventoRepositorie.ObtenerAsync(idEvento);
                    if (evento != null)
                    {
                        Persona personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(JwtHelper.GetClaimValueFromToken(_session.Token, "userId")));
                        if (personaLogueada != null)
                        {
                            if (confirmacion == "Asiste")
                            {
                                EventoPersona nuevoEventoPadre = new EventoPersona
                                {
                                    Evento = evento,
                                    EventoId = evento.Id,
                                    Persona = personaLogueada,
                                    PersonaId = personaLogueada.Id,
                                    Asistira = true,
                                    NoAsistira = false,
                                    TalVezAsista = false,
                                    FechaConfirmacion = DateTime.Now,
                                };
                                evento.EventoPersonas.Add(nuevoEventoPadre);                                
                                personaLogueada.EventosPersona.Add(nuevoEventoPadre);
                                _eventoRepositorie.ModificarEventoPorConfirmacion(nuevoEventoPadre);
                            }
                            else if (confirmacion == "No Asiste")
                            {
                                EventoPersona nuevoEventoPadre = new EventoPersona
                                {
                                    Evento = evento,
                                    EventoId = evento.Id,
                                    Persona = personaLogueada,
                                    PersonaId = personaLogueada.Id,
                                    Asistira = false,
                                    NoAsistira = true,
                                    TalVezAsista = false,
                                    FechaConfirmacion = DateTime.Now,
                                };
                                evento.EventoPersonas.Add(nuevoEventoPadre);
                                personaLogueada.EventosPersona.Add(nuevoEventoPadre);
                                _eventoRepositorie.ModificarEventoPorConfirmacion(nuevoEventoPadre);
                            }
                            else if (confirmacion == "Tal Vez Asiste")
                            {
                                EventoPersona nuevoEventoPadre = new EventoPersona
                                {
                                    Evento = evento,
                                    EventoId = evento.Id,
                                    Persona = personaLogueada,
                                    PersonaId = personaLogueada.Id,
                                    Asistira = false,
                                    NoAsistira = false,
                                    TalVezAsista = true,
                                    FechaConfirmacion = DateTime.Now,
                                };
                                evento.EventoPersonas.Add(nuevoEventoPadre);
                                personaLogueada.EventosPersona.Add(nuevoEventoPadre);
                                _eventoRepositorie.ModificarEventoPorConfirmacion(nuevoEventoPadre);
                            }
                            _facade.EnviarMailConfirmacionAsistenciaEvento(evento, confirmacion);
                            return Ok(true);
                        }
                        else
                        {
                            return BadRequest("Persona no encontrada");
                        }
                    }
                    else
                    {
                        return BadRequest("Evento no encontrado");
                    }
                }
                else
                {
                    return BadRequest("Id de Evento no encontrado");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("/[controller]/[action]/{localidad}/{fecha}")]
        public async Task<IActionResult> ChequearClima(string localidad, string fecha)
        {
            try
            {
                var (isSuccess, message, data) = await _weatherService.ObtenerClima(localidad, fecha);
                if (!isSuccess)
                {
                    return BadRequest(message);
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("/[controller]/[action]/{idEvento}")]
        public IActionResult EliminarEvento(Guid idEvento)
        {
            try
            {
                if (idEvento != null)
                {
                    _eventoRepositorie.Borrar(idEvento);
                    return Ok(true);
                }
                else
                {
                    return BadRequest("Id no encontrado");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
