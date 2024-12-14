using Data.Contracts;
using Model.CompositePattern;
using Model.Entities;

namespace SCCD.Services.CompositePattern
{
    public class NormalLoginAuth : IAuthentication
    {
        private readonly IUsuarioRepositorie _usuariosRepositorie;

        public NormalLoginAuth(IUsuarioRepositorie usuariosRepositorie)
        {
            _usuariosRepositorie = usuariosRepositorie;
        }

        public Usuario Authenticate(object credentials)
        {
            var loginCredentials = credentials as LoginRequest;
            if (loginCredentials == null) return null;

            var user = _usuariosRepositorie.ObtenerUserWthGroups(loginCredentials.Username, loginCredentials.Clave);
            return user;
        }
    }
}
