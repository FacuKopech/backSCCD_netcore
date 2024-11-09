using Model.Entities;

namespace Data.Contracts
{
    public interface IUsuarioRepositorie : IGenericRepositorie<Usuario>
    {
        Task<bool> CambiarContraseña(Usuario usuario);

        Task<Usuario> LogIn(string email, string clave);

        Usuario ObtenerUserWthGroups(string username, string clave);

        Usuario ObtenerUserWthGroupsWithEmail(string email);
        void ActualizarClaveUser(int idUser, string claveNueva);
        void RecuperarClaveUser(string email, string claveNueva);   
        bool UsuarioExistente(string email);
        IEnumerable<Usuario> ObtenerUsuariosSistema();
        IEnumerable<Grupo> ObtenerRolesSistema();
        void AgregarGrupoDefaultUser(Usuario user);
    }
}
