using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class LoggedInUser : Persona
    {
        public List<Grupo> Roles { get; set; }
    }
}
