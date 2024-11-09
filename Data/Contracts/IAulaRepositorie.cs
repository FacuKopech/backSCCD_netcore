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
        IEnumerable<Aula> ObtenerAulasDocente(int id);
        ICollection<Aula> ObtenerAulasDeInstitucion(int id);
        Persona ObtenerDocenteDeAula(int AulaId);
        Aula ObtenerAulaDeHijo(int idHijo);
        IEnumerable<Alumno> ObtenerAlumnosAula(int id);
        IEnumerable<Alumno> ObtenerAlumnosSinAula(int id);
        IEnumerable<Persona> ObtenerDocentesSinAulaAsignada(int id);
        string CheckearValorRepetido(int idInstitucion, string nombreAula, string gradoAula, string DivisionAula);
        string CheckearValorRepetidoEdicionAula(int idAula, string nombreAula, string gradoAula, string DivisionAula);
        Aula ObtenerAulaDeAlumno(int idAlumno);
        void AsignarDocenteAAula(int idAula, Persona docente);
        bool VerificarAlumnoEnAula(int idAlumno);
        void AgregarAlumnosAAula(int idAula, Persona alumno);
        void AgregarAsistenciaTomadaAula(int idAula, Asistencia nuevaAsistenciaTomada);
        bool EliminarAlumnoDeAula(int idAlumno);
        void CheckearAulasAsignadasAPersona(Persona persona);
        Aula ObtenerAulaConAsistencias(int idAula);

        void EliminarDocenteDeAulasAsignadas(int idDocente);
    }
}
