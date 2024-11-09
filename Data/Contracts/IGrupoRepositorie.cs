using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Entities;

namespace Data.Contracts
{
    public interface IGrupoRepositorie : IGenericRepositorie<Grupo>
    {
        void AgregarUserAGrupo(Usuario user); 
    }
}
