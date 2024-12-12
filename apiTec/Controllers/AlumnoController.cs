using apiTec.Helpers;
using apiTec.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                    var responseBody = response.Content.ReadAsStringAsync();
                    return Ok(responseBody.Result);
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
