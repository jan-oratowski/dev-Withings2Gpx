using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Sportsy.Connections.Gpx;
using Sportsy.Data.Database;
using Sportsy.Services.ActivityToolService;
using System;
using System.Collections.Generic;

namespace Sportsy.WebClient.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportGpxController : ControllerBase
    {
        private readonly SportsyContext _context;
        private readonly IGpxParser _gpxParser;
        private readonly IActivityToolService _activityTool;

        public ImportGpxController(SportsyContext context, IGpxParser gpxParser, IActivityToolService activityTool)
        {
            _context = context;
            _gpxParser = gpxParser;
            _activityTool = activityTool;
        }

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
                    var fromGpx = _gpxParser.Parse(file.OpenReadStream());
                    var activity = _activityTool.CreateFrom(fromGpx);
                    _context.Activities.Add(activity);
                    _context.SaveChanges();
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
