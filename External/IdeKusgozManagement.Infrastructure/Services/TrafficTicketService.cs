using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Enums;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class TrafficTicketService(IUnitOfWork unitOfWork, ILogger<TrafficTicketService> logger, IFileService fileService) : ITrafficTicketService
    {
        public async Task<ApiResponse<string>> CreateTrafficTicketAsync(CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);
                var newTicket = createTrafficTicketDTO.Adapt<IdtTrafficTicket>();

                if (createTrafficTicketDTO.File != null && createTrafficTicketDTO.File.FormFile.Length > 0)
                {
                    createTrafficTicketDTO.File.TargetUserId = !string.IsNullOrEmpty(createTrafficTicketDTO.TargetUserId) ? createTrafficTicketDTO.TargetUserId : null;

                    createTrafficTicketDTO.File.FileType = FileType.TrafficTicket;
                    var fileList = new List<UploadFileDTO> { createTrafficTicketDTO.File };

                    var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ApiResponse<string>.Error(fileResult.Message);
                    }

                    newTicket.FileId = fileResult.Data.FirstOrDefault()?.Id;
                }
                await unitOfWork.GetRepository<IdtTrafficTicket>().AddAsync(newTicket, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ApiResponse<string>.Success(newTicket.Id, "Trafik cezası başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "CreateTrafficTicketAsync işleminde hata oluştu");
                return ApiResponse<string>.Error("Trafik cezası oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().GetByIdAsync(trafficTicketId, cancellationToken);

                if (ticket == null)
                {
                    return ApiResponse<bool>.Error("Trafik cezası bulunamadı");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (!string.IsNullOrEmpty(ticket.FileId))
                    await fileService.DeleteFileAsync(ticket.FileId, cancellationToken);

                unitOfWork.GetRepository<IdtTrafficTicket>().Remove(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ApiResponse<bool>.Success(true, "Trafik cezası başarıyla silindi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteTrafficTicketAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Trafik cezası silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<TrafficTicketDTO>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default)
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
                    return ApiResponse<TrafficTicketDTO>.Error("Trafik cezası bulunamadı");
                }

                var ticketDTO = ticket.Adapt<TrafficTicketDTO>();

                return ApiResponse<TrafficTicketDTO>.Success(ticketDTO, "Trafik cezası başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetTrafficTicketByIdAsync işleminde hata oluştu");
                return ApiResponse<TrafficTicketDTO>.Error("Trafik cezası getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<TrafficTicketDTO>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default)
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

                return ApiResponse<IEnumerable<TrafficTicketDTO>>.Success(ticketDTOs, "Trafik cezası listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetTrafficTicketsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<TrafficTicketDTO>>.Error("Trafik cezası listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().GetByIdAsync(trafficTicketId, cancellationToken);

                if (ticket == null)
                {
                    return ApiResponse<bool>.Error("Trafik cezası bulunamadı");
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                updateTrafficTicketDTO.Adapt(ticket);
                if (updateTrafficTicketDTO.File != null && updateTrafficTicketDTO.File.FormFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(ticket.FileId))
                        await fileService.DeleteFileAsync(ticket.FileId, cancellationToken);

                    updateTrafficTicketDTO.File.TargetUserId = !string.IsNullOrEmpty(updateTrafficTicketDTO.TargetUserId) ? updateTrafficTicketDTO.TargetUserId : null;
                    updateTrafficTicketDTO.File.FileType = FileType.TrafficTicket;
                    var fileList = new List<UploadFileDTO> { updateTrafficTicketDTO.File };

                    var fileResult = await fileService.UploadFileAsync(fileList, cancellationToken);

                    if (!fileResult.IsSuccess)
                    {
                        await unitOfWork.RollbackTransactionAsync(cancellationToken);
                        return ApiResponse<bool>.Error(fileResult.Message);
                    }

                    ticket.FileId = fileResult.Data.FirstOrDefault()?.Id;
                }
                unitOfWork.GetRepository<IdtTrafficTicket>().Update(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ApiResponse<bool>.Success(true, "Trafik cezası başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateTrafficTicketAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Trafik cezası güncellenirken hata oluştu");
            }
        }
    }
}