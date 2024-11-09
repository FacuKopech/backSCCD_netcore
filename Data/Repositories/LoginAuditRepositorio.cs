using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class LoginAuditRepositorio : ILoginAuditRepositorie
    {
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();
        public void Agregar(LoginAudit entity)
        {
            var existingUsuario = _context.Usuarios.Local.FirstOrDefault(u => u.Id == entity.UsuarioLogueado.Id)
                                ?? _context.Usuarios.Find(entity.UsuarioLogueado.Id);

            if (existingUsuario == null)
            {
                _context.Attach(entity.UsuarioLogueado);
                _context.Entry(entity.UsuarioLogueado).Collection(u => u.Grupos).Load();
            }
            else
            {
                entity.UsuarioLogueado = existingUsuario;
            }
            foreach (var trackedUsuario in _context.Usuarios.Local.ToList())
            {
                if (trackedUsuario.Id != entity.UsuarioLogueado.Id)
                {
                    _context.Entry(trackedUsuario).State = EntityState.Detached;
                }
            }
            _context.LoginAudit.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(int id)
        {
            throw new NotImplementedException();
        }

        public void Modificar(LoginAudit entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public LoginAudit ObtenerAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LoginAudit> ObtenerLoginsDelMes()
        {
            DateTime currentDate = DateTime.Now;
            return _context.LoginAudit.Where(x => x.FechaYHoraLogin.Year == currentDate.Year && x.FechaYHoraLogin.Month == currentDate.Month &&
            x.FechaYHoraLogin.Day <= currentDate.Day).Include(x => x.UsuarioLogueado).ToList();
        }

        public IEnumerable<LoginAudit> ObtenerLoginsDeUsuario(int userId)
        {
            return _context.LoginAudit.Where(x => x.UsuarioLogueado.Id == userId).Include(x => x.UsuarioLogueado).ToList();
        }

        public IEnumerable<LoginAudit> ObtenerTodosAsync()
        {
            return _context.LoginAudit.Include(x => x.UsuarioLogueado).ToList();
        }
    }
}
