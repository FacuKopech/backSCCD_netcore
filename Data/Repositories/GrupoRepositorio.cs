using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories
{
    public class GrupoRepositorio : IGrupoRepositorie
    {
        ApplicationDbContext _context = ApplicationDbContext.GetInstance();
        public GrupoRepositorio()
        {
        }
        public void Agregar(Grupo entity)
        {
            _context.Grupos.Add(entity);
            _context.SaveChanges();
        }

        public void AgregarUserAGrupo(Usuario user)
        {
            var usuario = _context.Usuarios.Where(x => x.Id == user.Id).Include(grupos => grupos.Grupos).FirstOrDefault();
            if (usuario != null)
            {
                foreach (var grupo in usuario.Grupos)
                {
                    var grupoDeUser = _context.Grupos.Where(x => x.Id == grupo.Id).FirstOrDefault();
                    if (grupoDeUser != null)
                    {
                        if (grupoDeUser.Usuarios.Contains(usuario) == false)
                        {
                            grupoDeUser.Usuarios.Add(usuario);
                            _context.Entry(grupoDeUser).State = EntityState.Modified;
                            _context.SaveChanges();
                        }
                    }
                }
            }          
        }
        public void Borrar(int id)
        {
            throw new NotImplementedException();
        }

        public void Modificar(Grupo entity)
        {
            throw new NotImplementedException();
        }

        public Grupo ObtenerAsync(int id)
        {
            return _context.Grupos.Where(x => x.Id == id).Include(usuarios => usuarios.Usuarios).FirstOrDefault();
        }
        public IEnumerable<Grupo> ObtenerTodosAsync()
        {
            return _context.Grupos.Include(users => users.Usuarios).ToList();
        }
    }
}
