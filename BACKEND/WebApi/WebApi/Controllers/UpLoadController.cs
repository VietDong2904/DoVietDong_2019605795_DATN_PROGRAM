using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApi.Controllers
{
    public class UpLoadController : ApiBaseController
    {
        /// <summary>
        /// Upload file
        /// </summary> 
        /// <param name="id">File</param>
        /// <returns>UrlFile</returns> 
        /// <response code="200">Thành công</response>
        [HttpPost, Route(""), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadLogo()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = System.IO.File.Create(fullPath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok(new { dbPath });
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return StatusCode(500, "Internal Server");
            }
        }
    }
}
