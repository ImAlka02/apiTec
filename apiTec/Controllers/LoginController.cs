using apiTec.Helpers;
using apiTec.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace apiTec.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class LoginController : Controller
    {
        [HttpPost("Login")]
        public ActionResult Login(userDTO user)
        {
            var url = $"https://sie.itesrc.net/api/alumno/datosgenerales?control={user.NumControl}&password={user.Contraseña}";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return BadRequest();
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            AesEncrypter aes = new AesEncrypter();
                            string responseBody = objReader.ReadToEnd();
                            var respuestDTO = new { resp = responseBody, contra = aes.Encrypt(user.Contraseña) };
                            return Ok(respuestDTO);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+ e);
            }
        }
    }
}
