using Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Dtos;
using Model.State;
using SCCD.FacadePattern;
using System.Text.RegularExpressions;

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
        private Session _session = Session.GetInstance();
        private readonly Facade _facade;

        public AusenciasController(IAusenciaRepositorie ausenciasRepositorie, IAusenciaDataLayerRepo ausenciaDataLayerRepo, IPersonaRepositorie personaaRepositorie,
            IAulaRepositorie aulaRepositorie, IWebHostEnvironment webHost)
        {

            _ausenciaRepositorie = ausenciasRepositorie;
            _ausenciaDataLayerRepo = ausenciaDataLayerRepo;
            _personaRepositorie = personaaRepositorie;
            _aulaRepositorie = aulaRepositorie;
            _webHost = webHost;
            _facade = new Facade(_webHost, _personaRepositorie, _aulaRepositorie);
        }

        [HttpGet]
        [Route("/[controller]/[action]/{id}")]
        public IEnumerable<Ausencia> ObtenerAusenciasDeAlumno(int id)
        {
            if (id == null || id == 0)
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
        public bool GetEsAusenciaGenerica(int idAusencia, int idAlumno)
        {
            if (idAusencia == null || idAusencia == 0)
            {
                return false;
            }
            else
            {
                Persona personaLogueada = null;
                IEnumerable<Persona> hijosPadre = null;                

                if (idAlumno == null || idAlumno < 0)
                {
                    personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                    hijosPadre = _personaRepositorie.ObtenerHijos(Convert.ToInt32(personaLogueada.Id));
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
        public bool AgregarAusencia(int idHijo, [FromBody] AusenciaModificar nuevaAusencia)
        {
            try
            {                
                Ausencia nuevaAusenciaAlumno = new Ausencia();
                List<Alumno> hijosConAusencia = new List<Alumno>();
                nuevaAusenciaAlumno.Motivo = nuevaAusencia.Motivo;
                nuevaAusenciaAlumno.FechaComienzo = nuevaAusencia.FechaComienzo;
                nuevaAusenciaAlumno.FechaFin = nuevaAusencia.FechaFin;                
                nuevaAusenciaAlumno.FechaEmision = DateTime.Now;               
                
                                
                var alumno = _personaRepositorie.GetAlumno(idHijo);
                hijosConAusencia.Add(alumno);
                nuevaAusenciaAlumno.HijosConAusencia = hijosConAusencia;
                _ausenciaRepositorie.Agregar(nuevaAusenciaAlumno);
                this.ActualizarNombreArchivosAusencia(nuevaAusenciaAlumno.Id);
                _facade.EnviarMailAusencia(nuevaAusenciaAlumno, alumno, "nueva");
                return true;
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
        public IActionResult ActualizarNombreArchivosAusencia(int idAusencia)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);
            
            try
            {
                foreach (var file in di.GetFiles())
                {
                    int indice = file.Name.IndexOf("Ausencia");
                    string substring = file.Name.Substring(indice);
                    if (!Regex.IsMatch(substring, @"-\d+-"))
                    {
                       string modifiedName = Regex.Replace(substring, @"-", $"-{idAusencia}-");
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

        [HttpPost]
        [Route("/[controller]/[action]")]
        public IActionResult AgregarAusenciaGenerica([FromBody] AusenciaModificar nuevaAusencia)
        {
            try
            {
                Ausencia nuevaAusenciaAlumno = new Ausencia();
                List<Alumno> hijosConAusencia = new List<Alumno>();
                nuevaAusenciaAlumno.Motivo = nuevaAusencia.Motivo;
                nuevaAusenciaAlumno.FechaComienzo = nuevaAusencia.FechaComienzo;
                nuevaAusenciaAlumno.FechaFin = nuevaAusencia.FechaFin;                  
                nuevaAusenciaAlumno.FechaEmision = DateTime.Now;
                
                var personaLogueada = _personaRepositorie.ObtenerPersonaDeUsuario(Convert.ToInt32(_session.IdUserLogueado));
                var hijosPadreLogueado = _personaRepositorie.ObtenerHijos(Convert.ToInt32(personaLogueada.Id));
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
                        else if(aulaDeAlumno == null && hijosPadreLogueado.Count() == 1)
                        {
                            return BadRequest("No puede agregar una Ausencia a un Hijo sin Aula asignada");
                        }
                        else if(aulaDeAlumno == null && hijosPadreLogueado.Count() > 1)
                        {
                            continue;
                        }                      
                    }
                    nuevaAusenciaAlumno.HijosConAusencia = hijosConAusencia;
                    _ausenciaRepositorie.Agregar(nuevaAusenciaAlumno);
                    this.ActualizarNombreArchivosAusencia(nuevaAusenciaAlumno.Id);
                    foreach (var hijo in hijosPadreLogueado)
                    {                                                
                        var alumno = _personaRepositorie.GetAlumno(hijo.Id);
                        _facade.EnviarMailAusencia(nuevaAusenciaAlumno, alumno, "nueva");                        
                    }                    
                }

                return Ok(true);
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
        public IActionResult ObtenerArchivosAusencia(int idAusencia)
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
                        FileMetadata fileMetadata = new FileMetadata();
                        fileMetadata.FileName = file.Name;
                        fileMetadata.FileSize = file.Length;
                        string fullPath = uploadsFolder + Path.DirectorySeparatorChar + file.Name;
                        byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                        fileMetadata.Data = fileBytes;
                        fileMetadata.ContentType = GetContentType(file.FullName);
                        filesToDownload.Add(fileMetadata);
                        using (var stream = System.IO.File.OpenRead(file.FullName))
                        {
                            FormFile fileToShow = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                            filesToShow.Add(fileToShow);
                        }
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


        [NonAction]
        public void EliminarArchivosAusencia(int id)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);

            foreach (var file in di.GetFiles())
            {
                int indice = file.Name.IndexOf("Ausencia");
                string substring = file.Name.Substring(indice);
                if (substring.Contains(id.ToString()))
                {
                    file.Delete();
                }
            }

        }
        [HttpPut]
        [Route("/[controller]/[action]/{idAusencia}/{idAlumno}/{aceptada}")]
        public IActionResult AceptarODenegarAusencia(int IdAusencia, int idAlumno, bool aceptada)
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
                            return NotFound();
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
                            return NotFound();
                        }                        
                    }
                    
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
        [Route("/[controller]/[action]/{idAusencia}/{idHijo}")]
        public bool EditarAusencia(int idAusencia, int idHijo, [FromBody] AusenciaModificar ausenciaModificar)
        {
            if (idAusencia == null)
            {
                return false;
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
                    var alumno = _personaRepositorie.GetAlumno(idHijo);
                    _facade.EnviarMailAusencia(ausencia, alumno, "modificada");
                    return true;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
       
        }       


        [HttpDelete]
        [Route("/[controller]/[action]/{idAusencia}/{idHijo}")]
        public bool DeleteConfirmed(int idAusencia, int idHijo)
        {
           
            var ausencia = _ausenciaRepositorie.ObtenerAsync(idAusencia);
            if (ausencia != null)
            {
                _personaRepositorie.ActualizarAusenciaAlumno(idHijo, ausencia, "B");
                EliminarArchivosAusencia(idAusencia);
                _ausenciaRepositorie.Borrar(idAusencia);
                return true;
            }

            return false;
        }       
    }
}
