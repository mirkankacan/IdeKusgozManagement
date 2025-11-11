using Microsoft.AspNetCore.Http;

namespace IdeKusgozManagement.Infrastructure.Helpers
{
    public static class FileHelper
    {
        public static async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }

        // Overload - birden fazla dosya için
        public static async Task<List<byte[]>> ConvertToByteArrayAsync(IEnumerable<IFormFile> files)
        {
            var results = new List<byte[]>();

            foreach (var file in files)
            {
                results.Add(await ConvertToByteArrayAsync(file));
            }

            return results;
        }

        public static async Task<BinaryData> ConvertToBinaryDataAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BinaryData.FromBytes(Array.Empty<byte>());

            using var stream = file.OpenReadStream();
            var binaryData = await BinaryData.FromStreamAsync(stream);
            return binaryData;
        }

        public static string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }
    }
}