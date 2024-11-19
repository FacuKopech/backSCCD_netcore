using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.State;

namespace Data.Repositories
{
    public class AusenciaRepositorie : IAusenciaRepositorie, IAusenciaDataLayerRepo
    {
        private readonly ApplicationDbContext _context;

        public AusenciaRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }
        public void Agregar(Ausencia entity)
        {
            _context.Ausencias.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            var ausencia = _context.Ausencias.Where(x => x.Id == id).FirstOrDefault();
            if (ausencia != null)
            {
                _context.Ausencias.Remove(ausencia);
                _context.SaveChanges();                
            }
        }

        public void Modificar(Ausencia ausenciaAModificar)
        {
            var ausencia = _context.Ausencias.Where(x => x.Id == ausenciaAModificar.Id).FirstOrDefault();
            if (ausencia != null)
            {
                _context.Entry(ausenciaAModificar).State= EntityState.Modified;                
                _context.SaveChanges();                
            }
        }

        public Ausencia ObtenerAsync(Guid id)
        {
            var ausencia = _context.Ausencias.Where(x => x.Id == id).FirstOrDefault();
            if (ausencia != null)
            {
                return ausencia;
            }

            return null;
        }
        public void AceptarAusencia(Ausencia ausencia)
        {
            var ausenciaAAceptar = _context.Ausencias.Where(x => x.Id == ausencia.Id).FirstOrDefault();
            if (ausenciaAAceptar != null)
            {
                ausenciaAAceptar.Justificada = "Si";
                _context.Entry(ausenciaAAceptar).State = EntityState.Modified;
                _context.SaveChanges();

            }
        }
        public void DenegarAusencia(Ausencia ausencia)
        {
            var ausenciaAAceptar = _context.Ausencias.Where(x => x.Id == ausencia.Id).FirstOrDefault();
            if (ausenciaAAceptar != null)
            {
                ausenciaAAceptar.Justificada = "No";
                _context.Entry(ausenciaAAceptar).State = EntityState.Modified;
                _context.SaveChanges();

            }
        }
        public IEnumerable<Ausencia> ObtenerTodosAsync()
        {
            return _context.Ausencias.ToList();
        }

        public Ausencia ObtenerUltimaAusenciaAgregada()
        {
            return _context.Ausencias.OrderByDescending(a => a.Id).FirstOrDefault();

        }
    }
}
