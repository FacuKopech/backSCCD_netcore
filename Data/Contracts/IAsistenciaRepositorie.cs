using Microsoft.EntityFrameworkCore;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IAsistenciaRepositorie : IGenericRepositorie<Asistencia>
    {
        Asistencia ObtenerUltimaAsistenciaAgregada();
        Asistencia ObtenerAsistencia(int idAsistencia);

        void AgregarAsistenciaAlumno(AsistenciaAlumno nuevaAsistenciaAlumno);
        
    }
}
