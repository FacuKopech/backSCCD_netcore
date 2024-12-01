using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Observer
{
    public abstract class Subject
    {
        public Entities.NotaPersona nota;
        private List<IObserver> observadores = new List<IObserver>();

        public Subject(Entities.NotaPersona Nota)
        {
            this.nota = Nota;
        }

        public bool? Leida
        {
            get { return this.nota.Leida; }
            set 
            {
                nota.Leida = true;
                Notify();
            }
        }
        public void Attach(IObserver observer)
        {
            observadores.Add(observer);
        }
        public void Detach(IObserver observer)
        {
            observadores.Remove(observer);
        }
        public void Notify()
        {
            foreach (IObserver observer in observadores)
            {
                observer.update(this);
            }

        }
    }
}
