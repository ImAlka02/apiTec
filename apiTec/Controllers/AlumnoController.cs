using apiTec.Helpers;
using apiTec.Models;
using apiTec.Models.DTOs;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace apiTec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlumnoController : ControllerBase
{
    private readonly IHttpClientFactory clientFactory;

    public AlumnoController(IHttpClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    [HttpPost]
    public async Task<ActionResult> GetDataAlumnos(userDTO user)
    {
        try
        {
            // Desencriptar la contraseña
            user.Contraseña = AesEncrypter.Decrypt(user.Contraseña);
            var path = $"alumno/datosgenerales?control={user.NumControl}&password={user.Contraseña}";

            using HttpClient client = clientFactory.CreateClient("DataClient");
            HttpResponseMessage response = await client.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest(errorContent);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            // Verificar que el body no esté vacío
            if (string.IsNullOrEmpty(responseBody))
                return NotFound("No se encontró información.");

            // Deserializar el JSON a una lista de objetos
            var datosList = JsonConvert.DeserializeObject<List<DatoValor>>(responseBody);

            if (datosList == null || datosList.Count == 0)
                return NotFound("No se encontraron datos válidos.");

            // Crear diccionario
            var dictionary = datosList.ToDictionary(item => item.Dato.Replace(" ", "_").Replace(":",""), item => item.Valor);

            // Convertir el diccionario a JSON y reemplazar las entidades HTML
            string resultJson = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            resultJson = ReplaceHtmlEntities(resultJson);

            return Ok(resultJson);
        }
        catch (JsonException jsonEx)
        {
            return Problem("Error de deserialización: " + jsonEx.Message);
        }
        catch (Exception e)
        {
            return Problem("Ocurrió un problema: " + e.Message);
        }
    }

    private string ReplaceHtmlEntities(string input)
    {
        var replacements = new Dictionary<string, string>
        {
            { "&OACUTE;", "Ó" },
            { "&UACUTE;", "Ú" },
            { "&EACUTE;", "É" },
            { "&IACUTE;", "Í" },
            { "&AACUTE;", "Á" },
            { "&NTILDE;", "Ñ" },
            { "&oacute;", "ó" },
            { "&uacute;", "ú" },
            { "&eacute;", "é" },
            { "&iacute;", "í" },
            { "&aacute;", "á" },
            { "&ntilde;", "ñ" },
            { "&AMP;", "&" }
        };

        foreach (var pair in replacements)
        {
            input = input.Replace(pair.Key, pair.Value, StringComparison.OrdinalIgnoreCase);
        }

        return input;
    }
}
