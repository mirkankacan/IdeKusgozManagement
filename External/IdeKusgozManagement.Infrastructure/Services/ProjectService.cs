using IdeKusgozManagement.Application.Common;
using IdeKusgozManagement.Application.Contracts.Services;
using IdeKusgozManagement.Application.DTOs.ProjectDTOs;
using IdeKusgozManagement.Application.Interfaces.UnitOfWork;
using IdeKusgozManagement.Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdeKusgozManagement.Infrastructure.Services
{
    public class ProjectService(IUnitOfWork unitOfWork, ILogger<ProjectService> logger) : IProjectService
    {
        public async Task<ServiceResponse<string>> CreateProjectAsync(CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingProject = await unitOfWork.GetRepository<IdtProject>().AnyAsync(e => e.Name.ToLower() == createProjectDTO.Name.ToLower(), cancellationToken);

                if (existingProject)
                {
                    return ServiceResponse<string>.Error("Bu isimde bir proje zaten mevcut");
                }

                var project = createProjectDTO.Adapt<IdtProject>();
                project.IsActive = true;
                await unitOfWork.GetRepository<IdtProject>().AddAsync(project, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<string>.Success(project.Id, "Proje başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResponse<bool>.Error("Proje bulunamadı");
                }

                var isProjectUsed = await unitOfWork.GetRepository<IdtWorkRecord>().AnyAsync(wre => wre.ProjectId == project.Id, cancellationToken);

                if (isProjectUsed)
                {
                    return ServiceResponse<bool>.Error("Bu proje iş kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtProject>().Remove(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Proje başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResponse<bool>.Error("Proje bulunamadı");
                }

                project.IsActive = false;

                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Proje başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResponse<bool>.Error("Proje bulunamadı");
                }

                project.IsActive = true;

                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Proje başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableProjectAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<ProjectDTO>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var projects = await unitOfWork.GetRepository<IdtProject>().WhereAsNoTracking(e => e.IsActive == true).OrderByDescending(e => e.EndDate).ToListAsync(cancellationToken);

                var mappedProjects = projects.Adapt<IEnumerable<ProjectDTO>>();

                return ServiceResponse<IEnumerable<ProjectDTO>>.Success(mappedProjects, "Aktif proje listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveProjectsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<ProjectDTO>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetFirstOrDefaultAsync(x => x.Id == projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResponse<ProjectDTO>.Error("Proje bulunamadı");
                }

                var projectDTO = project.Adapt<ProjectDTO>();

                return ServiceResponse<ProjectDTO>.Success(projectDTO, "Proje başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetProjectByIdAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<IEnumerable<ProjectDTO>>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<IdtProject>? projects = (await unitOfWork.GetRepository<IdtProject>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var projectDTOs = projects.Adapt<IEnumerable<ProjectDTO>>();

                return ServiceResponse<IEnumerable<ProjectDTO>>.Success(projectDTOs, "Proje listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetProjectsAsync işleminde hata oluştu");
                throw;
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProjectAsync(string projectId, UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ServiceResponse<bool>.Error("Proje bulunamadı");
                }

                var existingProject = await unitOfWork.GetRepository<IdtProject>().AnyAsync(e => e.Name.ToLower() == updateProjectDTO.Name.ToLower() && e.Id != projectId, cancellationToken);

                if (existingProject)
                {
                    return ServiceResponse<bool>.Error("Bu isimde başka bir proje zaten mevcut");
                }

                updateProjectDTO.Adapt(project);
                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ServiceResponse<bool>.Success(true, "Proje başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateProjectAsync işleminde hata oluştu");
                throw;
            }
        }
    }
}