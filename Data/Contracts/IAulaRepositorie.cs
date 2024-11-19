using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Data.Contracts
{
    public interface IAulaRepositorie : IGenericRepositorie<Aula>
    {
        IEnumerable<Aula> ObtenerAulasDocente(Guid id);
        ICollection<Aula> ObtenerAulasDeInstitucion(Guid id);
        Persona ObtenerDocenteDeAula(Guid AulaId);
        Aula ObtenerAulaDeHijo(Guid idHijo);
        IEnumerable<Alumno> ObtenerAlumnosAula(Guid id);
        IEnumerable<Alumno> ObtenerAlumnosSinAula(Guid id);
        IEnumerable<Persona> ObtenerDocentesSinAulaAsignada(Guid id);
        string CheckearValorRepetido(Guid idInstitucion, string nombreAula, string gradoAula, string DivisionAula);
        string CheckearValorRepetidoEdicionAula(Guid idAula, string nombreAula, string gradoAula, string DivisionAula);
        Aula ObtenerAulaDeAlumno(Guid idAlumno);
        void AsignarDocenteAAula(Guid idAula, Persona docente);
        bool VerificarAlumnoEnAula(Guid idAlumno);
        void AgregarAlumnosAAula(Guid idAula, Persona alumno);
        void AgregarAsistenciaTomadaAula(Guid idAula, Asistencia nuevaAsistenciaTomada);
        bool EliminarAlumnoDeAula(Guid idAlumno);
        void CheckearAulasAsignadasAPersona(Persona persona);
        Aula ObtenerAulaConAsistencias(Guid idAula);

        void EliminarDocenteDeAulasAsignadas(Guid idDocente);
    }
}
