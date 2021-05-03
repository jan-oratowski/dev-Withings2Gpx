using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Net.Http.Headers;
using Sportsy.Connections.Gpx;

namespace Sportsy.WebClient.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportGpxController : ControllerBase
    {
        private readonly IGpxParser _gpxParser = new GpxParser();

        //public ImportGpxController(IGpxParser gpxParser)
        //{
        //    _gpxParser = gpxParser;
        //}

        [HttpPost("[action]")]
        public void Save(IList<IFormFile> chunkFile, IList<IFormFile> UploadFiles)
        {
            long size = 0;
            try
            {
                foreach (var file in UploadFiles)
                {
                    var filename = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName
                        .Trim();
                    _gpxParser.Parse(file.OpenReadStream());

                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }
    }
}
