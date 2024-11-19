using Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace SCCD.Command.Ausencia
{
    public class AgregarAusenciaCommand : IAusenciaCommand
    {
        private readonly HttpClient _httpClient;

        public AgregarAusenciaCommand(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool AgregarAusencia(Guid idHijo, AusenciaModificar nuevaAusencia)
        {
            try
            {
                var apiUrl = $"https://localhost:7092/Ausencias/AgregarAusencia/{idHijo}";

                var ausenciaJson = JsonSerializer.Serialize(nuevaAusencia);
                var content = new StringContent(ausenciaJson, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync(apiUrl, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
