using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.CompositePattern
{
    public interface IAuthentication
    {
        Usuario Authenticate(object credentials);
    }
}
