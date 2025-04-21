using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace RecruitmentApp.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;

        public ImageService(IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
        {
            _env = env;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> DownloadImageAsync(string imageUrl, string folderPath)
        {
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(imageUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null; // Không tải được ảnh
            }

            var extension = Path.GetExtension(imageUrl);
            if (string.IsNullOrEmpty(extension) || extension.Length > 5) // Giới hạn kiểu file cho an toàn
            {
                extension = ".jpg"; // Mặc định
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var savePath = Path.Combine(_env.WebRootPath, folderPath);

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var filePath = Path.Combine(savePath, fileName);
            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            await File.WriteAllBytesAsync(filePath, fileBytes);

            // Trả về đường dẫn tương đối để lưu vào DB
            return $"/{folderPath}/{fileName}".Replace("\\", "/");
        }
    }
}