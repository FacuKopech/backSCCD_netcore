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
        Persona ObtenerPersonaDeUsuario(int idUser);

        List<string> ObtenerEmailsDeTodasLasPersonasDeUnaInstitucion(int idInstitucion);

        List<string> ObtenerEmailsParaDocente(Persona docenteLogueada);
        List<string> ObtenerEmailsParaPadre(Persona padreLogueado);
        IEnumerable<Persona> ObtenerAlumnosInstitucion(int id);
        IEnumerable<Persona> ObtenerAlumnosSistema();
        IEnumerable<Persona> ObtenerPersonasSistema();
        IEnumerable<Persona> ObtenerHijos(int id);
        IEnumerable<Persona> ObtenerPadresDeAlumno(int id);

        Persona ObtenerDirectivoInstitucion(int id);
        IEnumerable<Persona> ObtenerPadres();
        IEnumerable<Persona> GetDirectivosDocentesPadres();
        void AgregarHijosAPadre(int idPadre, Persona alumno);
        Alumno GetAlumno(int id);        
        void ActualizarNotasRecibidas(int id, Nota nota);
        void ActualizarHistorialAlumno(int idAlumno, Historial nuevoHistorial);
        void ActualizarAusenciaAlumno(int idAlumno, Ausencia nuevaAusencia, string accion);        
        void ActualizarAsistenciaAlumno(int idAlumno);
        void AgregarAsistenciaAlumno(int idAlumno, AsistenciaAlumno nuevaAsistenciaAlumno);
        Task<bool> EliminarHistorial(int idAlumno, Historial historial);

        IEnumerable<Persona> ObtenerPersonasInstitucion(Persona tipoPersona, int idInstitucion);
        IEnumerable<Persona> ObtenerPadresDocentesDirectivosInstitucion(int idInstitucion);
        IEnumerable<Persona> ObtenerDocentesDeInstitucion(int id);

    }
}
