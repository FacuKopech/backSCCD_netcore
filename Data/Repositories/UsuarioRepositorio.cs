using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories
{
    public class UsuarioRepositorio : IUsuarioRepositorie
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }
        public UsuarioRepositorio()
        {
        }   
        public void Agregar(Usuario entity)
        {
            _context.Usuarios.Add(entity);
            _context.SaveChanges();
        }

        public void Borrar(Guid id)
        {
            var user = _context.Usuarios.Where(x => x.Id == id).FirstOrDefault();
            if (user != null)
            {                           
                _context.Usuarios.Remove(user);
                _context.Entry(user).State = EntityState.Deleted;
                _context.SaveChanges();
            }            
        }

        public Task<bool> CambiarContraseña(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario> LogIn(string email, string clave)
        {
            throw new NotImplementedException();
        }

        public void Modificar(Usuario entity)
        {
            var user = _context.Usuarios.Where(x => x.Id == entity.Id).FirstOrDefault();
            var gruposUser = _context.Grupos.Where(x => entity.Grupos.Contains(x)).ToList();
            if (user != null)
            {
                user.Email = entity.Email;
                user.Username = entity.Username;
                user.Clave = entity.Clave;
                user.Grupos = gruposUser;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
            }            
        }

        public Usuario ObtenerAsync(Guid id)
        {
            var usuario = _context.Usuarios.Where(x => x.Id == id).Include(u => u.Grupos).FirstOrDefault();

            if (usuario != null)
            {
                return usuario;
            }
            return null;
        }

        public Usuario ObtenerUserWthGroups(string username, string clave)
        {
            var userWGroups = _context.Usuarios.Where(x => x.Email == username && x.Clave == clave)
                .Include(g => g.Grupos)
                .FirstOrDefault();

            return userWGroups;
        }

        public IEnumerable<Usuario> ObtenerTodosAsync()
        {
            return _context.Usuarios.ToList();
        }
      
        public void ActualizarClaveUser(Guid idUser, string claveNueva)
        {
            var user = _context.Usuarios.Where(x => x.Id == idUser)
               .FirstOrDefault();
            if (user != null)
            {
                user.Clave = claveNueva;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();

            }
            else
            {
                throw new Exception("No se encontro el usuario.");
            }
            
        }

        public bool UsuarioExistente(string email)
        {
            var user = _context.Usuarios.Where(x => x.Email == email)
                .FirstOrDefault();
            if (user != null)
            {
                return true;
            }

            return false;
        }

        public void RecuperarClaveUser(string email, string claveNueva)
        {
            var user = _context.Usuarios.Where(x => x.Email == email)
                .FirstOrDefault();
            if (user != null)
            {
                foreach (var grupo in user.Grupos)
                {
                    _context.Entry(grupo).State = EntityState.Detached;
                }
                user.Clave = claveNueva;
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public IEnumerable<Usuario> ObtenerUsuariosSistema()
        {
            return _context.Usuarios.Include(user => user.Grupos).ToList();
        }

        public IEnumerable<Grupo> ObtenerRolesSistema()
        {
            return _context.Grupos.ToList();
        }

        public void AgregarGrupoDefaultUser(Usuario user)
        {
            var usuario = _context.Usuarios.Where(x => x.Id == user.Id)
                .FirstOrDefault();
            if (usuario != null)
            {
                usuario.Grupos = user.Grupos;
                _context.Entry(usuario).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public Usuario ObtenerUserWthGroupsWithEmail(string email)
        {
            var userWGroups = _context.Usuarios.Where(x => x.Email == email)
                .Include(g => g.Grupos)
                .FirstOrDefault();

            return userWGroups;
        }
    }
}
