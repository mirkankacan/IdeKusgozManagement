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
        public async Task<ApiResponse<string>> CreateProjectAsync(CreateProjectDTO createProjectDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingProject = await unitOfWork.GetRepository<IdtProject>().AnyAsync(e => e.Name.ToLower() == createProjectDTO.Name.ToLower(), cancellationToken);

                if (existingProject)
                {
                    return ApiResponse<string>.Error("Bu isimde bir proje zaten mevcut");
                }

                var project = createProjectDTO.Adapt<IdtProject>();
                project.IsActive = true;
                await unitOfWork.GetRepository<IdtProject>().AddAsync(project, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<string>.Success(project.Id, "Proje başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateProjectAsync işleminde hata oluştu");
                return ApiResponse<string>.Error("Proje oluşturulurken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ApiResponse<bool>.Error("Proje bulunamadı");
                }

                var isProjectUsed = await unitOfWork.GetRepository<IdtWorkRecord>().AnyAsync(wre => wre.ProjectId == project.Id, cancellationToken);

                if (isProjectUsed)
                {
                    return ApiResponse<bool>.Error("Bu proje iş kayıtlarında kullanıldığı için silinemez");
                }

                unitOfWork.GetRepository<IdtProject>().Remove(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Proje başarıyla silindi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteProjectAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Proje silinirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> DisableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ApiResponse<bool>.Error("Proje bulunamadı");
                }

                project.IsActive = false;

                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Proje başarıyla pasif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DisableProjectAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Proje pasif duruma getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> EnableProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ApiResponse<bool>.Error("Proje bulunamadı");
                }

                project.IsActive = true;

                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Proje başarıyla aktif duruma getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EnableProjectAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Proje aktif duruma getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectDTO>>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var projects = await unitOfWork.GetRepository<IdtProject>().Where(e => e.IsActive == true).OrderBy(e => e.Name).ToListAsync(cancellationToken);

                var projectDTOs = projects
                    .Adapt<IEnumerable<ProjectDTO>>();

                return ApiResponse<IEnumerable<ProjectDTO>>.Success(projectDTOs, "Aktif proje listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetActiveProjectsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<ProjectDTO>>.Error("Aktif proje listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<ProjectDTO>> GetProjectByIdAsync(string projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetFirstOrDefaultAsync(x => x.Id == projectId, cancellationToken);

                if (project == null)
                {
                    return ApiResponse<ProjectDTO>.Error("Proje bulunamadı");
                }

                var projectDTO = project.Adapt<ProjectDTO>();

                return ApiResponse<ProjectDTO>.Success(projectDTO, "Proje başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetProjectByIdAsync işleminde hata oluştu");
                return ApiResponse<ProjectDTO>.Error("Proje getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectDTO>>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<IdtProject>? projects = (await unitOfWork.GetRepository<IdtProject>().GetAllAsync(cancellationToken)).OrderByDescending(e => e.CreatedDate);

                var projectDTOs = projects.Adapt<IEnumerable<ProjectDTO>>();

                return ApiResponse<IEnumerable<ProjectDTO>>.Success(projectDTOs, "Proje listesi başarıyla getirildi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetProjectsAsync işleminde hata oluştu");
                return ApiResponse<IEnumerable<ProjectDTO>>.Error("Proje listesi getirilirken hata oluştu");
            }
        }

        public async Task<ApiResponse<bool>> UpdateProjectAsync(string projectId, UpdateProjectDTO updateProjectDTO, CancellationToken cancellationToken = default)
        {
            try
            {
                var project = await unitOfWork.GetRepository<IdtProject>().GetByIdAsync(projectId, cancellationToken);

                if (project == null)
                {
                    return ApiResponse<bool>.Error("Proje bulunamadı");
                }

                var existingProject = await unitOfWork.GetRepository<IdtProject>().AnyAsync(e => e.Name.ToLower() == updateProjectDTO.Name.ToLower() && e.Id != projectId, cancellationToken);

                if (existingProject)
                {
                    return ApiResponse<bool>.Error("Bu isimde başka bir proje zaten mevcut");
                }

                updateProjectDTO.Adapt(project);
                unitOfWork.GetRepository<IdtProject>().Update(project);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponse<bool>.Success(true, "Proje başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateProjectAsync işleminde hata oluştu");
                return ApiResponse<bool>.Error("Proje güncellenirken hata oluştu");
            }
        }
    }
}