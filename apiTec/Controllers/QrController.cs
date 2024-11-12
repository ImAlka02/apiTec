using apiTec.Models.DTO_s;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using QRCoder;
using Microsoft.AspNetCore.DataProtection;
using apiTec.Helpers;
using System.Diagnostics.Contracts;
using System.Text.Json;
using System.Text;
using apiTec.Models.Validators;
using apiTec.Helpers;
using apiTec.Models.DTOs;
using System.Net.Http.Headers;


namespace apiTec.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class QrController : Controller
    {
        private readonly IHttpClientFactory clientFactory = null!;

        public QrController(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        [HttpPost("Generar")]
        public ActionResult Generar(userDTO user)
        {

            if (user == null) return BadRequest("El dto esta vacio");
            if (string.IsNullOrWhiteSpace(user.NumControl)) return BadRequest("Ingrese el numero de control. ");
            if (string.IsNullOrWhiteSpace(user.Contraseña)) return BadRequest("Ingrese la contraseña. ");

            string data = $"{user.NumControl}<<{user.Contraseña}";

            try
            {
                var datosEncriptados = AesEncrypter.Encrypt(data).Replace("+", "-").Replace("/", "_");
                QRCodeGenerator qr = new();
                QRCodeData qrData = qr.CreateQrCode($"https://panel.halcon.labsystec.net/{datosEncriptados}", QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);
                using var stream = new MemoryStream(qrCodeImage);

                return File(stream.ToArray(), "image/png", "QRCode.png");
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+e);
            }
            
        }

        [HttpGet("Validar/{qrDatos}")]
        public async Task<ActionResult> Validar(string qrDatos)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(qrDatos)) { return BadRequest("El campo esta vacio. "); }
                string datosEncriptados = qrDatos
                    .Replace("-", "+")
                    .Replace("_", "/");

                var datos = AesEncrypter.Decrypt(datosEncriptados).Split("<<");
                var contraDesencriptada = AesEncrypter.Decrypt(datos[1]);

                try
                {
                    var path = $"alumno/datosgenerales?control={datos[0]}&password={contraDesencriptada}";
                    using HttpClient client = clientFactory.CreateClient("DataClient");
                    HttpResponseMessage response = await client.GetAsync(path);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync();
                        var respuestDTO = new { resp = responseBody.Result, numControl = datos[0] };
                        return Ok(respuestDTO);
                    }
                    else
                    {
                        return BadRequest("INVALIDO");
                    }
                }
                catch (Exception e)
                {
                    return Problem("INVALIDO");
                }
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+ e);
            }
            
        }
    }
}
