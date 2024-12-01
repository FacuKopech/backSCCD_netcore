using AutoMapper;
using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos;
using Model.Observer;
using SCCD.FacadePattern;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SCCD.Controllers
{
    [ApiController]
    public class NotasController : Controller
    {
        private INotaRepositorie _notaRepositorie;
        private IPersonaRepositorie _personaRepositorie;
        private IAulaRepositorie _aulaRepositorie;
        private IUsuarioRepositorie _userRepositorie;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHost;
        private Session _session = Session.GetInstance();
        private readonly Facade _facade;        

        public NotasController(INotaRepositorie notaRepositorie, IUsuarioRepositorie usuarioRepositorie, 
            IPersonaRepositorie personaRepositorie, IMapper mapper, IAulaRepositorie aulaRepositorie, IWebHostEnvironment webHost)
        {

            _notaRepositorie = notaRepositorie;
            _userRepositorie = usuarioRepositorie;
            _personaRepositorie = personaRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _mapper = mapper;
            _aulaRepositorie = aulaRepositorie;
            _webHost = webHost;
            _facade = new Facade(_webHost, _personaRepositorie, _aulaRepositorie);
        }
        [HttpGet]
        [Route("/[controller]/[action]")]
        // GET: Notas
        public IEnumerable<NotaConEnumerables> IndexNotasRecibidas()
        {
            try
            {
                List<NotaConEnumerables> notasARetornar = new List<NotaConEnumerables>();
                var id = Guid.Parse(_session.IdUserLogueado);
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(id);
                var notas = _notaRepositorie.GetNotasRecibidasYFirmadas(personaLogueada.Id);
                
                if (notas.NotasRecibidas != null && notas.NotasRecibidas.Count() > 0)
                {
                    foreach (var notaRecibida in notas.NotasRecibidas)
                    {
                        var leida = notaRecibida.NotaPersonas.Where(x => x.NotaId == notaRecibida.Id && x.PersonaId == personaLogueada.Id).FirstOrDefault().Leida == true ? true : false;
                        NotaConEnumerables notaARetornar = new NotaConEnumerables {
                            Id = notaRecibida.Id,
                            Titulo = notaRecibida.Titulo,
                            Fecha = notaRecibida.Fecha,
                            Leida = leida,
                            Emisor = notaRecibida.Emisor,
                            Destinatarios = new List<Persona>(notaRecibida.NotaPersonas.Select(persona => persona.Persona).ToList()),
                            LeidaPor = new List<Persona>(notaRecibida.NotaPersonas.Where(nota => nota.Leida == true).Select(persona => persona.Persona).ToList()),
                            FirmadaPor = new List<Persona>(notaRecibida.NotaPersonas.Where(nota => nota.Firmada == true).Select(persona => persona.Persona).ToList()),
                            Cuerpo = notaRecibida.Cuerpo,
                            Tipo = notaRecibida.Tipo,
                            Referido = notaRecibida.Referido,
                            AulasDestinadas = new List<Aula>(notaRecibida.AulasDestinadas)
                        };
                        notasARetornar.Add(notaARetornar);
                    }
                    return notasARetornar;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        [HttpGet]
        [Route("/[controller]/[action]")]
        public IEnumerable<NotaConEnumerables> IndexNotasEmitidas() //Cambio ICollection a IEnumerable para el Front
        {            
            var id = Guid.Parse(_session.IdUserLogueado);
            var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(id);
            var notasEmitidas = _notaRepositorie.GetNotasEmitidasPersona(personaLogueada.Id);
            ICollection<NotaConEnumerables> notasEmitidasConEnumerables = new List<NotaConEnumerables>();

            foreach (var nota in notasEmitidas)
            {
                NotaConEnumerables notaEmitida = new NotaConEnumerables
                {
                    Id = nota.Id,
                    Titulo = nota.Titulo,
                    Fecha = nota.Fecha,
                    Leida = false,
                    Emisor = nota.Emisor,
                    Destinatarios = new List<Persona>(nota.NotaPersonas.Select(persona => persona.Persona).ToList()),
                    LeidaPor = new List<Persona>(nota.NotaPersonas.Where(nota => nota.Leida == true).Select(persona => persona.Persona).ToList()),
                    FirmadaPor = new List<Persona>(nota.NotaPersonas.Where(nota => nota.Firmada == true).Select(persona => persona.Persona).ToList()),
                    Cuerpo = nota.Cuerpo,
                    Tipo = nota.Tipo,
                    Referido = nota.Referido,
                    AulasDestinadas = new List<Aula>(nota.AulasDestinadas)                    
                };
                notasEmitidasConEnumerables.Add(notaEmitida);
            }
            IEnumerable<NotaConEnumerables> notasARetornar = notasEmitidasConEnumerables;

            return notasARetornar;
            
        }

        [HttpPut]
        [Route("/[controller]/[action]/{id}")]        
        public bool LeerNota(Guid id)
        {
            if (id == null || _notaRepositorie.ObtenerTodosAsync() == null)
            {
                return false;
            }

            var nota = _notaRepositorie.ObtenerAsync(id);
            if (nota == null)
            {
                return false;
            }

            string IdUserLogueado = _session.IdUserLogueado;
            var idUser = Guid.Parse(IdUserLogueado);
            var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(idUser);
            var notaPersona = nota.NotaPersonas.Where(x => x.NotaId == nota.Id && x.PersonaId == personaLogueada.Id).FirstOrDefault();
            //subscribimos observers a observable           
            if (!notaPersona.Leida)
            {
                NotaObservable notaObservable = new NotaObservable(notaPersona);
                notaObservable.Attach(new EmisorNota(nota.Emisor.Email, personaLogueada.Email));
                notaObservable.Leida = true;
                _notaRepositorie.ActualizarNotaLeida(nota, personaLogueada.Email);
                return true;
            }
            return false;
            
        }
        [HttpPut]
        [Route("/[controller]/[action]/{idNota}")]        
        public IActionResult FirmarNota(Guid idNota)
        {
            try
            {
                if (idNota != null && idNota != Guid.Empty)
                {
                    var nota = _notaRepositorie.ObtenerAsync(idNota);
                    if (nota != null)
                    {
                        _notaRepositorie.FirmaDeNota(nota, _session.EmailUserLogueado);
                        _facade.EnviarMailNotaFirmada(nota.Titulo, nota.Emisor.Email);
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

        [HttpGet]
        [Route("/[controller]/[action]/{tipoDeNota}/{isPadre}/{esPadreYAlgoMas}/{enviaNotaComoPadre}")]
        public IEnumerable<Aula> ObtenerAulasParaNuevaNota(string tipoDeNota, bool isPadre, bool esPadreYAlgoMas, bool enviaNotaComoPadre)
        {            
            try
            {
                string IdUserLogueado = _session.IdUserLogueado;
                var id = Guid.Parse(IdUserLogueado);
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(id);                
                List<Aula> aulasAUsarNuevaNota = new List<Aula>();
                if (personaLogueada.Usuario.Grupos != null && personaLogueada.Usuario.Grupos.Count() > 0)
                {
                    if (tipoDeNota == "G")
                    {
                        if ((isPadre && esPadreYAlgoMas && enviaNotaComoPadre) || (isPadre && !esPadreYAlgoMas && !enviaNotaComoPadre))
                        {
                            var hijosDePadre = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                            foreach (var hijo in hijosDePadre)
                            {
                                var aulaDeHijo = _aulaRepositorie.ObtenerAulaDeAlumno(hijo.Id);
                                if (aulaDeHijo != null)
                                {
                                    if (aulasAUsarNuevaNota.Where(x => x.Id == aulaDeHijo.Id).FirstOrDefault() == null)
                                    {
                                        aulasAUsarNuevaNota.Add(aulaDeHijo);
                                    }
                                }
                            }
                        }else if (isPadre && esPadreYAlgoMas && !enviaNotaComoPadre)
                        {
                            var grupoAdicional = personaLogueada.Usuario.Grupos.Where(x => x.Tipo != "Padre" && x.Tipo != "UserRegular").FirstOrDefault();
                            if (grupoAdicional != null)
                            {
                                if (grupoAdicional.Tipo == "Docente")
                                {
                                    var aulasDeDocente = _aulaRepositorie.ObtenerAulasDocente(personaLogueada.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasDeDocente);
                                }
                                else if (grupoAdicional.Tipo == "Directivo")
                                {
                                    var aulasInstitucionDirectivo = _aulaRepositorie.ObtenerAulasDeInstitucion(personaLogueada.Institucion.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasInstitucionDirectivo);
                                }
                            }
                        }else if (!isPadre && !esPadreYAlgoMas && !enviaNotaComoPadre)
                        {
                            var grupo = personaLogueada.Usuario.Grupos.Where(x => x.Tipo != "UserRegular").FirstOrDefault();
                            if (grupo != null)
                            {
                                if (grupo.Tipo == "Docente")
                                {
                                    var aulasDeDocente = _aulaRepositorie.ObtenerAulasDocente(personaLogueada.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasDeDocente);
                                }
                                else if (grupo.Tipo == "Directivo")
                                {
                                    var aulasInstitucionDirectivo = _aulaRepositorie.ObtenerAulasDeInstitucion(personaLogueada.Institucion.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasInstitucionDirectivo);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (isPadre && esPadreYAlgoMas && !enviaNotaComoPadre)
                        {
                            var grupoAdicional = personaLogueada.Usuario.Grupos.Where(x => x.Tipo != "Padre" && x.Tipo != "UserRegular").FirstOrDefault();
                            if (grupoAdicional != null)
                            {
                                if (grupoAdicional.Tipo == "Docente")
                                {
                                    var aulasDeDocente = _aulaRepositorie.ObtenerAulasDocente(personaLogueada.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasDeDocente);
                                }
                                else if (grupoAdicional.Tipo == "Directivo")
                                {
                                    var aulasInstitucionDirectivo = _aulaRepositorie.ObtenerAulasDeInstitucion(personaLogueada.Institucion.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasInstitucionDirectivo);
                                }
                            }
                        }else if (!isPadre)
                        {
                            var grupoAdicional = personaLogueada.Usuario.Grupos.Where(x => x.Tipo != "UserRegular").FirstOrDefault();
                            if (grupoAdicional != null)
                            {
                                if (grupoAdicional.Tipo == "Docente")
                                {
                                    var aulasDeDocente = _aulaRepositorie.ObtenerAulasDocente(personaLogueada.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasDeDocente);
                                }
                                else if (grupoAdicional.Tipo == "Directivo")
                                {
                                    var aulasInstitucionDirectivo = _aulaRepositorie.ObtenerAulasDeInstitucion(personaLogueada.Institucion.Id);
                                    aulasAUsarNuevaNota = new List<Aula>(aulasInstitucionDirectivo);
                                }
                            }
                        }
                    }
                    return aulasAUsarNuevaNota;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAula}/{enviaNotaComo}")]
        public IEnumerable<Persona> ObtenerListaDeDestinatariosParaNuevaNota(Guid idAula, string enviaNotaComo)
        {
            try
            {
                List<Persona> destinatarios = new List<Persona>();
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                if (personaLogueada != null && idAula != Guid.Empty)
                {
                    if (enviaNotaComo == "Padre")
                    {
                        var aula = _aulaRepositorie.ObtenerAsync(idAula);
                        if (aula != null)
                        {
                            if (aula.Docente != null)
                            {
                                destinatarios.Add(aula.Docente);
                                var directivo = _personaRepositorie.ObtenerDirectivoInstitucion(aula.Institucion.Id);
                                if (directivo != null)
                                {
                                    destinatarios.Add(directivo);
                                }
                                var personaLogueadaEnDestinatarios = destinatarios.Where(x => x.Email == _session.EmailUserLogueado).FirstOrDefault();
                                if (personaLogueadaEnDestinatarios != null)
                                {
                                    destinatarios.Remove(personaLogueadaEnDestinatarios);
                                }
                                return destinatarios;
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
                    }else if (enviaNotaComo == "Docente")
                    {
                        var aula = _aulaRepositorie.ObtenerAsync(idAula);
                        if (aula != null && aula.Alumnos.Count() > 0 && aula.Alumnos != null)
                        {
                            foreach (var alumno in aula.Alumnos)
                            {
                                var padresAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumno.Id);
                                if (padresAlumno != null && padresAlumno.Count() > 0)
                                {
                                    foreach (var padre in padresAlumno)
                                    {
                                        var padreYaAgregado = destinatarios.Where(x => x.Id == padre.Id).FirstOrDefault();
                                        if (padreYaAgregado == null)
                                        {
                                            destinatarios.Add(padre);
                                        }
                                    }                                    
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            var directivoInstitucion = _personaRepositorie.ObtenerDirectivoInstitucion(aula.Institucion.Id);
                            if (directivoInstitucion != null)
                            {
                                destinatarios.Add(directivoInstitucion);
                            }
                            var personaLogueadaEnDestinatarios = destinatarios.Where(x => x.Email == _session.EmailUserLogueado).FirstOrDefault();
                            if (personaLogueadaEnDestinatarios != null)
                            {
                                destinatarios.Remove(personaLogueadaEnDestinatarios);
                            }
                            return destinatarios;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        var aula = _aulaRepositorie.ObtenerAsync(idAula);
                        if (aula != null)
                        {
                            if (aula.Alumnos != null && aula.Alumnos.Count() > 0)
                            {
                                foreach (var alumno in aula.Alumnos)
                                {
                                    var padresAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumno.Id);
                                    if (padresAlumno != null && padresAlumno.Count() > 0)
                                    {
                                        foreach (var padre in padresAlumno)
                                        {
                                            var padreYaAgregado = destinatarios.Where(x => x.Id == padre.Id).FirstOrDefault();
                                            if (padreYaAgregado == null)
                                            {
                                                destinatarios.Add(padre);
                                            }
                                        }
                                    }
                                }
                            }
                            if (aula.Docente != null)
                            {
                                destinatarios.Add(aula.Docente);
                            }
                            else
                            {
                                return null;
                            }
                            var personaLogueadaEnDestinatarios = destinatarios.Where(x => x.Email == _session.EmailUserLogueado).FirstOrDefault();
                            if (personaLogueadaEnDestinatarios != null)
                            {
                                destinatarios.Remove(personaLogueadaEnDestinatarios);
                            }
                            return destinatarios;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return null;
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
        [Route("/[controller]/[action]/")]
        public IEnumerable<Alumno> ObtenerHijosPadreParaNuevaNota()
        {
            try
            {
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                if (personaLogueada != null)
                {
                    var hijosPadreLogueado = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                    if (hijosPadreLogueado.Count() > 0 && hijosPadreLogueado != null)
                    {
                        return (List<Alumno>)hijosPadreLogueado;
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAula}")]
        public IEnumerable<Alumno> ObtenerAlumnosDeAulaParaNuevaNota(Guid idAula)
        {            
            var alumnos = _aulaRepositorie.ObtenerAlumnosAula(idAula);
            if (alumnos != null && alumnos.Count() > 0)
            {
                return alumnos;
            }
            else
            {
                return null;
            }
        }
        

        //ESCENARIOS PARA EL ENVIO DE NOTAS
        //Solo hay 3 posibles tipos de notas a enviar:

        //1. GENERICA con AULA DESTINADA: una nota para el docente del aula y todos los padres de los alumnos de dicha aula, el sistema ya sabe quienes son los destinatarios
        //2. GENERICA sin AULA DESTINADA: una nota para cualquier persona dentro de la institucion, el usuario elige los destinatarios
        //3. PARTICULAR con AULA DESTINADA: si la nota es PARTICULAR si o si hay AULA DESTINADA, porque se habla de un ALUMNO. El sistema ya sabe quienes son los destinatarios

        //Si se loguea un DIRECTIVO, puede crear cualquiera de los 3 tipos de nota
        //Si se loguea un DOCENTE, puede crear cualquiera de los 3 tipos de nota
        //Si se loguea un PADRE, puede crear los tipos de nota 2 y 3

        //De esta manera, el DIRECTIVO es el unico que puede enviar una nota a las aulas particulares, 
        //y las aulas se comunican directamente con el directivo, no las aulas entre si. Y si el DOCENTE quiere enviar una nota de tipo 1, el AULA DESTINADA no va a ser cualquiera dentro de la institucion como lo es para el DIRECTIVO,
        // sino para las AULAS que ese DOCENTE tiene a cargo. 
        //Ademas, si un DOCENTE o PADRE quiere mandar una nota de tipo 2, elige los destinatarios. Los destinatarios
        //que les van a ser disponibles depende:
        //PARA EL PADRE: sera el directivo y las docentes que esten a cargo de las aulas de sus hijos
        //PARA EL DOCENTE: sera el directivo y los padres de los alumnnos que esten en las aulas que ese docente 
        //tiene a cargo.
        //Si un DIRECTIVO envia una nota de tipo 2, puede elegir a quien sea dentro de la institucion, independientemente
        //del aula.

        [HttpPost]
        [Route("/[controller]/[action]/")]        
        public IActionResult EnviarNuevaNota([FromBody] NotaACrear nuevaNota)
        {
            try
            {
                if (nuevaNota.Tipo != "" && nuevaNota.Titulo != "" && nuevaNota.Cuerpo != "")
                {
                    var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                    if (personaLogueada != null)
                    {
                        if (nuevaNota.EnviaNotaComo == "Padre")
                        {
                            if (nuevaNota.Tipo == "P")
                            {
                                Alumno hijoReferido = null;
                                Aula aulaDestinada = null;
                                Nota notaAAgregar = new Nota
                                {
                                    Titulo = nuevaNota.Titulo,
                                    Fecha = DateTime.Today,
                                    Emisor = personaLogueada,
                                    NotaPersonas = new List<NotaPersona>(),
                                    Cuerpo = nuevaNota.Cuerpo,
                                    Tipo = Model.Enums.TipoNota.Particular,
                                };
                                hijoReferido = _personaRepositorie.GetAlumno(nuevaNota.IdAlumnoReferido);
                                if (hijoReferido != null)
                                {
                                    notaAAgregar.Referido = hijoReferido;
                                    aulaDestinada = _aulaRepositorie.ObtenerAulaDeAlumno(hijoReferido.Id);
                                    if (aulaDestinada != null)
                                    {
                                        notaAAgregar.AulasDestinadas = new List<Aula>();
                                        notaAAgregar.AulasDestinadas.Add(aulaDestinada);
                                        NotaPersona nuevaNotaPersona = new NotaPersona
                                        {
                                            Nota = notaAAgregar,
                                            NotaId = notaAAgregar.Id,
                                            Persona = aulaDestinada.Docente,
                                            PersonaId = aulaDestinada.Docente.Id,
                                            Leida = false,
                                            FechaLectura = null,
                                            Firmada = false,
                                            FechaFirma = null
                                        };
                                        notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                        aulaDestinada.Docente.NotaPersonas.Add(nuevaNotaPersona);
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
                                _notaRepositorie.Agregar(notaAAgregar);
                                _personaRepositorie.ActualizarNotasRecibidas(aulaDestinada.Docente.Id, notaAAgregar.NotaPersonas.Where(x => x.PersonaId == aulaDestinada.Docente.Id).FirstOrDefault());
                                this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                return Ok(true);
                            }
                            else
                            {
                                Nota notaAAgregar = new Nota
                                {
                                    Titulo = nuevaNota.Titulo,
                                    Fecha = DateTime.Today,
                                    Emisor = personaLogueada,
                                    NotaPersonas = new List<NotaPersona>(),
                                    Cuerpo = nuevaNota.Cuerpo,
                                    Tipo = Model.Enums.TipoNota.Generica,
                                    Referido = null,
                                    AulasDestinadas = null
                                };

                                foreach (var destinatario in nuevaNota.Destinatarios)
                                {
                                    var persona = _personaRepositorie.ObtenerAsync(destinatario);
                                    if (persona != null)
                                    {
                                        NotaPersona nuevaNotaPersona = new NotaPersona
                                        {
                                            Nota = notaAAgregar,
                                            NotaId = notaAAgregar.Id,
                                            Persona = persona,
                                            PersonaId = persona.Id,
                                            Leida = false,
                                            FechaLectura = null,
                                            Firmada = false,
                                            FechaFirma = null
                                        };
                                        notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                        persona.NotaPersonas.Add(nuevaNotaPersona);
                                    }
                                }

                                _notaRepositorie.Agregar(notaAAgregar);
                                foreach (var destinatario in nuevaNota.Destinatarios)
                                {
                                    var persona = _personaRepositorie.ObtenerAsync(destinatario);
                                    _personaRepositorie.ActualizarNotasRecibidas(persona.Id, notaAAgregar.NotaPersonas.Where(x => x.PersonaId == persona.Id).FirstOrDefault());
                                }
                                this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                return Ok(true);
                            }
                        }
                        else if (nuevaNota.EnviaNotaComo == "Docente")
                        {
                            if (nuevaNota.Tipo == "P")
                            {
                                Alumno alumnoReferido = null;
                                Aula aulaDestinada = null;

                                Nota notaAAgregar = new Nota
                                {
                                    Titulo = nuevaNota.Titulo,
                                    Fecha = DateTime.Today,
                                    Emisor = personaLogueada,
                                    NotaPersonas = new List<NotaPersona>(),
                                    Cuerpo = nuevaNota.Cuerpo,
                                    Tipo = Model.Enums.TipoNota.Particular,
                                };
                                alumnoReferido = _personaRepositorie.GetAlumno(nuevaNota.IdAlumnoReferido);
                                if (alumnoReferido != null)
                                {
                                    notaAAgregar.Referido = alumnoReferido;
                                    aulaDestinada = _aulaRepositorie.ObtenerAulaDeAlumno(alumnoReferido.Id);
                                    if (aulaDestinada != null)
                                    {
                                        notaAAgregar.AulasDestinadas = new List<Aula>();
                                        notaAAgregar.AulasDestinadas.Add(aulaDestinada);
                                    }
                                    else
                                    {
                                        return NotFound(false);
                                    }
                                    var padresDeAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumnoReferido.Id);
                                    if (padresDeAlumno != null && padresDeAlumno.Count() > 0)
                                    {
                                        foreach (var padre in padresDeAlumno)
                                        {
                                            NotaPersona nuevaNotaPersona = new NotaPersona
                                            {
                                                Nota = notaAAgregar,
                                                NotaId = notaAAgregar.Id,
                                                Persona = padre,
                                                PersonaId = padre.Id,
                                                Leida = false,
                                                FechaLectura = null,
                                                Firmada = false,
                                                FechaFirma = null
                                            };
                                            notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                            padre.NotaPersonas.Add(nuevaNotaPersona);
                                        }
                                    }
                                    else
                                    {
                                        return NotFound(false);
                                    }
                                    _notaRepositorie.Agregar(notaAAgregar);
                                    foreach (var padre in padresDeAlumno)
                                    {
                                        _personaRepositorie.ActualizarNotasRecibidas(padre.Id, notaAAgregar.NotaPersonas.Where(x => x.PersonaId == padre.Id).FirstOrDefault());
                                    }
                                    this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                    _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                    return Ok(true);
                                }
                                else
                                {
                                    return NotFound(false);
                                }
                            }
                            else
                            {
                                if (nuevaNota.ConAula)
                                {
                                    Aula aulaDestinada = null;

                                    Nota notaAAgregar = new Nota
                                    {
                                        Titulo = nuevaNota.Titulo,
                                        Fecha = DateTime.Today,
                                        Emisor = personaLogueada,
                                        NotaPersonas = new List<NotaPersona>(),
                                        Cuerpo = nuevaNota.Cuerpo,
                                        Tipo = Model.Enums.TipoNota.Generica,
                                        Referido = null
                                    };
                                    notaAAgregar.AulasDestinadas = new List<Aula>();
                                    foreach (var aulaId in nuevaNota.AulasDestinadas)
                                    {
                                        aulaDestinada = _aulaRepositorie.ObtenerAsync(aulaId);
                                        if (aulaDestinada != null)
                                        {
                                            notaAAgregar.AulasDestinadas.Add(aulaDestinada);
                                        }
                                        else
                                        {
                                            return NotFound(false);
                                        }

                                        if (aulaDestinada.Alumnos.Count() > 0 && aulaDestinada.Alumnos != null)
                                        {
                                            foreach (var alumno in aulaDestinada.Alumnos)
                                            {
                                                var padresAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumno.Id);
                                                if (padresAlumno != null && padresAlumno.Count() > 0)
                                                {
                                                    foreach (var padre in padresAlumno)
                                                    {
                                                        var padreYaAgregado = notaAAgregar.NotaPersonas.Where(x => x.PersonaId == padre.Id).FirstOrDefault();
                                                        if (padreYaAgregado == null)
                                                        {
                                                            NotaPersona nuevaNotaPersona = new NotaPersona
                                                            {
                                                                Nota = notaAAgregar,
                                                                NotaId = notaAAgregar.Id,
                                                                Persona = padre,
                                                                PersonaId = padre.Id,
                                                                Leida = false,
                                                                FechaLectura = null,
                                                                Firmada = false,
                                                                FechaFirma = null
                                                            };
                                                            notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                                            padre.NotaPersonas.Add(nuevaNotaPersona);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    return NotFound(false);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return NotFound(false);
                                        }
                                    }

                                    _notaRepositorie.Agregar(notaAAgregar);
                                    foreach (var notaPersona in notaAAgregar.NotaPersonas)
                                    {
                                        _personaRepositorie.ActualizarNotasRecibidas(notaPersona.PersonaId, notaPersona);
                                    }
                                    this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                    _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                    return Ok(true);
                                }
                                else
                                {
                                    Nota notaAAgregar = new Nota
                                    {
                                        Titulo = nuevaNota.Titulo,
                                        Fecha = DateTime.Today,
                                        Emisor = personaLogueada,
                                        NotaPersonas = new List<NotaPersona>(),
                                        Cuerpo = nuevaNota.Cuerpo,
                                        Tipo = Model.Enums.TipoNota.Generica,
                                        Referido = null,
                                        AulasDestinadas = null
                                    };
                                    foreach (var destinatarioId in nuevaNota.Destinatarios)
                                    {
                                        var destinatario = _personaRepositorie.ObtenerAsync(destinatarioId);
                                        if (destinatario != null)
                                        {
                                            NotaPersona nuevaNotaPersona = new NotaPersona
                                            {
                                                Nota = notaAAgregar,
                                                NotaId = notaAAgregar.Id,
                                                Persona = destinatario,
                                                PersonaId = destinatario.Id,
                                                Leida = false,
                                                FechaLectura = null,
                                                Firmada = false,
                                                FechaFirma = null
                                            };
                                            notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                            destinatario.NotaPersonas.Add(nuevaNotaPersona);
                                        }
                                        else
                                        {
                                            return NotFound(false);
                                        }
                                    }

                                    _notaRepositorie.Agregar(notaAAgregar);
                                    foreach (var notaPersona in notaAAgregar.NotaPersonas)
                                    {
                                        _personaRepositorie.ActualizarNotasRecibidas(notaPersona.PersonaId, notaPersona);
                                    }
                                    this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                    _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                    return Ok(true);
                                }
                            }
                        }
                        else
                        {
                            if (nuevaNota.Tipo == "P")
                            {
                                Alumno alumnoReferido = null;
                                Aula aulaDestinada = null;

                                Nota notaAAgregar = new Nota
                                {
                                    Titulo = nuevaNota.Titulo,
                                    Fecha = DateTime.Today,
                                    Emisor = personaLogueada,
                                    NotaPersonas = new List<NotaPersona>(),
                                    Cuerpo = nuevaNota.Cuerpo,
                                    Tipo = Model.Enums.TipoNota.Particular,
                                    AulasDestinadas = new List<Aula>(),
                                };
                                alumnoReferido = _personaRepositorie.GetAlumno(nuevaNota.IdAlumnoReferido);
                                if (alumnoReferido != null)
                                {
                                    notaAAgregar.Referido = alumnoReferido;
                                    aulaDestinada = _aulaRepositorie.ObtenerAulaDeAlumno(alumnoReferido.Id);
                                    if (aulaDestinada != null)
                                    {
                                        notaAAgregar.AulasDestinadas.Add(aulaDestinada);
                                    }
                                    else
                                    {
                                        return NotFound(false);
                                    }
                                    var padresDeAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumnoReferido.Id);
                                    if (padresDeAlumno != null && padresDeAlumno.Count() > 0)
                                    {
                                        foreach (var padre in padresDeAlumno)
                                        {
                                            NotaPersona nuevaNotaPersona = new NotaPersona
                                            {
                                                Nota = notaAAgregar,
                                                NotaId = notaAAgregar.Id,
                                                Persona = padre,
                                                PersonaId = padre.Id,
                                                Leida = false,
                                                FechaLectura = null,
                                                Firmada = false,
                                                FechaFirma = null
                                            };
                                            notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                            padre.NotaPersonas.Add(nuevaNotaPersona);
                                        }
                                    }
                                    else
                                    {
                                        return NotFound(false);
                                    }
                                    _notaRepositorie.Agregar(notaAAgregar);
                                    foreach (var notaPersona in notaAAgregar.NotaPersonas)
                                    {
                                        _personaRepositorie.ActualizarNotasRecibidas(notaPersona.PersonaId, notaPersona);
                                    }
                                    this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                    _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                    return Ok(true);
                                }
                                else
                                {
                                    return NotFound(false);
                                }
                            }
                            else
                            {
                                if (nuevaNota.ConAula)
                                {
                                    Aula aulaDestinada = null;

                                    Nota notaAAgregar = new Nota
                                    {
                                        Titulo = nuevaNota.Titulo,
                                        Fecha = DateTime.Today,
                                        Emisor = personaLogueada,
                                        NotaPersonas = new List<NotaPersona>(),
                                        Cuerpo = nuevaNota.Cuerpo,
                                        Tipo = Model.Enums.TipoNota.Generica,
                                        Referido = null,
                                        AulasDestinadas = new List<Aula>()                                    
                                    };

                                    foreach (var idAula in nuevaNota.AulasDestinadas)
                                    {
                                        aulaDestinada = _aulaRepositorie.ObtenerAsync(idAula);
                                        if (aulaDestinada != null)
                                        {
                                            notaAAgregar.AulasDestinadas.Add(aulaDestinada);
                                        }
                                        else
                                        {
                                            return NotFound(false);
                                        }

                                        if (aulaDestinada.Alumnos.Count() > 0 && aulaDestinada.Alumnos != null)
                                        {
                                            foreach (var alumno in aulaDestinada.Alumnos)
                                            {
                                                var padresAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumno.Id);
                                                if (padresAlumno != null && padresAlumno.Count() > 0)
                                                {
                                                    foreach (var padre in padresAlumno)
                                                    {
                                                        var padreYaAgregado = notaAAgregar.NotaPersonas.Where(x => x.PersonaId == padre.Id).FirstOrDefault();
                                                        if (padreYaAgregado == null)
                                                        {
                                                            NotaPersona nuevaNotaPersona = new NotaPersona
                                                            {
                                                                Nota = notaAAgregar,
                                                                NotaId = notaAAgregar.Id,
                                                                Persona = padre,
                                                                PersonaId = padre.Id,
                                                                Leida = false,
                                                                FechaLectura = null,
                                                                Firmada = false,
                                                                FechaFirma = null
                                                            };
                                                            notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                                            padre.NotaPersonas.Add(nuevaNotaPersona);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    return NotFound(false);
                                                }
                                            }
                                            if (aulaDestinada.Docente != null)
                                            {
                                                var docenteYaAgregada = notaAAgregar.NotaPersonas.Where(x => x.PersonaId == aulaDestinada.Docente.Id).FirstOrDefault();
                                                if (docenteYaAgregada == null)
                                                {
                                                    NotaPersona nuevaNotaPersona = new NotaPersona
                                                    {
                                                        Nota = notaAAgregar,
                                                        NotaId = notaAAgregar.Id,
                                                        Persona = aulaDestinada.Docente,
                                                        PersonaId = aulaDestinada.Docente.Id,
                                                        Leida = false,
                                                        FechaLectura = null,
                                                        Firmada = false,
                                                        FechaFirma = null
                                                    };
                                                    notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                                    aulaDestinada.Docente.NotaPersonas.Add(nuevaNotaPersona);
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
                                    _notaRepositorie.Agregar(notaAAgregar);
                                    foreach (var notaPersona in notaAAgregar.NotaPersonas)
                                    {
                                        _personaRepositorie.ActualizarNotasRecibidas(notaPersona.PersonaId, notaPersona);
                                    }
                                    this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                    _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                    return Ok(true);
                                }
                                else
                                {
                                    Nota notaAAgregar = new Nota
                                    {
                                        Titulo = nuevaNota.Titulo,
                                        Fecha = DateTime.Today,
                                        Emisor = personaLogueada,
                                        NotaPersonas = new List<NotaPersona>(),
                                        Cuerpo = nuevaNota.Cuerpo,
                                        Tipo = Model.Enums.TipoNota.Generica,
                                        Referido = null,
                                        AulasDestinadas = null
                                    };
                                    foreach (var destinatarioId in nuevaNota.Destinatarios)
                                    {
                                        var destinatario = _personaRepositorie.ObtenerAsync(destinatarioId);
                                        if (destinatario != null)
                                        {
                                            NotaPersona nuevaNotaPersona = new NotaPersona
                                            {
                                                Nota = notaAAgregar,
                                                NotaId = notaAAgregar.Id,
                                                Persona = destinatario,
                                                PersonaId = destinatario.Id,
                                                Leida = false,
                                                FechaLectura = null,
                                                Firmada = false,
                                                FechaFirma = null
                                            };
                                            notaAAgregar.NotaPersonas.Add(nuevaNotaPersona);
                                            destinatario.NotaPersonas.Add(nuevaNotaPersona);
                                        }
                                        else
                                        {
                                            return NotFound(false);
                                        }
                                    }

                                    _notaRepositorie.Agregar(notaAAgregar);
                                    foreach (var notaPersona in notaAAgregar.NotaPersonas)
                                    {
                                        _personaRepositorie.ActualizarNotasRecibidas(notaPersona.PersonaId, notaPersona);
                                    }
                                    this.ActualizarNombreArchivosNota(notaAAgregar.Id);
                                    _facade.EnviarMailNuevaNota(notaAAgregar, true);
                                    return Ok(true);
                                }
                            }
                        }
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
        [Route("/[controller]/[action]/{idHijo}")]
        public IActionResult EnviarNuevaNotaADocente(Guid idHijo, [FromBody] NotaADocente nuevaNotaADocente)
        {
            try
            {
                var hijo = _personaRepositorie.GetAlumno(idHijo);
                if (hijo != null)
                {
                    var aula = _aulaRepositorie.ObtenerAulaDeHijo(idHijo);
                    if (aula != null)
                    {
                        var docente = _aulaRepositorie.ObtenerDocenteDeAula(aula.Id);
                        if (docente != null)
                        {
                            Nota nuevaNota = new Nota { 
                                Titulo = nuevaNotaADocente.Titulo,
                                Cuerpo = nuevaNotaADocente.Cuerpo,
                                Fecha = DateTime.Now,
                                AulasDestinadas = new List<Aula> {aula},
                                NotaPersonas = new List<NotaPersona>(),                           
                            };
                            NotaPersona nuevaNotaPersona = new NotaPersona
                            {
                                Nota = nuevaNota,
                                NotaId = nuevaNota.Id,
                                Persona = docente,
                                PersonaId = docente.Id,
                                Leida = false,
                                FechaLectura = null,
                                Firmada = false,
                                FechaFirma = null
                            };
                            nuevaNota.NotaPersonas.Add(nuevaNotaPersona);
                            docente.NotaPersonas.Add(nuevaNotaPersona);
                            var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                            if (personaLogueada != null)
                            {
                                nuevaNota.Emisor = personaLogueada;
                            }
                            else
                            {
                                return NotFound("Persona logueada no encontrada!");
                            }
                            if (nuevaNotaADocente.Tipo == "Particular")
                            {
                                nuevaNota.Referido = hijo;
                                nuevaNota.Tipo = Model.Enums.TipoNota.Particular;
                            }
                            else
                            {
                                nuevaNota.Tipo = Model.Enums.TipoNota.Generica;
                            }

                            _notaRepositorie.Agregar(nuevaNota);
                            _personaRepositorie.ActualizarNotasRecibidas(docente.Id, nuevaNotaPersona);
                            this.ActualizarNombreArchivosNota(nuevaNota.Id);
                            _facade.EnviarMailNuevaNota(nuevaNota, true);
                            return Ok(true);
                        }
                        else
                        {
                            return NotFound("Docente no encontrada!");
                        }
                    }
                    else
                    {
                        return NotFound("Aula no encontrada!");
                    }
                }
                else
                {
                    return NotFound("Hijo no encontrado!");
                }
                
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
            
        }

        [HttpPost]
        [Route("/[controller]/[action]/{idAlumno}")]
        public IActionResult EnviarNuevaNotaAPadres(Guid idAlumno, [FromBody] NotaADocente nuevaNotaAPadres)
        {
            try
            {
                var alumno = _personaRepositorie.GetAlumno(idAlumno);
                var aulaAlumno = _aulaRepositorie.ObtenerAulaDeAlumno(alumno.Id);
                if (alumno != null && aulaAlumno != null)
                {
                    var padresAlumno = _personaRepositorie.ObtenerPadresDeAlumno(alumno.Id);                    
                    if (padresAlumno != null && padresAlumno.Count() > 0)
                    {                   
                        Nota nuevaNota = new Nota
                        {
                            Titulo = nuevaNotaAPadres.Titulo,
                            Cuerpo = nuevaNotaAPadres.Cuerpo,
                            Fecha = DateTime.Now,
                            AulasDestinadas = new List<Aula>{aulaAlumno},
                            NotaPersonas = new List<NotaPersona>(),
                        };
                       
                        var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                        if (personaLogueada != null)
                        {
                            nuevaNota.Emisor = personaLogueada;
                        }
                        else
                        {
                            return NotFound("Persona logueada no encontrada!");
                        }
                        if (nuevaNotaAPadres.Tipo == "Particular")
                        {
                            nuevaNota.Referido = alumno;
                            nuevaNota.Tipo = Model.Enums.TipoNota.Particular;
                        }
                        else
                        {
                            nuevaNota.Tipo = Model.Enums.TipoNota.Generica;
                        }
                        
                        foreach (var padre in padresAlumno)
                        {
                            NotaPersona nuevaNotaPersona = new NotaPersona
                            {
                                Nota = nuevaNota,
                                NotaId = nuevaNota.Id,
                                Persona = padre,
                                PersonaId = padre.Id,
                                Leida = false,
                                FechaLectura = null,
                                Firmada = false,
                                FechaFirma = null
                            };
                            nuevaNota.NotaPersonas.Add(nuevaNotaPersona);
                            padre.NotaPersonas.Add(nuevaNotaPersona);
                        }
                        _notaRepositorie.Agregar(nuevaNota);
                        foreach (var padre in padresAlumno)
                        {
                            var notaPersona = nuevaNota.NotaPersonas.Where(x => x.PersonaId == padre.Id).FirstOrDefault();
                            _personaRepositorie.ActualizarNotasRecibidas(padre.Id, notaPersona);
                        }
                        this.ActualizarNombreArchivosNota(nuevaNota.Id);
                        _facade.EnviarMailNuevaNota(nuevaNota, true);
                        return Ok(true);                        
                    }
                    else
                    {
                        return NotFound("Padres no encontrados!");
                    }
                }
                else
                {
                    return NotFound("Hijo o Aula no encontrados!");
                }

            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }

        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/[controller]/[action]/")]
        [RequestSizeLimit(10485760)]
        public IActionResult AgregarNotaFiles()
        {
            try
            {
                var files = Request.Form.Files;
                if (files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        string uploadsFolder = Path.Combine(_webHost.WebRootPath, "NotasFiles");
                        string fileName = $"Nota-{file.Name.Replace("-", "")}";
                        string filePath = Path.Combine(uploadsFolder, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                    }
                    return Ok(true);
                }
                return BadRequest(false);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Request body too large"))
                {
                    return BadRequest("Request body too large");
                }
                return BadRequest(ex);                
            }                       
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idNota}/")]
        public IActionResult ActualizarNombreArchivosNota(Guid idNota)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "NotasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);

            try
            {                
                foreach (var file in di.GetFiles())
                {                    
                    int indice = file.Name.IndexOf("Nota");
                    string substring = file.Name.Substring(indice);
                    if (!Regex.IsMatch(substring, @"-\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b-"))
                    {
                        string modifiedName = Regex.Replace(substring, @"-", $"-{idNota}-");
                        string updatedFileName = file.Name.Substring(0, indice) + modifiedName;
                        string oldFilePath = Path.Combine(uploadsFolder, file.Name);
                        string newFilePath = Path.Combine(uploadsFolder, updatedFileName);

                        System.IO.File.Move(oldFilePath, newFilePath);
                    }                                                         
                }
                return Ok(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]
        [Route("/[controller]/[action]/{idNota}")]
        public IActionResult ObtenerArchivosNota(Guid idNota)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "NotasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);
            var filesToShow = new FormFileCollection();
            var filesToDownload = new List<FileMetadata>();
            try
            {
                foreach (var file in di.GetFiles())
                {
                    int indice = file.Name.IndexOf("Nota");
                    string substring = file.Name.Substring(indice);
                    if (substring.Contains(idNota.ToString()))
                    {
                        FileMetadata fileMetadata = new FileMetadata
                        {
                            FileName = file.Name,
                            FileSize = file.Length,
                            ContentType = GetContentType(file.FullName)
                        };

                        using (var fileStream = System.IO.File.OpenRead(file.FullName))
                        {
                            fileMetadata.Data = new byte[fileStream.Length];
                            fileStream.Read(fileMetadata.Data, 0, fileMetadata.Data.Length);

                            FormFile fileToShow = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(fileStream.Name));
                            filesToShow.Add(fileToShow);
                        }
                        filesToDownload.Add(fileMetadata);
                    }
                }

                return Ok(filesToDownload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".docx":
                    return "application/msword";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]")]
        private string UploadedFile(Nota nuevaNota)
        {
            string fileName = null;
            if (nuevaNota.Files.Count() != 0)
            {
                foreach (var file in nuevaNota.Files)
                {
                    fileName = null;
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "NotasFiles");
                    int indexPunto = file.FileName.IndexOf('.');
                    string fileN = $"Nota{nuevaNota.Id}" + file.FileName.Substring(indexPunto);
                    fileName = Guid.NewGuid().ToString() + "_" + fileN;
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }
            return fileName;
        }
        [HttpGet]
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> VerArchivosNotas(Guid id)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "NotasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);
            ICollection<IFormFile> filesToShow = new List<IFormFile>();

            foreach (var file in di.GetFiles())
            {
                int indice = file.Name.IndexOf("Nota");
                string substring = file.Name.Substring(indice);
                if (substring.Contains(id.ToString()))
                {
                    using (var stream = System.IO.File.OpenRead(file.FullName))
                    {
                        IFormFile fileToShow = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                        filesToShow.Add(fileToShow);
                    }
                }
            }
            return View(filesToShow);
        }

        [NonAction]
        public void EliminarArchivosNota(Guid id)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "NotasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);

            foreach (var file in di.GetFiles())
            {
                try
                {
                    int indice = file.Name.IndexOf("Nota");
                    string substring = file.Name.Substring(indice);

                    if (substring.Contains(id.ToString()))
                    {
                        file.Attributes = FileAttributes.Normal;
                        using (var fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Delete))
                        {
                        }
                        file.Delete();
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"No fue posible eliminar el archivo {file.Name}: {ex.Message}");
                }
            }

        }       

        [HttpPut]
        [Route("/[controller]/[action]/{id}")]
        public IActionResult Edit(Guid id, [FromBody] NotaAModificar notaAModificar)
        {                      
            if (id != null)
            {
                try
                {                                      
                    var nota = _notaRepositorie.ObtenerAsync(id);
                    if (nota != null)
                    {
                        if (notaAModificar.AulasDestinadas.Count() > 0)
                        {
                            foreach (var idAula in notaAModificar.AulasDestinadas)
                            {
                                var AulaDestinada = _aulaRepositorie.ObtenerAsync(idAula);
                                if (AulaDestinada != null && nota.AulasDestinadas != null)
                                {
                                    var aulaEnNota = nota.AulasDestinadas.Where(x => x.Id == idAula).FirstOrDefault();
                                    if (aulaEnNota == null)
                                    {                                        
                                        nota.AulasDestinadas.Add(AulaDestinada);
                                        NotaPersona nuevaNotaPersona = new NotaPersona
                                        {
                                            Nota = nota,
                                            NotaId = nota.Id,
                                            Persona = AulaDestinada.Docente,
                                            PersonaId = AulaDestinada.Docente.Id,
                                            Leida = false,
                                            FechaLectura = null,
                                            Firmada = false,
                                            FechaFirma = null
                                        };
                                        nota.NotaPersonas.Add(nuevaNotaPersona);
                                        AulaDestinada.Docente.NotaPersonas.Add(nuevaNotaPersona);
                                    }
                                    else if (nota.Titulo == notaAModificar.Titulo && nota.Cuerpo == notaAModificar.Cuerpo)
                                    {
                                        return BadRequest("No se ha modificado ningun valor");
                                    }
                                }
                                
                            }
                            foreach (var aulaEnNota in nota.AulasDestinadas)
                            {
                                var notaEnListaDestinadas = notaAModificar.AulasDestinadas.Where(x => x == aulaEnNota.Id).FirstOrDefault();
                                if (notaEnListaDestinadas == Guid.Empty)
                                {
                                    var notaPersona = nota.NotaPersonas.Where(x => x.PersonaId == aulaEnNota.Docente.Id).FirstOrDefault();
                                    nota.NotaPersonas.Remove(notaPersona);
                                    nota.AulasDestinadas.Remove(aulaEnNota);
                                }
                            }
                        }                        
                        nota.Titulo = notaAModificar.Titulo;
                        nota.Cuerpo = notaAModificar.Cuerpo;
                         
                        foreach (var notaPersona in nota.NotaPersonas)
                        {
                            notaPersona.Leida = false; // Reiniciamos la property Leida a false, para que re lean la nota modificada
                        }
                        _notaRepositorie.Modificar(nota);
                        var notaModificada = _notaRepositorie.ObtenerAsync(id);
                        _facade.EnviarMailNuevaNota(notaModificada, false);
                        return Ok(true);
                    }
                    else
                    {
                        return NotFound("Nota no encontrada");
                    }                    
                }
                catch (Exception ex)
                {
                    return BadRequest("Ocurrio un error");
                }                
            }
            return BadRequest("El Id no existe");      
        }
       
        [HttpDelete]
        [Route("/[controller]/[action]/{id}")]        
        public IActionResult DeleteConfirmed(Guid id)
        {            
            var nota = _notaRepositorie.ObtenerAsync(id);
            if (nota != null)
            {
                _notaRepositorie.Borrar(nota.Id);
                EliminarArchivosNota(id);
                return Ok(true);
            }
            
            return NotFound(false);
        }

    }
}
