using Model.Entities;

namespace SCCD.Services.CompositePattern
{
    public interface IAuthentication
    {
        Usuario Authenticate(object credentials);
    }
}
