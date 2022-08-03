using Microsoft.AspNetCore.Mvc;

using AnypasteSite.Models;

using static AnypasteSite.Models.SiteFile;
using AnyPasteSite.Data;

namespace AnypasteSite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private static Random _random = new Random();
        private readonly string _fileStoragePath;
        private readonly IRepository _repository;
        private const string _fileSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";

        public static Dictionary<string, FileType> TypesDict = new Dictionary<string, FileType>
        {
            {".jpeg", FileType.Image },
            {".jpg", FileType.Image },
            {".png", FileType.Image },
            {".gif", FileType.Image },

            {".txt", FileType.Text },

            {".mp4", FileType.Video },
            {".avi", FileType.Video },

            {".mp3", FileType.Audio },
            {".wav", FileType.Audio },
            {".ogg", FileType.Audio },
        };

        public FilesController(IWebHostEnvironment appEnv, IRepository repository)
        {
            _env = appEnv;
            _fileStoragePath = $"{_env.WebRootPath}/FileStorage";
            _repository = repository;
        }

        [NonAction]
        public string GenerateFileName(IFormFile file)
        {
            string RandomString() => new string
                (
                Enumerable.Repeat(_fileSymbols, 10)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray()
                );

            string ext = Path.GetExtension(file.FileName);

            var name = RandomString() + ext;

            string path;
            do
                path = $"{_fileStoragePath}/{RandomString() + ext}";
            while (new FileInfo(path).Exists);

            return name;
        }

        [HttpGet("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            if (fileName != null)
            {
                var path = $"{_fileStoragePath}/{fileName}";
                var file = new FileInfo(path);

                if (!file.Exists)
                    return NotFound();

                if (!TypesDict.TryGetValue(file.Extension, out FileType type))
                    type = FileType.Other;

                return new JsonResult(new SiteFile($"/FileStorage/{fileName}", type));
            }

            return NotFound();
        }

        [HttpPost("Upload")]
        [RequestSizeLimit(6246400)]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null)
                return BadRequest();

            string fileName = GenerateFileName(file);
            string filePath = $"{_fileStoragePath}/{fileName}";

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fs);
                fs.Close();
            }

            var fileInfo = new FileInfo(fileName);

            if (!TypesDict.TryGetValue(fileInfo.Extension, out FileType type))
                type = FileType.Other;

            var remoteIpAddr = HttpContext.Connection.RemoteIpAddress.ToString();

            _repository.CreateUploadInfo(remoteIpAddr, fileName);
            _repository.SaveChanges();

            return new JsonResult(new SiteFile($"/FileStorage/{fileName}", type));
        }


        [HttpDelete("{fileName}")]
        public IActionResult Delete(string fileName)
        {
            if (fileName == null)
                return BadRequest();

            var info = _repository.GetUploadInfo(fileName);

            if (info == null)
                return BadRequest();

            var ipAddr = HttpContext.Connection.RemoteIpAddress.ToString();

            if (info.IpAddr != ipAddr)
                return Forbid();

            var fileInfo = new FileInfo($"wwwroot/FileStorage/{fileName}");

            if (!fileInfo.Exists)
                return BadRequest();

            fileInfo.Delete();
            return Ok();
        }

        [HttpGet]
        [Route("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var file = new FileInfo($"{_fileStoragePath}/{fileName}");

            if (!file.Exists)
                return NotFound();

            byte[] content = System.IO.File.ReadAllBytes(file.FullName);

            return File(content, "APPLICATION/octet-stream", fileName);
        }
    }
}
