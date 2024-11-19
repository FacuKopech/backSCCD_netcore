using Data.Contracts;
using Model.State;
using SCCD.FacadePattern;
using SCCD.Services.Interfaces;

namespace SCCD.Services.Entities
{
    public class ArchivosService : IArchivosService
    {
        private IWebHostEnvironment _webHost;
        public ArchivosService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }
        public void EliminarArchivosAusencia(Guid idAusencia)
        {
            string uploadsFolder = Path.Combine(_webHost.WebRootPath, "AusenciasFiles");
            DirectoryInfo di = new DirectoryInfo(uploadsFolder);

            foreach (var file in di.GetFiles())
            {
                try
                {
                    int indice = file.Name.IndexOf("Ausencia");
                    string substring = file.Name.Substring(indice);

                    if (substring.Contains(idAusencia.ToString()))
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
    }
}
