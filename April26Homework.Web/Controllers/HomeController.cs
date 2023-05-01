using April26Homework.Data;
using April26Homework.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace April26Homework.Web.Controllers
{
    public class HomeController : Controller
    {
        private IWebHostEnvironment _webHostEnvironment;
        private string _connectionString;
        public HomeController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionString = configuration.GetConnectionString("ConStr");
        }

        public IActionResult Index()
        {
            var repo = new ImageRepository(_connectionString);
            HomeViewModel model = new()
            {
                Images = repo.GetImages() 
            };
            return View(model);
        }
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile imageFile, string title)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var stream = new FileStream(fullPath, FileMode.CreateNew);
            imageFile.CopyTo(stream);
            Image image = new()
            {
                FileName = fileName,
                Title = title,
                Date = DateTime.Now,
            };
            var repo = new ImageRepository(_connectionString);
            repo.AddImage(image);
            return Redirect("/");
        }
        public IActionResult ViewImage(int id)
        {
            var repo = new ImageRepository(_connectionString);
            ViewImageViewModel model = new()
            {
                Image = repo.GetImage(id),
                LikedImages = HttpContext.Session.Get<List<int>>("likedImages") ?? new List<int>()
            };
            return View(model);
        }
        [HttpPost]
        public void AddLike(int id)
        {
            var repo = new ImageRepository(_connectionString);
            repo.AddLike(id);
            List<int> likedImages = HttpContext.Session.Get<List<int>>("likedImages") ?? new List<int>();
            likedImages.Add(id);
            HttpContext.Session.Set("likedImages", likedImages);
            
        }
        public IActionResult GetLikes(int id)
        {
            var repo = new ImageRepository(_connectionString);
            int likes = repo.GetLikes(id);
            return Json(likes);
        }
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}