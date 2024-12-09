using Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Model.Entities;
using System.Text;
using System.Text.Json;

namespace SCCD.Command.Ausencia
{
    public class AgregarAusenciaCommand : IAusenciaCommand
    {
        private readonly HttpClient _httpClient;
        private Session _session;

        public AgregarAusenciaCommand(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _session = Session.GetInstance();
        }

        public bool AgregarAusencia(Guid idHijo, AusenciaModificar nuevaAusencia)
        {
            try
            {
                var apiUrl = $"https://localhost:7092/Ausencias/AgregarAusencia/{idHijo}";
                if (string.IsNullOrEmpty(_session.Token))
                {
                    throw new InvalidOperationException("Token is missing. Please log in first.");
                }
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.Token);
                var ausenciaJson = JsonSerializer.Serialize(nuevaAusencia);
                var content = new StringContent(ausenciaJson, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync(apiUrl, content).Result;

                return response.IsSuccessStatusCode;                
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
