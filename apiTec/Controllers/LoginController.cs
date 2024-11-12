using apiTec.Helpers;
using apiTec.Models.DTO_s;
using apiTec.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace apiTec.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory clientFactory;

        public LoginController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync(userDTO user)
        {
            if (user == null) return BadRequest("El dto esta vacio");
            if (string.IsNullOrWhiteSpace(user.NumControl)) return BadRequest("Ingrese el numero de control. ");
            if (string.IsNullOrWhiteSpace(user.Contraseña)) return BadRequest("Ingrese la contraseña. ");

            try
            {
                var path = $"alumno/datosgenerales?control={user.NumControl}&password={user.Contraseña}";
                using HttpClient client = clientFactory.CreateClient("DataClient");
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync();
                    var respuestDTO = new { resp = responseBody.Result, contra = AesEncrypter.Encrypt(user.Contraseña) };
                    return Ok(respuestDTO);
                }
                else
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+e);
            }
            
        }


        [HttpPost("DecryptPassword")]
        public ActionResult PasswordDecrypter(contraDTO contra)
        {
            try
            {
                if (contra == null)
                    return BadRequest();
                if (string.IsNullOrWhiteSpace(contra.contraseña))
                    return BadRequest();
                var contraseñaDesencriptada = AesEncrypter.Decrypt(contra.contraseña);
                return Ok(contraseñaDesencriptada);
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: " + e);
            }
        }

    }
}
