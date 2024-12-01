using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Observer
{
    public class NotaObservable : Subject
    {
        public NotaObservable(Entities.NotaPersona nota)
            : base(nota)
        {
        }
    }
}
