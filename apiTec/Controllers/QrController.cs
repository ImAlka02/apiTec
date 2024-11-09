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


namespace apiTec.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class QrController : Controller
    {
        public QrController()
        {
        }
        [HttpPost("Generar")]
        public ActionResult Generar(datosCredencialDTO datos)
        {
            try
            {
                if(datos == null) { return BadRequest("Los datos estan vacios"); }

                var validator = new datosCredencialDTOValidator();
                var result = validator.Validate(datos);

                if (!result.IsValid)
                {
                    return BadRequest(result.Errors.Select(x => x.ErrorMessage));
                }
                else
                {
                    string datosConcatenados = $"{datos.Vigencia}<<{datos.NumControl}<<{datos.NombreAlumno}<<{datos.Carrera}<<{datos.NSS}<<{datos.CURP}<<{datos.Periodo}";
                    AesEncrypter aes = new();
                    var datosEncriptados = aes.Encrypt(datosConcatenados)
                        .Replace("+", "-")
                        .Replace("/", "_");
                    QRCodeGenerator qr = new();
                    QRCodeData qrData = qr.CreateQrCode($"https://localhost:44325/api/Qr/Validar/{datosEncriptados}", QRCodeGenerator.ECCLevel.Q);
                    PngByteQRCode qrCode = new PngByteQRCode(qrData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);
                    using var stream = new MemoryStream(qrCodeImage);

                    return File(stream.ToArray(), "image/png", "QRCode.png");
                }

            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+e);
            }
            
        }

        [HttpGet("Validar/{qrDatos}")]
        public ActionResult Validar(string qrDatos)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(qrDatos)) { return BadRequest("El campo esta vacio. "); }
                AesEncrypter aes = new();
                string base64Datos = qrDatos.
                    Replace("-", "+")
                    .Replace("_", "/");

                var datos = aes.Decrypt(base64Datos);
                return Ok(datos);
            }
            catch (Exception e)
            {
                return Problem("Ocurrio un problema: "+ e);
            }
            
        }
    }
}
