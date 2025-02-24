using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.DataAccess;
using ToDo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.CodeAnalysis;

namespace ToDo.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ItemController : Controller
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        public IActionResult Index()
        {
            var items = dbContext.Items;
            return View(items.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var items = dbContext.Items;

            return View(items.ToList());
        }
        [HttpPost]
        public IActionResult Create(Item item , IFormFile? file)
        {
            if (file != null && file.Length>0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                // Save img name in db
                item.File = fileName;
            }
            if (item != null)
            {
                //dbContext.Items.Add(new Item
                //{
                //    Title = item.Title,
                //    Description = item.Description,
                //    File = item.File,
                //    DeadLine = item.DeadLine

                //});
                dbContext.Items.Add(item);
                dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("NotFoundPage", nameof(Index)); 

        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var item= dbContext.Items.FirstOrDefault(e=> e.Id == Id);
            
            if(item != null)
            {
                return View(item);
            }
            return RedirectToAction("NotFoundPage", "Home");
        }
     
        [HttpPost]
        public IActionResult Edit(Item item, IFormFile? file)
        {
            var itemInDb = dbContext.Items.AsNoTracking().FirstOrDefault(e => e.Id == item.Id);

            if (itemInDb != null && file != null && file.Length > 0)
            {
                // Save img in wwwroot
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", itemInDb.File);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Save img name in db
                item.File = fileName;
            }
            else
                item.File = itemInDb.File;

            if (item != null)
            {
                dbContext.Items.Update(item);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("NotFoundPage", "Home");
        }


        public IActionResult DeleteFile(int Id)
        {
            var item = dbContext.Items.FirstOrDefault(e => e.Id == Id);

            if (item != null)
            {
                // Delete old img from wwwroot
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.File);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                // Delete img name in db
                item.File = null;
                dbContext.SaveChanges();

                return RedirectToAction("Edit", new { Id });
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        public IActionResult Delete(int Id)
        {
            var item = dbContext.Items.FirstOrDefault(e => e.Id == Id);

            if (item != null)
            {
                // Delete old img from wwwroot
                if (item.File != null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.File);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                // Delete img name in db
                dbContext.Items.Remove(item);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return RedirectToAction("NotFoundPage", "Home");
        }

        public IActionResult Download(int Id)
        {
            var item = dbContext.Items.FirstOrDefault(e => e.Id == Id);

            // Check if the item exists and has a valid file associated with it
            if (item != null && !string.IsNullOrEmpty(item.File))
            {
                // Build the full file path
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", item.File);

                // Check if the file exists on the server
                if (System.IO.File.Exists(filePath))
                {
                    // Read the file as bytes
                    var fileBytes = System.IO.File.ReadAllBytes(filePath);

                    // Get the file extension and determine the content type
                    var fileExtension = Path.GetExtension(item.File).ToLower();
                    var contentType = fileExtension switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".bmp" => "image/bmp",
                        ".pdf" => "application/pdf",
                        _ => "application/octet-stream", // Default MIME type
                    };

                    // Return the file to the user for download
                    return File(fileBytes, contentType, item.File);
                }
            }

            // If no file exists or the item is not found, redirect to the NotFoundPage
            return RedirectToAction("NotFoundPage", "Home");
        }


        public IActionResult SomeAction()
        {
            // Set a temporary message
            TempData["SuccessMessage"] = "Welcome Esraa !";

            return RedirectToAction("Index");  // Or return View();
        }


    }

}
