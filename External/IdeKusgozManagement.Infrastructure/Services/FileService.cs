using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class FileService(IUnitOfWork unitOfWork, IFileProvider fileProvider, ILogger<FileService> logger, IUserService userService) : IFileService
    {
        private readonly PhysicalFileProvider _physicalFileProvider = (PhysicalFileProvider)fileProvider;

        public async Task<ApiResponse<List<FileDTO>>> UploadFileAsync(List<UploadFileDTO> files, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!files.Any())
                {
                    return ApiResponse<List<FileDTO>>.Error("Dosya(lar) boş veya geçersiz");
                }

                var wwwrootPath = _physicalFileProvider.Root;
                if (string.IsNullOrEmpty(wwwrootPath))
                {
                    return ApiResponse<List<FileDTO>>.Error("wwwroot klasörü bulunamadı");
                }

                var uploadedFiles = new List<FileDTO>();
                var dateFolder = DateTime.Now.ToString("dd-MM-yyyy");

                foreach (var uploadFileDTO in files)
                {
                    // Her dosya için validasyon
                    if (uploadFileDTO.FormFile == null || uploadFileDTO.FormFile.Length == 0)
                    {
                        logger.LogWarning("Boş dosya atlandı: {FileName}", uploadFileDTO.FormFile?.FileName ?? "Unknown");
                        throw new Exception("Boş dosya yüklenemez");
                    }

                    var fileExtension = Path.GetExtension(uploadFileDTO.FormFile.FileName).ToLowerInvariant();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        logger.LogWarning("Geçersiz dosya formatı atlandı: {FileName}", uploadFileDTO.FormFile.FileName);
                        throw new Exception("Geçersiz dosya formatı");
                    }

                    if (uploadFileDTO.FileType == null)
                    {
                        uploadFileDTO.FileType = FileType.Other;
                    }

                    // Kullanıcı klasörü belirleme
                    var userFolder = "System";
                    if (!string.IsNullOrEmpty(uploadFileDTO.TargetUserId))
                    {
                        var userResult = await userService.GetUserByIdAsync(uploadFileDTO.TargetUserId);
                        if (userResult.IsSuccess && userResult.Data != null)
                        {
                            userFolder = userResult.Data.FullName;
                        }
                    }

                    // Dosya yolu oluştur
                    var newFileName = $"{NewId.NextGuid()}{fileExtension}";
                    var fullFolderPath = Path.Combine(
                        wwwrootPath,
                        "Uploads",
                        uploadFileDTO.FileType.ToFolderName(),
                        userFolder,
                        dateFolder);

                    if (!Directory.Exists(fullFolderPath))
                    {
                        Directory.CreateDirectory(fullFolderPath);
                    }

                    var uploadPath = Path.Combine(fullFolderPath, newFileName);

                    // Fiziksel dosyayı kaydet
                    await using var stream = new FileStream(uploadPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await uploadFileDTO.FormFile.CopyToAsync(stream, cancellationToken);

                    // Relative path
                    var relativePath = $"/Uploads/{uploadFileDTO.FileType.ToFolderName()}/{userFolder}/{dateFolder}/{newFileName}";

                    // Entity oluştur
                    var file = new IdtFile
                    {
                        Name = newFileName,
                        Path = relativePath,
                        OriginalName = uploadFileDTO.FormFile.FileName,
                        TargetUserId = uploadFileDTO.TargetUserId ?? null,
                        Type = uploadFileDTO.FileType
                    };

                    await unitOfWork.GetRepository<IdtFile>().AddAsync(file, cancellationToken);

                    var fileDTO = new FileDTO
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Path = file.Path,
                        OriginalName = file.OriginalName
                    };

                    uploadedFiles.Add(fileDTO);

                    logger.LogInformation("Dosya yüklendi: {FileName}, CreatedBy: {CreatedBy}", file.Name, file.CreatedBy);
                }

                // Tüm dosyalar işlendikten sonra tek seferde kaydet
                await unitOfWork.SaveChangesAsync(cancellationToken);

                if (!uploadedFiles.Any())
                {
                    return ApiResponse<List<FileDTO>>.Error("Hiçbir dosya yüklenemedi");
                }

                return ApiResponse<List<FileDTO>>.Success(uploadedFiles);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosya yüklenirken hata oluştu");
                throw;
            }
        }

        public async Task<ApiResponse<bool>> DeleteFileAsync(string fileId, CancellationToken cancellationToken = default)
        {
            try
            {
                var file = await unitOfWork.GetRepository<IdtFile>().GetByIdAsync(fileId, cancellationToken);
                if (file == null)
                {
                    logger.LogWarning("Dosya kaydı bulunamadı: {FileId}", fileId);
                    return ApiResponse<bool>.Error("Dosya kaydı bulunamadı");
                }
                var fullPath = Path.Combine(_physicalFileProvider.Root, file.Path.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    await unitOfWork.GetRepository<IdtFile>().RemoveAsync(fileId, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    logger.LogInformation("Dosya silindi: {FileId}", fileId);
                    return ApiResponse<bool>.Success(true, "Dosya silindi");
                }

                logger.LogWarning("Silinecek dosya bulunamadı: {FileId}", fileId);
                return ApiResponse<bool>.Error("Dosya bulunamadı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosya silinemedi: {FileId}", fileId);
                return ApiResponse<bool>.Error("Dosya silinemedi");
            }
        }

        public async Task<ApiResponse<FileDTO>> GetFileByIdAsync(string fileId, CancellationToken cancellationToken = default)
        {
            try
            {
                var file = await unitOfWork.GetRepository<IdtFile>().GetByIdAsync(fileId, cancellationToken);
                if (file == null)
                {
                    logger.LogWarning("Dosya kaydı bulunamadı: {FileId}", fileId);
                    return ApiResponse<FileDTO>.Success(null, "Dosya kaydı bulunamadı");
                }
                var fullPath = Path.Combine(_physicalFileProvider.Root, file.Path.TrimStart('/'));
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                var extension = Path.GetExtension(file.OriginalName).ToLowerInvariant();

                var contentType = GetContentType(extension);

                var fileDTO = new FileDTO
                {
                    Id = file.Id,
                    Name = file.Name,
                    Path = fullPath,
                    OriginalName = file.OriginalName,
                    FileStream = fileStream,
                    ContentType = contentType
                };

                return ApiResponse<FileDTO>.Success(fileDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosya okunamadı: {FileId}", fileId);
                return ApiResponse<FileDTO>.Error("Dosya okunamadı");
            }
        }

        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}