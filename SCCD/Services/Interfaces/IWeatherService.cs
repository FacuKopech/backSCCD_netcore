using Microsoft.AspNetCore.Mvc;

namespace SCCD.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<(bool IsSuccess, string Message, string Data)> ObtenerClima(string localidad, string fecha);
    }
}
