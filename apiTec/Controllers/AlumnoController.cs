using apiTec.Helpers;
using apiTec.Models;
using apiTec.Models.DTOs;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace apiTec.Controllers
{
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
            if (user == null) return BadRequest("El dto esta vacio");
            if (string.IsNullOrWhiteSpace(user.NumControl)) return BadRequest("Ingrese el numero de control. ");
            if (string.IsNullOrWhiteSpace(user.Contraseña)) return BadRequest("Ingrese la contraseña. ");

            try
            {
                user.Contraseña = AesEncrypter.Decrypt(user.Contraseña)!;
                var path = $"alumno/datosgenerales?control={user.NumControl}&password={user.Contraseña}";
                using HttpClient client = clientFactory.CreateClient("DataClient");
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    
                    var datosList = JsonConvert.DeserializeObject<List<DatoValor>>(responseBody);
                    var datosDictionary = new Dictionary<string, string>();
                    
                    foreach (var item in datosList)
                    {
                        datosDictionary[item.Dato] = item.Valor;
                    }

                    string resultJson = JsonConvert.SerializeObject(datosDictionary);
                    
                    return Ok(resultJson);
                }
                else
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: " + e);
            }
        }
    }
}
