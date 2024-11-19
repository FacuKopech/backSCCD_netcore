using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos;
using Model.State;
using SCCD.FacadePattern;
using System.Text.RegularExpressions;
using SCCD.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace SCCD.Controllers
{
    [ApiController]
    public class AusenciasController : Controller
    {
        private IAusenciaRepositorie _ausenciaRepositorie;
        private IAusenciaDataLayerRepo _ausenciaDataLayerRepo;        
        private IPersonaRepositorie _personaRepositorie;
        private IAulaRepositorie _aulaRepositorie;
        private IWebHostEnvironment _webHost;
        private IArchivosService _archivosService;
        private Session _session = Session.GetInstance();
        private readonly Facade _facade;

        public AusenciasController(IAusenciaRepositorie ausenciasRepositorie, IAusenciaDataLayerRepo ausenciaDataLayerRepo, IPersonaRepositorie personaaRepositorie,
            IAulaRepositorie aulaRepositorie, IWebHostEnvironment webHost, IArchivosService archivosService)
        {

            _ausenciaRepositorie = ausenciasRepositorie;
            _ausenciaDataLayerRepo = ausenciaDataLayerRepo;
            _personaRepositorie = personaaRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _webHost = webHost;
            _archivosService = archivosService;
            _facade = new Facade(_webHost, _personaRepositorie, _aulaRepositorie);
        }

        [HttpGet]
        [Route("/[controller]/[action]/{id}")]
        public IEnumerable<Ausencia> ObtenerAusenciasDeAlumno(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return null;
            }
            else
            {                
                var alumno = _personaRepositorie.GetAlumno(id);
                if (alumno != null)
                {
                    return alumno.Ausencias;
                }
            }

            return null;
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAusencia}/{idAlumno}")]
        public bool GetEsAusenciaGenerica(Guid idAusencia, Guid idAlumno)
        {
            if (idAusencia == Guid.Empty || idAusencia == Guid.Empty)
            {
                return false;
            }
            else
            {
                var ausenciaEncontrada = _ausenciaRepositorie.ObtenerAsync(idAusencia);
                if (ausenciaEncontrada != null && ausenciaEncontrada.Motivo == "Toma de asistencia - Hijo/a ausente")
                {
                    return false;
                }
                Persona personaLogueada = null;
                IEnumerable<Persona> hijosPadre = null;                

                if (idAlumno == null || idAlumno == Guid.Empty)
                {
                    personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                    hijosPadre = _personaRepositorie.ObtenerHijos(personaLogueada.Id);
                }
                else
                {
                    var padresDeAlumno = _personaRepositorie.ObtenerPadresDeAlumno(idAlumno);
                    var padre = padresDeAlumno.First();
                    hijosPadre = _personaRepositorie.ObtenerHijos(padre.Id);
                }
                
                int counter = 0;
                foreach (var hijo in hijosPadre)
                {
                    var alumno = _personaRepositorie.GetAlumno(hijo.Id);
                    var ausencia = alumno.Ausencias.Where(x => x.Id == idAusencia).FirstOrDefault();
                    if (ausencia != null)
                    {
                        counter += 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (counter == hijosPadre.Count())
                {
                    return true;
                }

                return false;
            }            
        }
        
        [HttpPost]        
        [Route("/[controller]/[action]/{idHijo}")]      
        public IActionResult AgregarAusencia(Guid idHijo, [FromBody] AusenciaModificar nuevaAusencia)
        {
            try
            {                
                Ausencia nuevaAusenciaAlumno = new Ausencia();
                List<Alumno> hijosConAusencia = new List<Alumno>();
                var ausenciasHijo = this.ObtenerAusenciasDeAlumno(idHijo);
                var ausenciaExiste = ausenciasHijo.Any(x => x.Motivo == nuevaAusencia.Motivo 
                    && x.FechaComienzo == nuevaAusencia.FechaComienzo 
                    && x.FechaFin == nuevaAusencia.FechaFin);
                if (!ausenciaExiste)
                {
                    nuevaAusenciaAlumno.Motivo = nuevaAusencia.Motivo;
                    nuevaAusenciaAlumno.FechaComienzo = nuevaAusencia.FechaComienzo;
                    nuevaAusenciaAlumno.FechaFin = nuevaAusencia.FechaFin;
                    nuevaAusenciaAlumno.FechaEmision = DateTime.Now;
                    var alumno = _personaRepositorie.GetAlumno(idHijo);
                    if (alumno != null)
                    {
                        hijosConAusencia.Add(alumno);
                        nuevaAusenciaAlumno.HijosConAusencia = hijosConAusencia;
                        _ausenciaRepositorie.Agregar(nuevaAusenciaAlumno);
                        this.ActualizarNombreArchivosAusencia(nuevaAusenciaAlumno.Id);
                        _facade.EnviarMailAusencia(nuevaAusenciaAlumno, alumno, "nueva");
                        return Ok(true);
                    }
                    return NotFound("Hijo no encontrado");
                }
                return BadRequest("La Ausencia ya existe");                
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/[controller]/[action]")]
        [RequestSizeLimit(10485760)]
        public bool AgregarAusenciaFiles()
        {
            var files = Request.Form.Files;
            
            if (files.Count() > 0)
            {
               foreach (var file in files)
               {
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");                        
                    string fileName = $"Ausencia-{file.Name.Replace("-", "")}";
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
               }
                return true;                                
            }
            return false;
        }

        [HttpGet]
        [Route("/[controller]/[action]/{idAusencia}")]
        public IActionResult ActualizarNombreArchivosAusencia(Guid idAusencia)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);
            
            try
            {
                foreach (var file in di.GetFiles())
                {
                    int indice = file.Name.IndexOf("Ausencia");
                    string substring = file.Name.Substring(indice);
                    if (!Regex.IsMatch(substring, @"-\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b-"))
                    {
                        string modifiedName = Regex.Replace(substring, @"-", $"-{idAusencia}-");
                        string updatedFileName = file.Name.Substring(0, indice) + modifiedName;
                        string oldFilePath = Path.Combine(uploadsFolder, file.Name);
                        string newFilePath = Path.Combine(uploadsFolder, updatedFileName);
                        using (var fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                        }
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

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult AgregarAusenciaGenerica([FromBody] AusenciaModificar nuevaAusencia)
        {
            try
            {
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                var hijosPadreLogueado = _personaRepositorie.ObtenerHijos(personaLogueada.Id).OfType<Alumno>();
                var hijoDePadre = hijosPadreLogueado.First();
                var ausenciasHijo = this.ObtenerAusenciasDeAlumno(hijoDePadre.Id);
                var ausenciaExiste = ausenciasHijo.Any(x => x.Motivo == nuevaAusencia.Motivo
                    && x.FechaComienzo == nuevaAusencia.FechaComienzo
                    && x.FechaFin == nuevaAusencia.FechaFin);
                if (!ausenciaExiste)
                {
                    Ausencia nuevaAusenciaAlumno = new Ausencia();
                    List<Alumno> hijosConAusencia = new List<Alumno>();
                    nuevaAusenciaAlumno.Motivo = nuevaAusencia.Motivo;
                    nuevaAusenciaAlumno.FechaComienzo = nuevaAusencia.FechaComienzo;
                    nuevaAusenciaAlumno.FechaFin = nuevaAusencia.FechaFin;
                    nuevaAusenciaAlumno.FechaEmision = DateTime.Now;

                    if (hijosPadreLogueado != null && hijosPadreLogueado.Count() > 0)
                    {
                        foreach (var hijo in hijosPadreLogueado)
                        {
                            var aulaDeAlumno = _aulaRepositorie.ObtenerAulaDeAlumno(hijo.Id);
                            if (aulaDeAlumno != null)
                            {
                                var alumno = _personaRepositorie.GetAlumno(hijo.Id);
                                hijosConAusencia.Add(alumno);
                            }
                            else if (aulaDeAlumno == null)
                            {
                                return BadRequest("No puede agregar una Ausencia a un Hijo sin Aula asignada");
                            }
                            else if (aulaDeAlumno == null && hijosPadreLogueado.Count() > 1)
                            {
                                continue;
                            }
                        }
                        nuevaAusenciaAlumno.HijosConAusencia = hijosConAusencia;
                        _ausenciaRepositorie.Agregar(nuevaAusenciaAlumno);
                        this.ActualizarNombreArchivosAusencia(nuevaAusenciaAlumno.Id);
                        foreach (var hijo in hijosPadreLogueado)
                        {
                            _personaRepositorie.ActualizarAusenciaAlumno(hijo.Id, nuevaAusenciaAlumno, "A");
                            _facade.EnviarMailAusencia(nuevaAusenciaAlumno, hijo, "nueva");
                        }

                        return Ok(true);
                    }
                    return NotFound("No se encontraron Hijos para el Padre");
                }
                return BadRequest("La Ausencia ya existe");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [NonAction]
        private string UploadedFile(Ausencia ausencia)
        {
            string fileName = null;
            if (ausencia.Files.Count() != 0)
            {
                foreach (var file in ausencia.Files)
                {
                    fileName = null;
                    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");
                    int indexPunto = file.FileName.IndexOf('.');
                    string fileN = $"Ausencia{ausencia.Id}" + file.FileName.Substring(indexPunto);
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
        [Route("/[controller]/[action]/{idAusencia}")]
        public IActionResult ObtenerArchivosAusencia(Guid idAusencia)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);
            var filesToShow = new FormFileCollection();  
            var filesToDownload = new List<FileMetadata>();
            try
            {
                foreach (var file in di.GetFiles())
                {
                    int indice = file.Name.IndexOf("Ausencia");
                    string substring = file.Name.Substring(indice);

                    if (substring.Contains(idAusencia.ToString()))
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

        [HttpPut]
        [Route("/[controller]/[action]/{idAusencia}/{idAlumno}/{aceptada}")]
        public IActionResult AceptarODenegarAusencia(Guid IdAusencia, Guid idAlumno, bool aceptada)
        {
            try
            {
                var ausencia = _ausenciaRepositorie.ObtenerAsync(IdAusencia);
                if (ausencia != null)
                {
                    if (aceptada)
                    {
                        ausencia.SetState(new ApprovedState(_ausenciaDataLayerRepo));
                        ausencia.AceptarAusencia(ausencia);
                        var alumno = _personaRepositorie.GetAlumno(idAlumno);
                        if (alumno != null)
                        {
                            _facade.EnviarMailAusencia(ausencia, alumno, "aceptada");
                        }
                        else
                        {
                            return NotFound("Alumno no encontrado");
                        }                                                
                    }
                    else
                    {
                        ausencia.SetState(new DeniedState(_ausenciaDataLayerRepo));
                        ausencia.DenegarAusencia(ausencia);
                        var alumno = _personaRepositorie.GetAlumno(idAlumno);
                        if (alumno != null)
                        {
                            _facade.EnviarMailAusencia(ausencia, alumno, "denegada");
                        }
                        else
                        {
                            return NotFound("Alumno no encontrado");
                        }                        
                    }
                    
                    return Ok(true);
                }
                else
                {
                    return NotFound("Ausencia no encontrada");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }                       
        }       
     
        [HttpPut]
        [Route("/[controller]/[action]/{idAusencia}/{idHijo}")]
        public IActionResult EditarAusencia(Guid idAusencia, Guid idHijo, [FromBody] AusenciaModificar ausenciaModificar)
        {
            if (idAusencia == null || idAusencia == Guid.Empty)
            {
                return BadRequest("Id de Asuencia invalido");
            }

            try
            {
                var ausencia = _ausenciaRepositorie.ObtenerAsync(idAusencia);
                if (ausencia != null)
                {
                    ausencia.Motivo = ausenciaModificar.Motivo;
                    ausencia.FechaComienzo = ausenciaModificar.FechaComienzo;
                    ausencia.FechaFin = ausenciaModificar.FechaFin;
                    ausencia.Justificada = "";
                    _ausenciaRepositorie.Modificar(ausencia);
                    var esAusenciaGenerica = this.GetEsAusenciaGenerica(idAusencia, idHijo);
                    if (!esAusenciaGenerica)
                    {
                        var alumno = _personaRepositorie.GetAlumno(idHijo);
                        if (alumno != null)
                        {
                            _facade.EnviarMailAusencia(ausencia, alumno, "modificada");
                            return Ok(true);
                        }
                    }
                    else
                    {
                        var padreLogueado = _personaRepositorie.ObtenerPersonaDeUsuario(Guid.Parse(_session.IdUserLogueado));
                        var hijosDePadre = _personaRepositorie.ObtenerHijos(padreLogueado.Id).OfType<Alumno>();
                        if (hijosDePadre != null)
                        {
                            foreach (var hijo in hijosDePadre)
                            {
                                _facade.EnviarMailAusencia(ausencia, hijo, "modificada");
                            }
                            return Ok(true);
                        }
                    }
                    
                    return NotFound("Alumno no encontrado"); ;
                }

                return NotFound("Ausencia no encontrada"); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
       
        }       


        [HttpDelete]
        [Route("/[controller]/[action]/{idAusencia}/{idHijo}")]
        public IActionResult DeleteConfirmed(Guid idAusencia, Guid idHijo)
        {
           
            var ausencia = _ausenciaRepositorie.ObtenerAsync(idAusencia);
            if (ausencia != null)
            {
                _personaRepositorie.ActualizarAusenciaAlumno(idHijo, ausencia, "B");
                _archivosService.EliminarArchivosAusencia(idAusencia);
                _ausenciaRepositorie.Borrar(idAusencia);
                return Ok(true);
            }

            return NotFound("La Ausencia no se ha encontrado"); ;
        }       
    }
}
