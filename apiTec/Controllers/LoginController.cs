﻿using apiTec.Helpers;
using apiTec.Models.DTO_s;
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
        
        [HttpPost]
        public ActionResult Login(userDTO user)
        {
            if(user == null || string.IsNullOrWhiteSpace(user.NumControl) || string.IsNullOrWhiteSpace(user.Contraseña))
            {
                return BadRequest("El número de control y la contraseña son requeridos");
            }

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
                            
                            string responseBody = objReader.ReadToEnd();
                            var respuestDTO = new { resp = responseBody, contra = AesEncrypter.Encrypt(user.Contraseña) };
                            return Ok(respuestDTO);
                        }
                    }
                }
            }
            catch(WebException ex)
            {
                if(ex.Response is HttpWebResponse errorResponse)
                {
                    var statusCode = errorResponse.StatusCode;
                    string mensaje = "Error al obtener datos";

                    if(statusCode == HttpStatusCode.BadRequest)
                    {
                        return BadRequest("El número de control o la contraseña no son válidos");
                    }

                    return StatusCode((int)statusCode, mensaje);
                }
                return Problem("Ocurrio un problema: " + ex);
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+ e);
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
