using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;     
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace TradesCompany_AW.Web.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            // ✅ Validate extension
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException("Invalid file extension. Only JPG, JPEG, PNG allowed.");

            // ✅ Validate content type
            if (!file.ContentType.StartsWith("image/"))
                throw new ArgumentException("Invalid content type. Only image files allowed.");

            // ✅ Validate size
            if (file.Length > _maxFileSize)
                throw new ArgumentException("File size exceeds 5MB limit.");

            // ✅ Generate unique file name
            var fileName = $"{Guid.NewGuid()}{extension}";

            // ✅ Save folder path
            var folderPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            // ✅ Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // ✅ Return relative path to store in database (e.g., "/uploads/xyz.jpg")
            return $"/uploads/{fileName}";
        }
    }
}
