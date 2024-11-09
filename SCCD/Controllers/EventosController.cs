﻿using Data.Contracts;
using Data.Migrations;
using Dtos;
using Dtos.Entities;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using SCCD.FacadePattern;
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

        [HttpGet]
        [Route("/[controller]/[action]/")]
        public IActionResult ObtenerEventos()
        {
            try
            {
                Persona personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                if (personaLogueada != null)
                {
                    List<EventoConEnumerables> eventosARetornar = new List<EventoConEnumerables>();
                    List<Evento> eventos = new List<Evento>();
                    if (personaLogueada is Directivo)
                    {
                        eventos.AddRange(_eventoRepositorie.ObtenerEventosDeInstitucion(personaLogueada.Institucion.Id));

                    }
                    else if (personaLogueada is Docente)
                    {
                        eventos.AddRange(_eventoRepositorie.ObtenerEventosConAulaDeDocente(personaLogueada.Id));
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
                    return NotFound("Persona no encontrada");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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
                    && nuevoEvento.IdAulaDestinada > 0
                    && nuevoEvento.IdCreador > 0)
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
                                Asistiran = new List<Persona>(),
                                NoAsistiran = new List<Persona>(),
                                TalVezAsistan = new List<Persona>(),
                                AulaDestinada = aulaDestinada,
                                Creador = creadorDeEvento
                            };
                            eventoAAgregar.Asistiran.Add(creadorDeEvento);
                            if (creadorDeEvento is Directivo)
                            {
                                Persona docenteDeAula = _aulaRepositorie.ObtenerDocenteDeAula(aulaDestinada.Id);
                                eventoAAgregar.Asistiran.Add(docenteDeAula);
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

        [HttpPut]
        [Route("/[controller]/[action]/{id}")]
        public IActionResult ModificarEvento(int id, [FromBody] EventoAModificar eventoAModificar)
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

        [HttpPut]
        [Route("/[controller]/[action]/{idEvento}/{confirmacion}")]
        public IActionResult ConfirmarAsistenciaEvento(int idEvento, string confirmacion, [FromBody] object body)
        {
            try
            {
                if (idEvento != null)
                {
                    Evento evento = _eventoRepositorie.ObtenerAsync(idEvento);
                    if (evento != null)
                    {
                        Persona personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                        if (personaLogueada != null)
                        {
                            if (confirmacion == "Asiste")
                            {
                                evento.Asistiran.Add(personaLogueada);
                                personaLogueada.EventosAsistire.Add(evento);
                            }else if (confirmacion == "No Asiste")
                            {
                                evento.NoAsistiran.Add(personaLogueada);
                                personaLogueada.EventosNoAsistire.Add(evento);
                            }
                            else if (confirmacion == "Tal Vez Asiste")
                            {
                                evento.TalVezAsistan.Add(personaLogueada);
                                personaLogueada.EventosTalVezAsista.Add(evento);
                            }
                            _eventoRepositorie.Modificar(evento);
                            _personaRepositorie.Modificar(personaLogueada);
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


        [HttpDelete]
        [Route("/[controller]/[action]/{idEvento}")]
        public IActionResult EliminarEvento(int idEvento)
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
