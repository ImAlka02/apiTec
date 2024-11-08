using apiTec.Models.DTO_s;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using apiTec.Helpers;

namespace apiTec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : Controller
    {
        [HttpPost]
        public ActionResult Encryption(contraDTO contra) 
        {
            try
            {
                if(contra == null) { return BadRequest("El dto esta vacio."); }
                if (string.IsNullOrWhiteSpace(contra.contraseña)) { return BadRequest("Ingrese la contraseña. "); }

                var contraEncriptada = Encriptar.StringToSHA512(contra.contraseña);

                contraDTO c = new()
                {
                    contraseña = contraEncriptada
                };
                
                return Ok(c);
            }
            catch (Exception e)
            {
                return BadRequest("Ocurrio este problema: " + e);
            }
        }
    }

    
}
