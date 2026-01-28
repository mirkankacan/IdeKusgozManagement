using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.ProjectDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger, UserManager<ApplicationUser> userManager) : IProjectService
    {
        public async Task<ServiceResult<string>> CreateProjectAsync(CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingProject = await unitOfWork.GetRepository<IdtProject>().AnyAsync(e => e.Name.ToLower() == createProjectDTO.Name.ToLower(), cancellationToken);

                if (existingProject)
                {
                    return ServiceResult<string>.Error("Proje Zaten Mevcut", "Bu isimde bir proje zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                var project = createProjectDTO.Adapt<IdtProject>();
                project.IsActive = true;
                await unitOfWork.GetRepository<IdtProject>().AddAsync(project, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<string>.SuccessAsCreated(project.Id, $"/api/projects/{project.Id}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResult<bool>.Error("Proje Bulunamadı", "Belirtilen ID'ye sahip proje bulunamadı.", HttpStatusCode.NotFound);
                }

                var isProjectUsed = await unitOfWork.GetRepository<IdtWorkRecord>().AnyAsync(wre => wre.ProjectId == project.Id, cancellationToken);

                if (isProjectUsed)
                {
                    return ServiceResult<bool>.Error("Silme İşlemi Başarısız", "Bu proje iş kayıtlarında kullanıldığı için silinemez.", HttpStatusCode.BadRequest);
                }

                unitOfWork.GetRepository<IdtProject>().Remove(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResult<bool>.Error("Proje Bulunamadı", "Belirtilen ID'ye sahip proje bulunamadı.", HttpStatusCode.NotFound);
                }

                project.IsActive = false;

                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResult<bool>.Error("Proje Bulunamadı", "Belirtilen ID'ye sahip proje bulunamadı.", HttpStatusCode.NotFound);
                }

                project.IsActive = true;

                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<ProjectDTO>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var projects = await unitOfWork.GetRepository<IdtProject>().WhereAsNoTracking(e => e.IsActive == true).OrderByDescending(e => e.EndDate).ToListAsync(cancellationToken);

                var mappedProjects = projects.Adapt<List<ProjectDTO>>();
                foreach (var project in mappedProjects)
                {
                    if (project.TargetUserIds != null && project.TargetUserIds.Any())
                    {
                        project.TargetUsers = new List<string>();

                        foreach (var userId in project.TargetUserIds)
                        {
                            var user = await userManager.FindByIdAsync(userId);
                            if (user != null)
                            {
                                project.TargetUsers.Add($"{user.Name} {user.Surname}");
                            }
                        }
                    }
                    if (project.TargetEquipmentIds != null && project.TargetEquipmentIds.Any())
                    {
                        project.TargetEquipments = new List<string>();
                        foreach (var equipmentId in project.TargetEquipmentIds)
                        {
                            var equipment = await unitOfWork.GetRepository<IdtEquipment>().WhereAsNoTracking(x => x.Id == equipmentId).FirstOrDefaultAsync(cancellationToken);
                            if (equipmentId is not null)
                            {
                                project.TargetEquipments.Add($"{equipment.Name}");
                            }
                        }
                    }
                }

                return ServiceResult<IEnumerable<ProjectDTO>>.SuccessAsOk(mappedProjects);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveProjectsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<ProjectDTO>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetFirstOrDefaultAsync(x => x.Id == projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResult<ProjectDTO>.Error("Proje Bulunamadı", "Belirtilen ID'ye sahip proje bulunamadı.", HttpStatusCode.NotFound);
                }

                var projectDTO = project.Adapt<ProjectDTO>();

                if (projectDTO.TargetUserIds != null && projectDTO.TargetUserIds.Any())
                {
                    projectDTO.TargetUsers = new List<string>();

                    foreach (var userId in projectDTO.TargetUserIds)
                    {
                        var user = await userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            projectDTO.TargetUsers.Add($"{user.Name} {user.Surname}");
                        }
                    }
                }
                if (projectDTO.TargetEquipmentIds != null && project.TargetEquipmentIds.Any())
                {
                    projectDTO.TargetEquipments = new List<string>();
                    foreach (var equipmentId in projectDTO.TargetEquipmentIds)
                    {
                        var equipment = await unitOfWork.GetRepository<IdtEquipment>().WhereAsNoTracking(x => x.Id == equipmentId).FirstOrDefaultAsync(cancellationToken);
                        if (equipmentId is not null)
                        {
                            projectDTO.TargetEquipments.Add($"{equipment.Name}");
                        }
                    }
                }

                return ServiceResult<ProjectDTO>.SuccessAsOk(projectDTO);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetProjectByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<IEnumerable<ProjectDTO>>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<IdtProject>? projects = (await unitOfWork.GetRepository<IdtProject>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var mappedProjects = projects.Adapt<List<ProjectDTO>>();
                foreach (var project in mappedProjects)
                {
                    if (project.TargetUserIds != null && project.TargetUserIds.Any())
                    {
                        project.TargetUsers = new List<string>();

                        foreach (var userId in project.TargetUserIds)
                        {
                            var user = await userManager.FindByIdAsync(userId);
                            if (user != null)
                            {
                                project.TargetUsers.Add($"{user.Name} {user.Surname}");
                            }
                        }
                    }
                    if (project.TargetEquipmentIds != null && project.TargetEquipmentIds.Any())
                    {
                        project.TargetEquipments = new List<string>();
                        foreach (var equipmentId in project.TargetEquipmentIds)
                        {
                            var equipment = await unitOfWork.GetRepository<IdtEquipment>().WhereAsNoTracking(x => x.Id == equipmentId).FirstOrDefaultAsync(cancellationToken);
                            if (equipmentId is not null)
                            {
                                project.TargetEquipments.Add($"{equipment.Name}");
                            }
                        }
                    }
                }
                return ServiceResult<IEnumerable<ProjectDTO>>.SuccessAsOk(mappedProjects);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetProjectsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> UpdateProjectAsync(string projectId, UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResult<bool>.Error("Proje Bulunamadı", "Belirtilen ID'ye sahip proje bulunamadı.", HttpStatusCode.NotFound);
                }

                var existingProject = await unitOfWork.GetRepository<IdtProject>().AnyAsync(e => e.Name.ToLower() == updateProjectDTO.Name.ToLower() && e.Id != projectId, cancellationToken);

                if (existingProject)
                {
                    return ServiceResult<bool>.Error("Proje Zaten Mevcut", "Bu isimde başka bir proje zaten mevcut. Lütfen farklı bir isim kullanın.", HttpStatusCode.BadRequest);
                }

                updateProjectDTO.Adapt(project);
                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResult<bool>.SuccessAsOk(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateProjectAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}