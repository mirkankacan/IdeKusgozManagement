using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Infrastructure.Helpers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class FileService(IUnitOfWork unitOfWork, IFileProvider fileProvider, ILogger<FileService> logger, IUserService userService, IDocumentService documentService, IAIService aiService) : IFileService
    {
        private readonly PhysicalFileProvider _physicalFileProvider = (PhysicalFileProvider)fileProvider;

        public async Task<ApiResponse<List<FileDTO>>> UploadFileAsync(List<UploadFileDTO> files, CancellationToken cancellationToken = default)
        {
            try
            {
                var wwwrootPath = _physicalFileProvider.Root;
                if (string.IsNullOrEmpty(wwwrootPath))
                {
                    return ApiResponse<List<FileDTO>>.Error("wwwroot klasörü bulunamadı");
                }

                var dateFolder = DateTime.Now.ToString("dd-MM-yyyy");
                var newFiles = new List<IdtFile>();
                var uploadedFiles = new List<FileDTO>();

                foreach (var file in files)
                {
                    // Dosya validasyonu
                    ValidateFile(file);

                    // Folder bilgilerini al
                    var (userFolder, documentTypeName, renewalPeriodInMonths) = await GetFolderInfoAsync(file, cancellationToken);

                    // Dosya yükleme işlemi
                    var (newFile, fileDTO) = await ProcessFileUploadAsync(file, wwwrootPath, documentTypeName, renewalPeriodInMonths, userFolder, dateFolder, cancellationToken);

                    newFiles.Add(newFile);
                    uploadedFiles.Add(fileDTO);

                    logger.LogInformation("Dosya yüklendi: {FileName}, CreatedBy: {CreatedBy}", newFile.Name, newFile.CreatedBy);
                }

                // Tüm dosyaları tek seferde veritabanına ekle
                await unitOfWork.GetRepository<IdtFile>().AddRangeAsync(newFiles, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<List<FileDTO>>.Success(uploadedFiles);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosya yüklenirken hata oluştu");
                throw;
            }
        }

        private void ValidateFile(UploadFileDTO file)
        {
            if (file.FormFile == null || file.FormFile.Length == 0)
            {
                logger.LogWarning("Boş dosya atlandı: {FileName}", file.FormFile?.FileName ?? "Unknown");
                throw new Exception("Boş dosya yüklenemez");
            }

            var fileExtension = Path.GetExtension(file.FormFile.FileName).ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                logger.LogWarning("Geçersiz dosya formatı atlandı: {FileName}", file.FormFile.FileName);
                throw new Exception("Geçersiz dosya formatı");
            }
        }

        private async Task<(string userFolder, string documentTypeName, int? renewalPeriodInMonths)> GetFolderInfoAsync(UploadFileDTO file, CancellationToken cancellationToken)
        {
            var userFolder = "Firma";
            if (!string.IsNullOrEmpty(file.TargetUserId))
            {
                var userResult = await userService.GetUserByIdAsync(file.TargetUserId);
                if (userResult.IsSuccess && userResult.Data != null)
                {
                    userFolder = userResult.Data.FullName;
                }
            }

            var documentTypeName = "";
            int? renewalPeriodInMonths = null;
            var departmentResult = await documentService.GetDocumentTypeByIdAsync(file.DocumentTypeId, cancellationToken);
            if (departmentResult.IsSuccess && departmentResult.Data != null)
            {
                documentTypeName = departmentResult.Data.Name;
                renewalPeriodInMonths = departmentResult.Data.RenewalPeriodInMonths;
            }

            return (userFolder, documentTypeName, renewalPeriodInMonths);
        }

        private async Task<(IdtFile newFile, FileDTO fileDTO)> ProcessFileUploadAsync(UploadFileDTO file, string wwwrootPath, string documentTypeName, int? renewalPeriodInMonths, string userFolder, string dateFolder, CancellationToken cancellationToken)
        {
            var fileExtension = Path.GetExtension(file.FormFile.FileName).ToLowerInvariant();
            var newFileName = $"{NewId.NextGuid()}{fileExtension}";
            var fullFolderPath = Path.Combine(wwwrootPath, "Uploads", documentTypeName, userFolder, dateFolder);
            var contentType = FileHelper.GetContentType(fileExtension);
            if (!Directory.Exists(fullFolderPath))
            {
                Directory.CreateDirectory(fullFolderPath);
            }

            var uploadPath = Path.Combine(fullFolderPath, newFileName);

            DateTime? startDate = file.StartDate;
            DateTime? endDate = file.EndDate;
            if (file.HasRenewalPeriod.HasValue)
            {
                if (file.HasRenewalPeriod.Value == true && file.StartDate == null && file.EndDate == null)
                {
                    var aiResponse = await aiService.AnalyzeDocumentDateAsync(file.FormFile, documentTypeName, cancellationToken);

                    if (aiResponse.IsSuccess && !string.IsNullOrEmpty(aiResponse.Data.SelectedDate))
                    {
                        startDate = DateTime.Parse(aiResponse.Data.SelectedDate);
                        endDate = startDate.Value.AddMonths(renewalPeriodInMonths!.Value);
                    }
                    else
                    {
                        throw new Exception(aiResponse.Message);
                    }
                }
            }

            // Dosyayı kaydet
            await using var sourceStream = file.FormFile.OpenReadStream();
            await using var fileStream = new FileStream(uploadPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await sourceStream.CopyToAsync(fileStream, cancellationToken);

            var relativePath = $"/Uploads/{documentTypeName}/{userFolder}/{dateFolder}/{newFileName}";

            var newFile = new IdtFile
            {
                Name = newFileName,
                Path = relativePath,
                OriginalName = file.FormFile.FileName,
                TargetUserId = file.TargetUserId,
                TargetProjectId = file.TargetProjectId,
                TargetEquipmentId = file.TargetEquipmentId,
                DocumentTypeId = file.DocumentTypeId!,
                DepartmentId = file.DepartmentId ?? null,
                StartDate = startDate,
                EndDate = endDate,
            };

            var fileDTO = new FileDTO
            {
                Id = newFile.Id,
                Name = newFile.Name,
                OriginalName = newFile.OriginalName,
                DepartmentId = newFile.DepartmentId,
                DocumentTypeId = newFile.DocumentTypeId,
                TargetUserId = newFile.TargetUserId,
                TargetEquipmentId = newFile.TargetEquipmentId,
                TargetProjectId = newFile.TargetProjectId,
                StartDate = newFile.StartDate,
                EndDate = newFile.EndDate,
                CreatedDate = newFile.CreatedDate
            };

            return (newFile, fileDTO);
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
                var file = await unitOfWork.GetRepository<IdtFile>()
                    .WhereAsNoTracking(x => x.Id == fileId)
                    .Include(x => x.Department)
                    .Include(x => x.DocumentType)
                    .Include(x => x.TargetUser)
                    .FirstOrDefaultAsync(cancellationToken);

                if (file == null)
                {
                    logger.LogWarning("Dosya kaydı bulunamadı: {FileId}", fileId);
                    return ApiResponse<FileDTO>.Success(new FileDTO(), "Dosya kaydı bulunamadı");
                }

                var fileDTO = new FileDTO
                {
                    Id = file.Id,
                    Name = file.Name,
                    OriginalName = file.OriginalName,
                    DepartmentId = file.DepartmentId,
                    DocumentTypeId = file.DocumentTypeId,
                    TargetUserId = file.TargetUserId,
                    TargetEquipmentId = file.TargetEquipmentId,
                    TargetProjectId = file.TargetProjectId,
                    TargetUserName = file.TargetUser?.Name + " " + file.TargetUser?.Surname,
                    TargetEquipmentName = file.TargetEquipment?.Name,
                    TargetProjectName = file.TargetProject?.Name,
                    DocumentTypeName = file.DocumentType.Name,
                    DepartmentName = file.Department?.Name,
                    EndDate = file.EndDate,
                    StartDate = file.StartDate,
                    CreatedDate = file.CreatedDate
                };

                return ApiResponse<FileDTO>.Success(fileDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosya okunamadı: {FileId}", fileId);
                return ApiResponse<FileDTO>.Error("Dosya okunamadı");
            }
        }

        public async Task<ApiResponse<(FileStream fileStream, string contentType, string originalName)>> GetFileStreamByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var file = await unitOfWork.GetRepository<IdtFile>()
                    .WhereAsNoTracking(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (file == null)
                {
                    logger.LogWarning("Dosya bulunamadı. FileId: {FileId}", id);
                    return ApiResponse<(FileStream fileStream, string contentType, string originalName)>.Error("Dosya bulunamadı");
                }

                var fullPath = Path.Combine(_physicalFileProvider.Root, file.Path.TrimStart('/'));

                if (!File.Exists(fullPath))
                {
                    logger.LogWarning("Fiziksel dosya bulunamadı. FileId: {FileId}, Path: {Path}", id, fullPath);
                    return ApiResponse<(FileStream fileStream, string contentType, string originalName)>.Error("Dosya yolu bulunamadı");
                }

                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                var extension = Path.GetExtension(file.OriginalName).ToLowerInvariant();
                var contentType = FileHelper.GetContentType(extension);
                return ApiResponse<(FileStream fileStream, string contentType, string originalName)>.Success((fileStream, contentType, file.OriginalName), "Dosya kaydı bulunamadı");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosya stream'i okunamadı. FileId: {FileId}", id);
                return ApiResponse<(FileStream fileStream, string contentType, string originalName)>.Error("Dosya stream'i okunamadı");
            }
        }

        public async Task<ApiResponse<List<FileDTO>>> GetFilesByParamsAsync(string? userId, string? documentType, string? departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = unitOfWork.GetRepository<IdtFile>().WhereAsNoTracking(x => x.Id != null);
                query = query.Include(x => x.TargetUser)
                                   .Include(x => x.DocumentType)
                                   .Include(x => x.Department);
                if (!string.IsNullOrWhiteSpace(userId))
                    query = query.Where(x => x.TargetUserId == userId);

                if (!string.IsNullOrWhiteSpace(documentType))
                    query = query.Where(x => x.DocumentTypeId == documentType);

                if (!string.IsNullOrWhiteSpace(departmentId))
                    query = query.Where(x => x.DepartmentId == departmentId);

                var files = await query.ToListAsync(cancellationToken);

                if (!files.Any())
                {
                    return ApiResponse<List<FileDTO>>.Success(new List<FileDTO>(), "Dosya kaydı bulunamadı");
                }

                var fileDTOs = new List<FileDTO>();

                foreach (var file in files)
                {
                    var fileDTO = new FileDTO
                    {
                        Id = file.Id,
                        Name = file.Name,
                        OriginalName = file.OriginalName,
                        DepartmentId = file.DepartmentId,
                        DocumentTypeId = file.DocumentTypeId,
                        TargetUserId = file.TargetUserId,
                        TargetEquipmentId = file.TargetEquipmentId,
                        TargetProjectId = file.TargetProjectId,
                        TargetUserName = file.TargetUser?.Name + " " + file.TargetUser?.Surname,
                        TargetEquipmentName = file.TargetEquipment?.Name,
                        TargetProjectName = file.TargetProject?.Name,
                        DocumentTypeName = file.DocumentType.Name,
                        DepartmentName = file.Department?.Name,
                        EndDate = file.EndDate,
                        StartDate = file.StartDate,
                        CreatedDate = file.CreatedDate
                    };

                    fileDTOs.Add(fileDTO);
                }

                return ApiResponse<List<FileDTO>>.Success(fileDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Dosyalar okunamadı. UserId: {UserId}, DocumentType: {DocumentType}, DepartmentId: {DepartmentId}",
                    userId, documentType, departmentId);
                return ApiResponse<List<FileDTO>>.Error("Dosyalar okunamadı");
            }
        }
    }
}