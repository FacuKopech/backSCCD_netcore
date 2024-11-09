using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface ILoginAuditRepositorie : IGenericRepositorie<LoginAudit>
    {
        IEnumerable<LoginAudit> ObtenerLoginsDelMes();
        IEnumerable<LoginAudit> ObtenerLoginsDeUsuario(int userId);
    }
}
