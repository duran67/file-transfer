using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FileTransfer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace FileTransfer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly IFileProvider _fileProvider;
        private readonly IContentTypeProvider _contentTypeProvider;

        public FilesController(IFileProvider fileProvider, IContentTypeProvider contentTypeProvider)
        {
            _fileProvider = fileProvider;
            _contentTypeProvider = contentTypeProvider;
        }

        /// <summary>
        /// Get files with optional regex pattern matching
        /// </summary>
        /// <param name="regex"></param>
        /// <example>GET files[?regex=<pattern>]</example>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(string regex = ".*?")
        {
            var contents = _fileProvider.GetDirectoryContents("");
            var matcher = new Regex(regex);
            var files = contents
                .Where(f => f.Exists && !f.IsDirectory && matcher.IsMatch(f.Name))
                .Select(f => new FileInformation(f))
                .ToList();
            return Ok(files);
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="fileName"> file to download</param>
        /// <example>GET files/filename</example>
        /// <returns>FileStreamResult</returns>
        [HttpGet("{fileName}")]
        public IActionResult Download(string fileName)
        {
            var fileInfo = _fileProvider.GetFileInfo(fileName);
            var readStream = fileInfo.CreateReadStream();
            if (!_contentTypeProvider.TryGetContentType(fileName, out var mimeType))
                mimeType = "application/octet-stream";
            return File(readStream, mimeType, fileName);
        }
    }
}
