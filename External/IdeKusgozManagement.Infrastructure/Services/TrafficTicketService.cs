using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class TrafficTicketService(IUnitOfWork unitOfWork, ILogger<TrafficTicketService> logger, IFileService fileService) : ITrafficTicketService
    {
        public async Task<ServiceResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var newTicket = createTrafficTicketDTO.Adapt<IdtTrafficTicket>();

                if (createTrafficTicketDTO.File != null && createTrafficTicketDTO.File.FormFile.Length > 0)
                {
                    createTrafficTicketDTO.File.TargetUserId = !string.IsNullOrEmpty(createTrafficTicketDTO.TargetUserId) ? createTrafficTicketDTO.TargetUserId : null;

                    createTrafficTicketDTO.File.DocumentTypeId = "52E008EE-FFD3-440F-A6BD-CF0E5F1128C1";
                    var fileList = new List<UploadFileDTO> { createTrafficTicketDTO.File };

                    var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResponse<string>.Error(fileResult.Message);
                    }

                    newTicket.FileId = fileResult.Data.FirstOrDefault()?.Id;
                }
                await unitOfWork.GetRepository<IdtTrafficTicket>().AddAsync(newTicket, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResponse<string>.Success(newTicket.Id, "Trafik cezası başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "CreateTrafficTicketAsync işleminde hata oluştu");
                return ServiceResponse<string>.Error("Trafik cezası oluşturulurken hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().GetByIdAsync(trafficTicketId, cancellationToken);

                if (ticket == null)
                {
                    return ServiceResponse<bool>.Error("Trafik cezası bulunamadı");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (!string.IsNullOrEmpty(ticket.FileId))
                    await fileService.DeleteFileAsync(ticket.FileId, cancellationToken);

                unitOfWork.GetRepository<IdtTrafficTicket>().Remove(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResponse<bool>.Success(true, "Trafik cezası başarıyla silindi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteTrafficTicketAsync işleminde hata oluştu");
                return ServiceResponse<bool>.Error("Trafik cezası silinirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<TrafficTicketDTO>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().WhereAsNoTracking(x => x.Id == trafficTicketId)
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.TargetUser)
                    .Include(x => x.File)
                    .OrderByDescending(e => e.CreatedDate)
                    .FirstOrDefaultAsync(cancellationToken);

                if (ticket == null)
                {
                    return ServiceResponse<TrafficTicketDTO>.Error("Trafik cezası bulunamadı");
                }

                var ticketDTO = ticket.Adapt<TrafficTicketDTO>();

                return ServiceResponse<TrafficTicketDTO>.Success(ticketDTO, "Trafik cezası başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetTrafficTicketByIdAsync işleminde hata oluştu");
                return ServiceResponse<TrafficTicketDTO>.Error("Trafik cezası getirilirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<IEnumerable<TrafficTicketDTO>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var tickets = await unitOfWork.GetRepository<IdtTrafficTicket>().WhereAsNoTracking(x => x.Id != null)
                    .Include(x => x.Equipment)
                    .Include(x => x.Project)
                    .Include(x => x.CreatedByUser)
                    .Include(x => x.TargetUser)
                    .Include(x => x.File)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToListAsync(cancellationToken);

                var ticketDTOs = tickets.Adapt<IEnumerable<TrafficTicketDTO>>();

                return ServiceResponse<IEnumerable<TrafficTicketDTO>>.Success(ticketDTOs, "Trafik cezası listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetTrafficTicketsAsync işleminde hata oluştu");
                return ServiceResponse<IEnumerable<TrafficTicketDTO>>.Error("Trafik cezası listesi getirilirken hata oluştu");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().GetByIdAsync(trafficTicketId, cancellationToken);

                if (ticket == null)
                {
                    return ServiceResponse<bool>.Error("Trafik cezası bulunamadı");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                updateTrafficTicketDTO.Adapt(ticket);
                if (updateTrafficTicketDTO.File != null && updateTrafficTicketDTO.File.FormFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(ticket.FileId))
                        await fileService.DeleteFileAsync(ticket.FileId, cancellationToken);

                    updateTrafficTicketDTO.File.TargetUserId = !string.IsNullOrEmpty(updateTrafficTicketDTO.TargetUserId) ? updateTrafficTicketDTO.TargetUserId : null;
                    updateTrafficTicketDTO.File.DocumentTypeId = "52E008EE-FFD3-440F-A6BD-CF0E5F1128C1";
                    var fileList = new List<UploadFileDTO> { updateTrafficTicketDTO.File };

                    var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ServiceResponse<bool>.Error(fileResult.Message);
                    }

                    ticket.FileId = fileResult.Data.FirstOrDefault()?.Id;
                }
                unitOfWork.GetRepository<IdtTrafficTicket>().Update(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResponse<bool>.Success(true, "Trafik cezası başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateTrafficTicketAsync işleminde hata oluştu");
                return ServiceResponse<bool>.Error("Trafik cezası güncellenirken hata oluştu");
            }
        }
    }
}