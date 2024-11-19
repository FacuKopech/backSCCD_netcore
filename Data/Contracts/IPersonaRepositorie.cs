using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Data.Contracts
{
    public interface IPersonaRepositorie : IGenericRepositorie<Persona>
    {
        Persona ObtenerPersonaDeUsuario(Guid idUser);
        List<string> ObtenerEmailsDeTodasLasPersonasDeUnaInstitucion(Guid idInstitucion);
        List<string> ObtenerEmailsParaDocente(Persona docenteLogueada);
        List<string> ObtenerEmailsParaPadre(Persona padreLogueado);
        IEnumerable<Persona> ObtenerAlumnosInstitucion(Guid id);
        IEnumerable<Persona> ObtenerAlumnosSistema();
        IEnumerable<Persona> ObtenerPersonasSistema();
        IEnumerable<Persona> ObtenerHijos(Guid id);
        IEnumerable<Persona> ObtenerPadresDeAlumno(Guid id);
        Persona ObtenerDirectivoInstitucion(Guid id);
        IEnumerable<Persona> ObtenerPadres();
        IEnumerable<Persona> GetDirectivosDocentesPadres();
        void AgregarHijosAPadre(Guid idPadre, Persona alumno);
        void ModificarHijosAsignados(Persona entity, string idHijo, string accion);
        Alumno GetAlumno(Guid id);        
        void ActualizarNotasRecibidas(Guid id, Nota nota);
        void ActualizarHistorialAlumno(Guid idAlumno, Historial nuevoHistorial);
        void ActualizarAusenciaAlumno(Guid idAlumno, Ausencia nuevaAusencia, string accion);
        Task<bool> EliminarAusenciasAlumno(Guid idAlumno);
        IEnumerable<Ausencia> ObtenerAusenciasAlumno(Guid idAlumno);
        void ActualizarAsistenciaAlumno(Guid idAlumno);
        void AgregarAsistenciaAlumno(Guid idAlumno, AsistenciaAlumno nuevaAsistenciaAlumno);
        Task<bool> EliminarHistorial(Guid idAlumno, Historial historial);
        Task<bool> ResetearFirmasHistorialesAlumno(Guid idAlumno);
        IEnumerable<Persona> ObtenerPersonasInstitucion(Persona tipoPersona, Guid idInstitucion);
        IEnumerable<Persona> ObtenerPadresDocentesDirectivosInstitucion(Guid idInstitucion);
        IEnumerable<Persona> ObtenerDocentesDeInstitucion(Guid id);

    }
}
