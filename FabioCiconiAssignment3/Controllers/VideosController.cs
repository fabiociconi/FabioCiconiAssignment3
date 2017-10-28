using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FabioCiconiAssignment3.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FabioCiconiAssignment3.Controllers
{
    public class VideosController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public VideosController(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        public IActionResult List()
        {
            Videos vd = new Videos();
            List<Videos> m = vd.ListVideos();
            ViewBag.Caralho = m;

            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Add()
        {
            //chama a tela de Upload
            return View();
        }
        [HttpPost]
        public IActionResult AddVideos(Videos videosUpload, IFormFile video)
        {
           // LoginViewModel ll = new LoginViewModel();

         
                                
                var userLog = HttpContext.Session.GetString("logadoUser");
            if (userLog == null)
            {
                return RedirectToAction("Index", "Login");
            }


            else
            {
                if (ModelState.IsValid)
                {
                    var filePath = Path.GetTempPath();
                    using (var stream = new FileStream(Path.Combine(Path.GetTempPath(), video.FileName), FileMode.Create))

                    {
                        videosUpload.LocalVideo = filePath + @"\" + video.FileName;
                    }
                    videosUpload.UploadVideos();

                    return RedirectToAction("Add", "Videos");
                }
                else
                {
                    return View("Add", videosUpload);
                }
            }
             
        }
        public IActionResult DownloadVideo(string fileName)
        {
            Videos videosDownload = new Videos();
            string fullpath = Path.Combine(_hostingEnvironment.WebRootPath, fileName);
            videosDownload.DestinationVideo = fullpath;
            videosDownload.IdVideo = fileName;
            videosDownload.DownloadVideoFromGoogleCloud();

            return RedirectToAction("List", "Videos");
        }
        [HttpPost]
        public IActionResult AddCommenty(string id, string Desc, string userId)
        {

           
                
                var userLog = HttpContext.Session.GetString("logadoUser");
            if (userLog == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            { 
                Commentaries comments = new Commentaries
                {
                    IdVideo = id,
                    User = userLog,
                    Desc = Desc
                };
                comments.InsertComment();

                return RedirectToAction("List", "Videos");
            }
            
                
            
            //return RedirectToAction("Index", "Home");

        }
    }
}