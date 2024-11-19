using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
    public interface IGenericRepositorie<T> where T : class
    {
        IEnumerable<T> ObtenerTodosAsync();
        T ObtenerAsync(Guid id);
        void Agregar(T entity);
        void Borrar(Guid id);
        void Modificar(T entity);


    }
}
