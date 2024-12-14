using Model.CompositePattern;
using Model.Entities;

namespace SCCD.Services.CompositePattern
{
    public class CompositeAuth : IAuthentication
    {
        private readonly List<IAuthentication> _authMethods = new();

        public void AddAuthMethod(IAuthentication authMethod)
        {
            _authMethods.Add(authMethod);
        }
        public Usuario Authenticate(object credentials)
        {
            foreach (var authMethod in _authMethods)
            {
                var user = authMethod.Authenticate(credentials);
                if ( user != null)
                {
                    return user;
                }
            }
            return null;
        }
    }
}
