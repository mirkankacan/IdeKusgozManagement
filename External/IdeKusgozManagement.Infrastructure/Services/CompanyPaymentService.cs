using System.Net;
using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.CompanyPaymentDTOs;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.DTOs.NotificationDTOs;
using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class CompanyPaymentService(IUnitOfWork unitOfWork, ILogger<CompanyPaymentService> logger, IFileService fileService, IIdentityService identityService, INotificationService notificationService) : ICompanyPaymentService
    {
        public async Task<ServiceResult<IEnumerable<CompanyPaymentDTO>>> GetCompanyPaymentsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var companyPayments = await unitOfWork.GetRepository<IdtCompanyPayment>()
                    .WhereAsNoTracking(null)
                    .Include(x => x.Equipment)
                    .Include(x => x.Expense)
                    .Include(x => x.Project)
                    .Include(x => x.Approver)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(cp => cp.CreatedDate)
                    .ToListAsync(cancellationToken);

                var companyPaymentDTOs = companyPayments.Adapt<IEnumerable<CompanyPaymentDTO>>();

                return ServiceResult<IEnumerable<CompanyPaymentDTO>>.SuccessAsOk(companyPaymentDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompanyPaymentsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<CompanyPaymentDTO>> GetCompanyPaymentByIdAsync(string companyPaymentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var companyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>()
                    .WhereAsNoTracking(cp => cp.Id == companyPaymentId)
                    .Include(x => x.Equipment)
                    .Include(x => x.Expense)
                    .Include(x => x.Project)
                     .Include(x => x.Approver)
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(cancellationToken);

                if (companyPayment == null)
                {
                    return ServiceResult<CompanyPaymentDTO>.Error("Şirket Ödemesi Bulunamadı", "Belirtilen ID'ye sahip şirket ödemesi bulunamadı.", HttpStatusCode.NotFound);
                }

                var companyPaymentDTO = companyPayment.Adapt<CompanyPaymentDTO>();

                return ServiceResult<CompanyPaymentDTO>.SuccessAsOk(companyPaymentDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompanyPaymentByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<CompanyPaymentDTO>>> GetCompanyPaymentByStatusAsync(CompanyPaymentStatus status, CancellationToken cancellationToken = default)
        {
            try
            {
                var companyPayments = await unitOfWork.GetRepository<IdtCompanyPayment>()
                    .WhereAsNoTracking(cp => cp.Status == status)
                    .Include(x => x.Equipment)
                    .Include(x => x.Expense)
                    .Include(x => x.Project)
                     .Include(x => x.Approver)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(cp => cp.CreatedDate)
                    .ToListAsync(cancellationToken);

                var companyPaymentDTOs = companyPayments.Adapt<IEnumerable<CompanyPaymentDTO>>();

                return ServiceResult<IEnumerable<CompanyPaymentDTO>>.SuccessAsOk(companyPaymentDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompanyPaymentByStatusAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<CompanyPaymentDTO>>> GetCompanyPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var companyPayments = await unitOfWork.GetRepository<IdtCompanyPayment>()
                    .WhereAsNoTracking(cp => cp.CreatedBy == userId)
                    .Include(x => x.Equipment)
                    .Include(x => x.Expense)
                    .Include(x => x.Project)
                    .Include(x => x.Approver)
                    .Include(x => x.CreatedByUser)
                    .OrderByDescending(cp => cp.CreatedDate)
                    .ToListAsync(cancellationToken);

                if (companyPayments == null || !companyPayments.Any())
                {
                    return ServiceResult<IEnumerable<CompanyPaymentDTO>>.SuccessAsOk(Enumerable.Empty<CompanyPaymentDTO>());
                }

                var companyPaymentDTOs = companyPayments.Adapt<IEnumerable<CompanyPaymentDTO>>();

                return ServiceResult<IEnumerable<CompanyPaymentDTO>>.SuccessAsOk(companyPaymentDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetCompanyPaymentsByUserIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<string>> CreateCompanyPaymentAsync(CreateCompanyPaymentDTO createCompanyPaymentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var companyPayment = createCompanyPaymentDTO.Adapt<IdtCompanyPayment>();
                companyPayment.Status = CompanyPaymentStatus.Pending;

                // Dosya yükleme işlemi
                if (createCompanyPaymentDTO.Files != null && createCompanyPaymentDTO.Files.Any())
                {
                    var fileList = new List<UploadFileDTO>();

                    foreach (var file in createCompanyPaymentDTO.Files)
                    {
                        if (file != null && file.Length > 0)
                        {
                            fileList.Add(new UploadFileDTO
                            {
                                TargetUserId = identityService.GetUserId(),
                                FormFile = file,
                                DocumentTypeId = "C80ABBC6-6525-4806-9089-492754FCA498"
                            });
                        }
                    }

                    if (fileList.Any())
                    {
                        var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);
                        if (!fileResult.IsSuccess)
                        {
                            await unitOfWork.RollbackTransactionAsync(cancellationToken);
                            return ServiceResult<string>.Error("Dosya Yükleme Hatası", fileResult.Fail?.Detail ?? "Dosya yüklenirken bir hata oluştu.", HttpStatusCode.BadRequest);
                        }

                        // Yüklenen dosyaların ID'lerini ; ile birleştir
                        var uploadedFileIds = fileResult.Data?.Select(f => f.Id) ?? Enumerable.Empty<string>();
                        companyPayment.FileIds = string.Join(",", uploadedFileIds);
                    }
                    else
                    {
                        companyPayment.FileIds = string.Empty;
                    }
                }
                else
                {
                    companyPayment.FileIds = string.Empty;
                }

                await unitOfWork.GetRepository<IdtCompanyPayment>().AddAsync(companyPayment, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return ServiceResult<string>.SuccessAsCreated(companyPayment.Id, $"/api/companyPayments/{companyPayment.Id}");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "CreateCompanyPaymentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateCompanyPaymentAsync(string companyPaymentId, UpdateCompanyPaymentDTO updateCompanyPaymentDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var companyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>().GetByIdAsync(companyPaymentId, cancellationToken);

                if (companyPayment == null)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResult<bool>.Error("Şirket Ödemesi Bulunamadı", "Belirtilen ID'ye sahip şirket ödemesi bulunamadı.", HttpStatusCode.NotFound);
                }

                // Dosya güncelleme işlemi
                if (updateCompanyPaymentDTO.File != null && updateCompanyPaymentDTO.File.FormFile.Length > 0)
                {
                    // Eski dosyayı sil
                    if (!string.IsNullOrEmpty(companyPayment.FileIds))
                    {
                        var oldFileIds = companyPayment.FileIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (var oldFileId in oldFileIds)
                        {
                            await fileService.DeleteFileAsync(oldFileId, cancellationToken);
                        }
                    }

                    // Yeni dosyayı yükle
                    var fileList = new List<UploadFileDTO> { updateCompanyPaymentDTO.File };
                    var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResult<bool>.Error("Dosya Yükleme Hatası", fileResult.Fail?.Detail ?? "Dosya yüklenirken bir hata oluştu.", HttpStatusCode.BadRequest);
                    }

                    companyPayment.FileIds = fileResult.Data?.FirstOrDefault()?.Id ?? string.Empty;
                }

                // Diğer alanları güncelle
                updateCompanyPaymentDTO.Adapt(companyPayment);

                unitOfWork.GetRepository<IdtCompanyPayment>().Update(companyPayment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateCompanyPaymentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteCompanyPaymentAsync(string companyPaymentId, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var companyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>().GetByIdAsync(companyPaymentId, cancellationToken);

                if (companyPayment == null)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResult<bool>.Error("Şirket Ödemesi Bulunamadı", "Belirtilen ID'ye sahip şirket ödemesi bulunamadı.", HttpStatusCode.NotFound);
                }

                // Dosyaları sil
                if (!string.IsNullOrEmpty(companyPayment.FileIds))
                {
                    var fileIds = companyPayment.FileIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var fileId in fileIds)
                    {
                        await fileService.DeleteFileAsync(fileId, cancellationToken);
                    }
                }

                unitOfWork.GetRepository<IdtCompanyPayment>().Remove(companyPayment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteCompanyPaymentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> ApproveCompanyPaymentAsync(string companyPaymentId, string? chiefNote = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var companyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>().GetByIdAsync(companyPaymentId, cancellationToken);
                if (companyPayment == null)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResult<bool>.Error("Şirket Ödemesi Bulunamadı", "Belirtilen ID'ye sahip şirket ödemesi bulunamadı.", HttpStatusCode.NotFound);
                }
                if (companyPayment.Status == CompanyPaymentStatus.FinanceApproved || companyPayment.Status == CompanyPaymentStatus.ChiefApproved)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResult<bool>.Error("Şirket Ödemesi Zaten Onaylanmış", "Bu şirket ödemesi daha önce onaylanmış durumda.", HttpStatusCode.BadRequest);
                }

                var userRole = identityService.GetUserRole();
                var userDepartment = identityService.GetUserDepartment();

                if (userRole == "Şef")
                {
                    companyPayment.Status = CompanyPaymentStatus.ChiefApproved;
                    if (!string.IsNullOrWhiteSpace(chiefNote))
                    {
                        companyPayment.ChiefNote = chiefNote;
                    }
                }
                else if (userRole == "Admin" || userRole == "Yönetici" || userDepartment == "İdari ve Mali İşler")
                {
                    companyPayment.Status = CompanyPaymentStatus.FinanceApproved;
                }


                unitOfWork.GetRepository<IdtCompanyPayment>().Update(companyPayment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                // Bildirim gönder
                var approvedCompanyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>()
                    .WhereAsNoTracking(cp => cp.Id == companyPaymentId)
                    .Include(x => x.Equipment)
                    .Include(x => x.Expense)
                    .Include(x => x.Project)
                    .Include(x => x.Approver)
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(cancellationToken);

                if (approvedCompanyPayment != null)
                {
                    var companyPaymentDTO = approvedCompanyPayment.Adapt<CompanyPaymentDTO>();
                    var approverName = identityService.GetUserFullName();
                    CreateNotificationDTO notificationDTO = new()
                    {
                        Message = $"{approverName} tarafından, {companyPaymentDTO.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {companyPaymentDTO.Project} projesi için {companyPaymentDTO.Amount:C} tutarındaki şirket ödemesi talebiniz onaylandı.",
                        Type = NotificationType.CompanyPayment,
                        RedirectUrl = "/sirket-odemesi/listem",
                        TargetUsers = new List<string> { companyPayment.CreatedBy }
                    };
                    await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "ApproveCompanyPaymentAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> RejectCompanyPaymentAsync(string companyPaymentId, string? rejectReason = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var companyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>().GetByIdAsync(companyPaymentId, cancellationToken);
                if (companyPayment == null)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResult<bool>.Error("Şirket Ödemesi Bulunamadı", "Belirtilen ID'ye sahip şirket ödemesi bulunamadı.", HttpStatusCode.NotFound);
                }

                if (companyPayment.Status == CompanyPaymentStatus.FinanceRejected || companyPayment.Status == CompanyPaymentStatus.ChiefRejected)
                {
                    await unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return ServiceResult<bool>.Error("Şirket Ödemesi Zaten Reddedilmiş", "Bu şirket ödemesi daha önce reddedilmiş durumda.", HttpStatusCode.BadRequest);
                }

                var userRole = identityService.GetUserRole();
                var userDepartment = identityService.GetUserDepartment();

                if (userRole == "Şef")
                {
                    companyPayment.Status = CompanyPaymentStatus.ChiefRejected;
                    if (!string.IsNullOrWhiteSpace(rejectReason))
                    {
                        companyPayment.ChiefNote = rejectReason;
                    }
                }
                else if (userRole == "Admin" || userRole == "Yönetici" || userDepartment == "İdari ve Mali İşler")
                {
                    companyPayment.Status = CompanyPaymentStatus.FinanceRejected;
                }

                unitOfWork.GetRepository<IdtCompanyPayment>().Update(companyPayment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                // Bildirim gönder
                var rejectedCompanyPayment = await unitOfWork.GetRepository<IdtCompanyPayment>()
                    .WhereAsNoTracking(cp => cp.Id == companyPaymentId)
                    .Include(x => x.Equipment)
                    .Include(x => x.Expense)
                    .Include(x => x.Project)
                    .Include(x => x.Approver)
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(cancellationToken);

                if (rejectedCompanyPayment != null)
                {
                    var companyPaymentDTO = rejectedCompanyPayment.Adapt<CompanyPaymentDTO>();
                    var approverName = identityService.GetUserFullName();
                    CreateNotificationDTO notificationDTO = new()
                    {
                        Message = $"{approverName} tarafından, {companyPaymentDTO.CreatedDate:dd/MM/yyyy} tarihinde oluşturduğunuz {companyPaymentDTO.Project} projesi için {companyPaymentDTO.Amount:C} tutarındaki şirket ödemesi talebiniz reddedildi.{(string.IsNullOrWhiteSpace(rejectReason) ? "" : $" Red nedeni: {rejectReason}")}",
                        Type = NotificationType.CompanyPayment,
                        RedirectUrl = "/sirket-odemesi/listem",
                        TargetUsers = new List<string> { companyPayment.CreatedBy }
                    };
                    await notificationService.SendNotificationToUsersAsync(notificationDTO, cancellationToken);
                }

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "RejectCompanyPaymentAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}