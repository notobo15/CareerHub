using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecruitmentApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Controllers
{
    [Route("images")]
    public class ImagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ImagesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile filepond)
        {
            if (filepond == null || filepond.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "sliders");
                Directory.CreateDirectory(uploads); // Không cần check exists vì method này idempotent

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(filepond.FileName)}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await filepond.CopyToAsync(stream);
                }

                var image = new Image
                {
                    FileName = fileName,
                    FullPath = filePath,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                // FilePond mong đợi response dạng này
                return Ok(image.ImageId.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpDelete("revert")]
        public async Task<IActionResult> Revert([FromBody] string id)
        {
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int imageId))
                return BadRequest("Invalid image ID");

            var image = await _context.Images.FindAsync(imageId);
            if (image == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "images", "sliders", image.FileName);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("load/{id}")]
        public IActionResult Load(int id)
        {
            var image = _context.Images.FirstOrDefault(x => x.ImageId == id && !x.IsDeleted);
            if (image == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "images", "sliders", image.FileName);
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "image/jpeg"); // Điều chỉnh MIME type theo thực tế
        }
    }
}
