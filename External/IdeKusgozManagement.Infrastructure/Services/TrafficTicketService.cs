using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.FileDTOs;
using IdeKusgozManagement.Application.DTOs.TrafficTicketDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class TrafficTicketService(IUnitOfWork unitOfWork, ILogger<TrafficTicketService> logger, IFileService fileService) : ITrafficTicketService
    {
        public async Task<ServiceResult<string>> CreateTrafficTicketAsync(CreateTrafficTicketDTO createTrafficTicketDTO, CancellationToken cancellationToken = default)
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
                        return ServiceResult<string>.Error("Dosya Yükleme Hatası", fileResult.Fail?.Detail ?? "Dosya yüklenirken bir hata oluştu.", HttpStatusCode.BadRequest);
                    }

                    newTicket.FileId = fileResult.Data.FirstOrDefault()?.Id;
                }
                await unitOfWork.GetRepository<IdtTrafficTicket>().AddAsync(newTicket, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResult<string>.SuccessAsCreated(newTicket.Id, $"/api/trafficTickets/{newTicket.Id}");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "CreateTrafficTicketAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteTrafficTicketAsync(string trafficTicketId, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().GetByIdAsync(trafficTicketId, cancellationToken);

                if (ticket == null)
                {
                    return ServiceResult<bool>.Error("Trafik Cezası Bulunamadı", "Belirtilen ID'ye sahip trafik cezası bulunamadı.", HttpStatusCode.NotFound);
                }
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                if (!string.IsNullOrEmpty(ticket.FileId))
                    await fileService.DeleteFileAsync(ticket.FileId, cancellationToken);

                unitOfWork.GetRepository<IdtTrafficTicket>().Remove(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "DeleteTrafficTicketAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<TrafficTicketDTO>> GetTrafficTicketByIdAsync(string trafficTicketId, CancellationToken cancellationToken = default)
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
                    return ServiceResult<TrafficTicketDTO>.Error("Trafik Cezası Bulunamadı", "Belirtilen ID'ye sahip trafik cezası bulunamadı.", HttpStatusCode.NotFound);
                }

                var ticketDTO = ticket.Adapt<TrafficTicketDTO>();

                return ServiceResult<TrafficTicketDTO>.SuccessAsOk(ticketDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetTrafficTicketByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<TrafficTicketDTO>>> GetTrafficTicketsAsync(CancellationToken cancellationToken = default)
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

                return ServiceResult<IEnumerable<TrafficTicketDTO>>.SuccessAsOk(ticketDTOs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetTrafficTicketsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateTrafficTicketAsync(string trafficTicketId, UpdateTrafficTicketDTO updateTrafficTicketDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var ticket = await unitOfWork.GetRepository<IdtTrafficTicket>().GetByIdAsync(trafficTicketId, cancellationToken);

                if (ticket == null)
                {
                    return ServiceResult<bool>.Error("Trafik Cezası Bulunamadı", "Belirtilen ID'ye sahip trafik cezası bulunamadı.", HttpStatusCode.NotFound);
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
                        return ServiceResult<bool>.Error("Dosya Yükleme Hatası", fileResult.Fail?.Detail ?? "Dosya yüklenirken bir hata oluştu.", HttpStatusCode.BadRequest);
                    }

                    ticket.FileId = fileResult.Data.FirstOrDefault()?.Id;
                }
                unitOfWork.GetRepository<IdtTrafficTicket>().Update(ticket);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);
                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                logger.LogError(ex, "UpdateTrafficTicketAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}