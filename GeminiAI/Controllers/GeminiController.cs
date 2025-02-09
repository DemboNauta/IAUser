using GeminiAI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace GeminiAI.Controllers
{
    [ApiController]
    [Route("api/gemini")]
    public class GeminiController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GeminiController(HttpClient httpClient, IConfiguration config)
        {
            this._httpClient = httpClient;
            this._config = config;
        }

        [HttpGet()]
        public async Task<ActionResult> Test([FromQuery] string query)
        {
            var apiKey = this._config.GetSection("Gemini").GetValue<string>("API_KEY");

            // Crear el contenido con el texto adecuado
            var content = new GeminiContent
            {
                Contents = new List<Content>
        {
            new Content
            {
                Parts = new List<Part>
                {
                    new Part { Text = $"{query} \n\n responde con contenido html no añadas información a parte del texto en sí, ni entrada ni despedida ni explicación, solo lo solicitado (no ```html ni cosas así)" }
                }
            }
        }
            };

            // Serializar el contenido en JSON
            string json = System.Text.Json.JsonSerializer.Serialize(content, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase // Aseguramos que use camelCase
            });
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // URL de la API de Gemini
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

            // Realizar la solicitud POST
            var response = await this._httpClient.PostAsync(url, httpContent);

            // Comprobar si la respuesta fue exitosa
            if (response.IsSuccessStatusCode)
            {
                // Deserializar la respuesta en un objeto GeminiResponse sin aplicar camelCase
                var responseContent = await response.Content.ReadAsStringAsync();
                var geminiResponse = System.Text.Json.JsonSerializer.Deserialize<GeminiResponse>(responseContent, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase  // Deshabilitar la conversión automática de nombres
                });

                // Verificar si la respuesta tiene candidatos y extraer el texto
                if (geminiResponse?.Candidates != null && geminiResponse.Candidates.Length > 0)
                {
                    // Acceder al primer candidato y su primer "Part"
                    var text = geminiResponse.Candidates[0].Content.Parts[0].Text;

                    return Ok(new { text }); // Devolver solo el texto
                }
                else
                {
                    return BadRequest("No candidates found in the response.");
                }
            }
            else
            {
                return BadRequest($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
            }
        }
    }

    }
