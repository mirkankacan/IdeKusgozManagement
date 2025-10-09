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

        public async Task<ApiResponse<FileDTO>> UploadFileAsync(UploadFileDTO uploadFileDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                // Validasyonlar
                if (uploadFileDTO == null || uploadFileDTO.FormFile.Length == 0)
                {
                    return ApiResponse<FileDTO>.Error("Dosya boş veya geçersiz");
                }
                var fileExtension = Path.GetExtension(uploadFileDTO.FormFile.FileName).ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(fileExtension))
                {
                    return ApiResponse<FileDTO>.Error("Geçersiz dosya formatı");
                }
                var wwwrootPath = _physicalFileProvider.Root;
                if (string.IsNullOrEmpty(wwwrootPath))
                {
                    return ApiResponse<FileDTO>.Error("wwwroot klasörü bulunamadı");
                }
                if (uploadFileDTO.FileType == null)
                {
                    uploadFileDTO.FileType = FileType.Other;
                }

                // Dosya yolu oluştur
                var newFileName = $"{NewId.NextGuid()}{fileExtension}";
                var dateFolder = DateTime.Now.ToString("dd-MM-yyyy");

                var userFolder = "Anonymous";
                if (!string.IsNullOrEmpty(uploadFileDTO.TargetUserId))
                {
                    var user = await userService.GetUserByIdAsync(uploadFileDTO.TargetUserId);
                    userFolder = user.Data.FullName;
                }

                var fullFolderPath = Path.Combine(
                    wwwrootPath,
                    "Uploads",
                    uploadFileDTO.FileType?.ToFolderName(),
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
                var relativePath = $"/Uploads/{uploadFileDTO.FileType?.ToFolderName()}/{userFolder}/{dateFolder}/{newFileName}";

                // Entity oluştur
                var mappedFile = new IdtFile
                {
                    Name = newFileName,
                    Path = relativePath,
                    OriginalName = uploadFileDTO.FormFile.FileName
                };

                await unitOfWork.GetRepository<IdtFile>().AddAsync(mappedFile, cancellationToken);


                await unitOfWork.SaveChangesAsync(cancellationToken);


                var fileDTO = new FileDTO
                {
                    Id = mappedFile.Id,
                    Name = mappedFile.Name,
                    Path = mappedFile.Path,
                    OriginalName = mappedFile.OriginalName,
                };

                return ApiResponse<FileDTO>.Success(fileDTO);
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
    }
}