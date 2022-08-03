using AnyPasteSite.Data;
using AnyPasteSite.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AnyPasteSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository, IWebHostEnvironment env) => _repository = repository;


        public IActionResult Index() => View();

        public IActionResult Upload() => View();

        [HttpGet("/home/show/{fileName}")]
        public IActionResult Show(string fileName)
        {
            var uploadInfo = _repository.GetUploadInfo(fileName);

            if (uploadInfo == null)
                return NotFound();

            var isOwner = false;
            var ipAddr = HttpContext.Connection.RemoteIpAddress.ToString();

            if (uploadInfo.IpAddr == ipAddr)
                isOwner = true;

            var model = new ShowFileViewModel() { FileName = fileName, IsOwner = isOwner };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}