using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos.Entities.Reportes;

namespace SCCD.Controllers
{
    [ApiController]
    public class ReportesController : Controller
    {

        private IReportesRepositorie _reportesRepositorie;
        private IPersonaRepositorie _personaRepositorie;
        private ILoginAuditRepositorie _loginAuditRepositorie;
        private IAulaRepositorie _aulasRepositorie;
        private IHistorialAuditRepositorie _historialesAuditRepositorie;
        private INotaRepositorie _notasRepositorie;
        private Session _session = Session.GetInstance();

        public ReportesController(IReportesRepositorie reportesRepositorie, IPersonaRepositorie personaRepositorie, ILoginAuditRepositorie loginAuditRepositorie,
            IAulaRepositorie aulasRepositorie, IHistorialAuditRepositorie historialesAuditRepositorie, INotaRepositorie notasRepositorie)
        {
            _reportesRepositorie = reportesRepositorie;
            _personaRepositorie = personaRepositorie;
            _loginAuditRepositorie = loginAuditRepositorie;
            _aulasRepositorie = aulasRepositorie;
            _historialesAuditRepositorie = historialesAuditRepositorie;
            _notasRepositorie = notasRepositorie;
        }

        [HttpGet]
        [Route("/[controller]/[action]")] 
        public IActionResult ObtenerLogInsAverage()
        {
            try
            {
                int directivoCounter = 0;
                int docenteCounter = 0;
                int padreCounter = 0;

                double directivoSessionTimeCounter = 0;
                double docenteSessionTimeCounter = 0;
                double padreSessionTimeCounter = 0;


                decimal directivoAvg = 0;
                decimal docenteAvg = 0;
                decimal padreAvg = 0;

                double directivoSessionTimeAvg = 0;
                double docenteSessionTimeAvg = 0;
                double padreSessionTimeAvg = 0;

                var loginsCurrentMonth = _loginAuditRepositorie.ObtenerLoginsDelMes();
                if (loginsCurrentMonth != null)
                {
                    foreach (var login in loginsCurrentMonth)
                    {
                        var persona = _personaRepositorie.ObtenerPersonaDeUsuario(login.UsuarioLogueado.Id);
                        if (persona != null)
                        {
                            DateTime defaultDateTime;
                            if (persona is Directivo)
                            {
                                directivoCounter += 1;
                                if (DateTime.TryParseExact("0001-01-01 00:00:00.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", null, System.Globalization.DateTimeStyles.None, out defaultDateTime))
                                {
                                    DateTime defaultDateTimeMidnight = DateTime.MinValue.Date;
                                    if (login.FechaYHoraLogout != defaultDateTimeMidnight)
                                    {
                                        directivoSessionTimeCounter += (login.FechaYHoraLogout - login.FechaYHoraLogin).TotalMinutes;
                                    }
                                }                                                         
                            }else if (persona is Docente)
                            {
                                docenteCounter += 1;
                                if (DateTime.TryParseExact("0001-01-01 00:00:00.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", null, System.Globalization.DateTimeStyles.None, out defaultDateTime))
                                {
                                    DateTime defaultDateTimeMidnight = DateTime.MinValue.Date;
                                    if (login.FechaYHoraLogout != defaultDateTimeMidnight)
                                    {
                                        docenteSessionTimeCounter += (login.FechaYHoraLogout - login.FechaYHoraLogin).TotalMinutes;
                                    }
                                }                                
                            }
                            else if(persona is Padre)
                            {
                                padreCounter += 1;
                                if (DateTime.TryParseExact("0001-01-01 00:00:00.0000000", "yyyy-MM-dd HH:mm:ss.fffffff", null, System.Globalization.DateTimeStyles.None, out defaultDateTime))
                                {
                                    DateTime defaultDateTimeMidnight = DateTime.MinValue.Date;
                                    if (login.FechaYHoraLogout != defaultDateTimeMidnight)
                                    {
                                        padreSessionTimeCounter += (login.FechaYHoraLogout - login.FechaYHoraLogin).TotalMinutes;
                                    }
                                }                                
                            }
                        }
                        else
                        {
                            return NotFound(false);
                        }
                    }
                    int cantidadDias = loginsCurrentMonth.Select(record => record.FechaYHoraLogin.Date).Distinct().Count();

                    directivoAvg = directivoCounter / cantidadDias;
                    docenteAvg = docenteCounter / cantidadDias;
                    padreAvg = padreCounter / cantidadDias;

                    if (directivoCounter > 0)
                    {
                        directivoSessionTimeAvg = directivoSessionTimeCounter / directivoCounter;
                    }
                    if (docenteCounter > 0)
                    {
                        docenteSessionTimeAvg = docenteSessionTimeCounter / docenteCounter;
                    }
                    if (padreCounter > 0)
                    {
                        padreSessionTimeAvg = padreSessionTimeCounter / padreCounter;
                    }

                    LoginsAvg loginAvgs = new LoginsAvg();

                    loginAvgs.LoginsAvgs = new List<decimal>();
                    loginAvgs.SessionTimeAvgs = new List<double>();

                    loginAvgs.LoginsAvgs.Add(directivoAvg);
                    loginAvgs.LoginsAvgs.Add(docenteAvg);
                    loginAvgs.LoginsAvgs.Add(padreAvg);

                    loginAvgs.SessionTimeAvgs.Add(directivoSessionTimeAvg);
                    loginAvgs.SessionTimeAvgs.Add(docenteSessionTimeAvg);
                    loginAvgs.SessionTimeAvgs.Add(padreSessionTimeAvg);

                    return Ok(loginAvgs);
                }
                else
                {
                    return NotFound(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerAsistenciasPorAulaAverage()
        {
            try
            {
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                if (personaLogueada != null && personaLogueada.Institucion != null)
                {                    
                    List<AulasAvg> asistenciasPorAulaAvgs = new List<AulasAvg>();
                    
                    var aulasInstitucion = _aulasRepositorie.ObtenerAulasDeInstitucion(personaLogueada.Institucion.Id);
                    foreach (var aula in aulasInstitucion)
                    {
                        decimal porcentajesAsistenciasCounter = 0;
                        if (aula.Alumnos != null && aula.Alumnos.Count() > 0)
                        {
                            AulasAvg asistenciasPorAulaAvg = new AulasAvg();
                            foreach (var alumnoDeAula in aula.Alumnos)
                            {
                                porcentajesAsistenciasCounter += alumnoDeAula.Asistencia;
                            }
                            decimal asistenciaAulaAvg = 0;
                            asistenciaAulaAvg = porcentajesAsistenciasCounter / aula.Alumnos.Count();
                            asistenciasPorAulaAvg.NombreAula = aula.Nombre;
                            asistenciasPorAulaAvg.Avg = asistenciaAulaAvg;
                            asistenciasPorAulaAvgs.Add(asistenciasPorAulaAvg);
                        }                        
                    }
                    return Ok(asistenciasPorAulaAvgs);
                }
                else
                {
                    return NotFound(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerCondicionPorAulaAverage()
        {
            try
            {
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                if (personaLogueada != null && personaLogueada.Institucion != null)
                {
                    List<AulasAvg> condicionesPorAulaAvgs = new List<AulasAvg>();

                    var aulasInstitucion = _aulasRepositorie.ObtenerAulasDeInstitucion(personaLogueada.Institucion.Id);
                    foreach (var aula in aulasInstitucion)
                    {
                        decimal? condicionesAulasCounter = 0;
                        if (aula.Alumnos != null && aula.Alumnos.Count() > 0)
                        {
                            AulasAvg condicionesPorAulaAvg = new AulasAvg();
                            foreach (var alumnoDeAula in aula.Alumnos)
                            {
                                if (alumnoDeAula.Historiales != null && alumnoDeAula.Historiales.Count() > 0)
                                {
                                    decimal? alumnoCalificacionesCounter = 0;
                                    foreach (var historialAlumno in alumnoDeAula.Historiales)
                                    {
                                        if (historialAlumno.Calificacion != null)
                                        {
                                            alumnoCalificacionesCounter += historialAlumno.Calificacion;
                                        }
                                        else
                                        {
                                            if (historialAlumno.Estado == Model.Enums.Estado.Aprobado 
                                                || historialAlumno.Estado == Model.Enums.Estado.Entregado)
                                            {
                                                alumnoCalificacionesCounter += 6;
                                            }else if (historialAlumno.Estado == Model.Enums.Estado.NoAprobado
                                                || historialAlumno.Estado == Model.Enums.Estado.NoEntregado)
                                            {
                                                alumnoCalificacionesCounter += 4;
                                            }
                                        }                                       
                                    }
                                    decimal? promedioAlumno = alumnoCalificacionesCounter / alumnoDeAula.Historiales.Count();
                                    condicionesAulasCounter += promedioAlumno;                                    
                                }                                
                            }
                            decimal? condicionFinalAulaAvg = condicionesAulasCounter / aula.Alumnos.Count();
                            condicionesPorAulaAvg.NombreAula = aula.Nombre;
                            condicionesPorAulaAvg.Avg = condicionFinalAulaAvg;
                            condicionesPorAulaAvgs.Add(condicionesPorAulaAvg);
                        }
                    }
                    return Ok(condicionesPorAulaAvgs);
                }
                else
                {
                    return NotFound(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerAuditoriasHistoriales()
        {
            try
            {
                var auditoriasHistoriales = _historialesAuditRepositorie.ObtenerTodosAsync();
                if (auditoriasHistoriales != null)
                {
                    DataAuditHistorial dataAuditHistorial = new DataAuditHistorial();
                    foreach (var audit in auditoriasHistoriales)
                    {
                        if (audit.Accion == "ALTA")
                        {
                            dataAuditHistorial.CantidadAltas = dataAuditHistorial.CantidadAltas + 1;
                        }else if (audit.Accion == "BAJA")
                        {
                            dataAuditHistorial.CantidadBajas = dataAuditHistorial.CantidadBajas + 1;
                        }
                        else if (audit.Accion == "MODIFICACION")
                        {
                            dataAuditHistorial.CantidadModificaciones = dataAuditHistorial.CantidadModificaciones + 1;
                        }
                        else if (audit.Accion == "FIRMA")
                        {
                            dataAuditHistorial.CantidadFirmas = dataAuditHistorial.CantidadFirmas + 1;
                        }
                    }
                    return Ok(dataAuditHistorial);
                }
                else
                {
                    return NotFound(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        public IActionResult ObtenerNotasEnviadasYRecibidasAverage()
        {
            try
            {
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                if (personaLogueada != null)
                {
                    var personasInstitucion = _personaRepositorie.ObtenerPadresDocentesDirectivosInstitucion(personaLogueada.Institucion.Id);
                    if (personasInstitucion != null)
                    {
                        NotasAvg notasAvg = new NotasAvg();

                        var directivoCounter = 0;
                        var docenteCounter = 0;
                        var padreCounter = 0;

                        var sumatoriaNotasRecibidasDirectivo = 0;
                        var sumatoriaNotasRecibidasDocente = 0;
                        var sumatoriaNotasRecibidasPadre = 0;

                        var sumatoriaNotasEmitidasDirectivo = 0;
                        var sumatoriaNotasEmitidasDocente = 0;
                        var sumatoriaNotasEmitidasPadre = 0;

                        var notasRecibidasAvgDirectivo = 0;
                        var notasRecibidasAvgDocente = 0;
                        var notasRecibidasAvgPadre = 0;

                        var notasEmitidasAvgDirectivo = 0;
                        var notasEmitidasAvgDocente = 0;
                        var notasEmitidasAvgPadre = 0;

                        foreach (var persona in personasInstitucion)
                        {
                            var notasEmitidasPorPersona = _notasRepositorie.GetNotasEmitidasPersona(persona.Id);
                            var notasRecibidasPorPersona = _notasRepositorie.GetNotasRecibidasPersona(persona.Id);

                            if (persona is Directivo)
                            {
                                directivoCounter += 1;
                                sumatoriaNotasRecibidasDirectivo += notasRecibidasPorPersona.Count();
                                sumatoriaNotasEmitidasDirectivo += notasEmitidasPorPersona.Count();
                            }else if (persona is Docente)
                            {
                                docenteCounter += 1;
                                sumatoriaNotasRecibidasDocente += notasRecibidasPorPersona.Count();
                                sumatoriaNotasEmitidasDocente += notasEmitidasPorPersona.Count();
                            }else if (persona is Padre)
                            {
                                padreCounter += 1;
                                sumatoriaNotasRecibidasPadre += notasRecibidasPorPersona.Count();
                                sumatoriaNotasEmitidasPadre += notasEmitidasPorPersona.Count();
                            }
                        }

                        if (directivoCounter > 0)
                        {
                            notasRecibidasAvgDirectivo = sumatoriaNotasRecibidasDirectivo / directivoCounter;
                            notasEmitidasAvgDirectivo = sumatoriaNotasEmitidasDirectivo / directivoCounter;
                        }
                        if (docenteCounter > 0)
                        {
                            notasRecibidasAvgDocente = sumatoriaNotasRecibidasDocente / docenteCounter;
                            notasEmitidasAvgDocente = sumatoriaNotasEmitidasDocente / docenteCounter;
                        }
                        if (padreCounter > 0)
                        {
                            notasRecibidasAvgPadre = sumatoriaNotasRecibidasPadre / padreCounter;
                            notasEmitidasAvgPadre = sumatoriaNotasEmitidasPadre / padreCounter;
                        }

                        notasAvg.DirectivosRecibidasAvg = notasRecibidasAvgDirectivo;
                        notasAvg.DirectivosEmitidasAvg = notasEmitidasAvgDirectivo;
                        notasAvg.DocentesRecibidasAvg = notasRecibidasAvgDocente;
                        notasAvg.DocentesEmitidasAvg = notasEmitidasAvgDocente;
                        notasAvg.PadresRecibidasAvg = notasRecibidasAvgPadre;
                        notasAvg.PadresEmitidasAvg = notasEmitidasAvgPadre;

                        return Ok(notasAvg);
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
                return BadRequest(ex.Message);
            }
        }
    }
}
