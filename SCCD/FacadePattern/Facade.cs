using Data.Contracts;
using Model.Entities;

namespace SCCD.FacadePattern
{
    public class Facade
    {
        private IWebHostEnvironment _webHost;
        private IPersonaRepositorie _personaRepositorie;
        private IAulaRepositorie _aulaRepositorie;

        AusenciaMailSubSystem ausenciaMailSubSystem;
        HistorialMailSubSystem historialMailSubSystem;
        NotaMailSubSystem notaMailSubSystem;
        EventoMailSubSystem eventoMailSubSystem;
        public Facade(IWebHostEnvironment webHost, IPersonaRepositorie personaaRepositorie,
            IAulaRepositorie aulaRepositorie)
        {
            _webHost = webHost;
            _personaRepositorie = personaaRepositorie;
            _aulaRepositorie = aulaRepositorie;

            ausenciaMailSubSystem = new AusenciaMailSubSystem(_webHost, _personaRepositorie, _aulaRepositorie);
            notaMailSubSystem = new NotaMailSubSystem(_webHost);
        }

        public Facade(IPersonaRepositorie personaaRepositorie,
           IAulaRepositorie aulaRepositorie)
        {
            _personaRepositorie = personaaRepositorie;
            _aulaRepositorie = aulaRepositorie;

            historialMailSubSystem = new HistorialMailSubSystem(_personaRepositorie, _aulaRepositorie);
            eventoMailSubSystem = new EventoMailSubSystem(_personaRepositorie, _aulaRepositorie);
        }

        public void EnviarMailNuevaNota(Nota notaAEnviar, bool esNueva)
        {
            notaMailSubSystem.EnviarMail(notaAEnviar, esNueva);
        }

        public void EnviarMailNotaFirmada(string titulo, string emisorEmail)
        {
            notaMailSubSystem.EnviarMailNotaFirmada(titulo, emisorEmail);
        }

        public void EnviarMailAusencia(Ausencia ausencia, Alumno alumno, string accion)
        {
            ausenciaMailSubSystem.EnviarMailAusencia(ausencia, alumno, accion);
        }

        public void EnviarMailHistorial(Historial historial, int idHijo, string accion)
        {
            historialMailSubSystem.EnviarMailHistorial(historial, idHijo, accion);
        }

        public void EnviarMailNuevoEvento(Evento evento)
        {
            eventoMailSubSystem.EnviarMailNuevoEvento(evento);
        }

        public void EnviarMailEventoModificado(Evento evento)
        {
            eventoMailSubSystem.EnviarMailEventoModificado(evento);
        }

        public void EnviarMailConfirmacionAsistenciaEvento(Evento evento, string confirmacion)
        {
            eventoMailSubSystem.EnviarMailConfirmacionAsistenciaEvento(evento, confirmacion);
        }
    }
}
