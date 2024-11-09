using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos;
using Model.Enums;
using SCCD.FacadePattern;

namespace SCCD.Controllers
{
    [ApiController]
    public class HistorialesController : Controller
    {
        private IPersonaRepositorie _personaRepositorie;
        private IAulaRepositorie _aulaRepositorie;
        private IUsuarioRepositorie _userRepositorie;
        private IHistorialRepositorie _historialRepositorie;
        private IHistorialAuditRepositorie _historialAuditRepositorie;
        private Session _session = Session.GetInstance();
        private readonly Facade _facade;
        public HistorialesController(IPersonaRepositorie personasRepositorie,IAulaRepositorie aulaRepositorie, IUsuarioRepositorie usuarioRepositorie, 
            IHistorialRepositorie historialRepositorie, IHistorialAuditRepositorie historialAuditRepositorie)
        {
            _personaRepositorie = personasRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _userRepositorie = usuarioRepositorie;
            _historialRepositorie = historialRepositorie;
            _historialAuditRepositorie = historialAuditRepositorie;
            _facade = new Facade(_personaRepositorie, _aulaRepositorie);
        }
        [HttpGet]
        [Route("/[controller]/[action]/{idHijo}")]
        public IEnumerable<Historial> ObtenerHistorialesDeAlumno(int idHijo)
        {
            var alumno = _personaRepositorie.GetAlumno(idHijo);
            if (alumno != null && alumno.Historiales != null)
            {
                return alumno.Historiales;
            }

            return null;            
        }

        [HttpPut]
        [Route("/[controller]/[action]/{idHijo}/{idHistorial}")]
        public IActionResult FirmarHistorial(int idHijo, int idHistorial)
        {
            try
            {
                if (idHijo != null && idHijo > 0 && idHistorial != null && idHistorial > 0)
                {
                    var historial = _historialRepositorie.ObtenerAsync(idHistorial);
                    if (historial != null)
                    {
                        _historialRepositorie.FirmarHistorial(historial);
                        _facade.EnviarMailHistorial(historial, idHijo, "firmado");
                        this.NuevaAuditHistorial(historial, "FIRMA");                        
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
                return BadRequest(ex);
            }                       
        }      

        [HttpPost]
        [Route("/[controller]/[action]/{idAlumno}")]
        public IActionResult AgregarHistorial(int idAlumno, [FromBody] HistorialACrear historialAAgregar)
        {
            try
            {
                Historial nuevoHistorial = new Historial();
                if (historialAAgregar != null && idAlumno > 0)
                {
                    nuevoHistorial.Descripcion = historialAAgregar.Descripcion;
                    if (historialAAgregar.Calificacion == 0)
                    {
                        nuevoHistorial.Calificacion = null;
                    }
                    else
                    {
                        nuevoHistorial.Calificacion = historialAAgregar.Calificacion;
                    }
                    switch (historialAAgregar.Estado)
                    {
                        case 1:
                            nuevoHistorial.Estado = Estado.Aprobado;
                            break;
                        case 2:
                            nuevoHistorial.Estado = Estado.NoAprobado;
                            break;
                        case 3:
                            nuevoHistorial.Estado = Estado.Entregado;
                            break;
                        case 4:
                            nuevoHistorial.Estado = Estado.NoEntregado;
                            break;
                        case 7:
                            nuevoHistorial.Estado = Estado.Observacion;
                            break;
                        default:
                            break;
                    }
                    nuevoHistorial.Fecha = DateTime.Now;
                    nuevoHistorial.Firmado = false;

                    _facade.EnviarMailHistorial(nuevoHistorial, idAlumno, "nuevo");
                    _historialRepositorie.Agregar(nuevoHistorial);
                    _personaRepositorie.ActualizarHistorialAlumno(idAlumno, nuevoHistorial);
                    this.NuevaAuditHistorial(nuevoHistorial, "ALTA");
                    return Ok(true);
                }
                else
                {
                    return BadRequest(false);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "A recipient must be specified.")
                {
                    return BadRequest("No puede agregarse el Historial debido a que el Alumno aun no tiene Padre(s) asignado(s)");
                }
                return BadRequest(ex);
            }                        
        }
       

        [HttpPut]
        [Route("/[controller]/[action]/{idAlumno}/{idHistorial}")]
        public IActionResult EditHistorialAlumno(int idAlumno, int idHistorial, [FromBody] HistorialACrear historialAModificar)
        {
            try
            {
                var alumno = _personaRepositorie.GetAlumno(idAlumno);
                if (alumno != null && historialAModificar != null)
                {
                    var historial = alumno.Historiales.FirstOrDefault(x => x.IdHistorial == idHistorial);
                    if (historial != null)
                    {
                        historial.Descripcion = historialAModificar.Descripcion;
                        if (historialAModificar.Calificacion == 0)
                        {
                            historial.Calificacion = null;
                        }
                        else
                        {
                            historial.Calificacion = historialAModificar.Calificacion;
                        }
                        switch (historialAModificar.Estado)
                        {
                            case 1:
                                historial.Estado = Estado.Aprobado;
                                break;
                            case 2:
                                historial.Estado = Estado.NoAprobado;
                                break;
                            case 3:
                                historial.Estado = Estado.Entregado;
                                break;
                            case 4:
                                historial.Estado = Estado.NoEntregado;
                                break;
                            case 7:
                                historial.Estado = Estado.Observacion;
                                break;
                            default:
                                break;
                        }
                        historial.Firmado = false;
                        _personaRepositorie.ActualizarHistorialAlumno(alumno.Id, historial);
                        _historialRepositorie.Modificar(historial);
                        _facade.EnviarMailHistorial(historial, alumno.Id, "modificado");
                        this.NuevaAuditHistorial(historial, "MODIFICACION");
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
                return BadRequest(ex);
            }
                   
        }
        [HttpDelete]
        [Route("/[controller]/[action]/{idHistorial}/{idAlumno}")]
        public IActionResult DeleteHistorial(int IdHistorial, int idAlumno)
        {
            try
            {
                if (idAlumno == null || idAlumno == 0 || IdHistorial == 0 || IdHistorial == null)
                {
                    return NotFound(false);
                }
                var alumno = _personaRepositorie.GetAlumno(idAlumno);
                if (alumno != null)
                {
                    var historial = alumno.Historiales.FirstOrDefault(x => x.IdHistorial == IdHistorial);
                    if (historial != null)
                    {
                        _personaRepositorie.EliminarHistorial(alumno.Id, historial);
                        _historialRepositorie.Borrar(historial.IdHistorial);
                        this.NuevaAuditHistorial(historial, "BAJA");
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
            catch (Exception ex)
            {
                return BadRequest(ex);
            }                        
        }

        [NonAction]
        private IActionResult NuevaAuditHistorial(Historial historial, string accion)
        {
            var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
            if (personaLogueada != null)
            {
                HistorialAudit nuevaAuditHistorial = new HistorialAudit
                {
                    IdPersona = personaLogueada.Id,
                    IdHistorial = historial.IdHistorial,
                    Accion = accion,
                    Fecha = DateTime.Now
                };
                _historialAuditRepositorie.Agregar(nuevaAuditHistorial);
                return Ok(true);
            }
            else
            {
                return NotFound(false);
            }
        }
    }
}
