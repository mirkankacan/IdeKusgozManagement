using IdeKusgozManagement.Application.Interfaces.Services;
using IdeKusgozManagement.Domain.Entities;
using IdeKusgozManagement.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace IdeKusgozManagement.Infrastructure.Data.Interceptors
{
    public class AuditLogInterceptor(IServiceProvider serviceProvider) : SaveChangesInterceptor
    {


        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ProcessEntries(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            ProcessEntries(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ProcessEntries(DbContext? context)
        {
            if (context == null) return;
            using var scope = serviceProvider.CreateScope();
            var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();

            var userId = identityService.GetUserIdOrNull();
            var now = DateTime.Now;

            var entries = context.ChangeTracker.Entries()
                .Where(x => x.Entity is not IdtAuditLog)
                .ToList();

            var auditLogsToAdd = new List<IdtAuditLog>();

            foreach (var entry in entries)
            {
                // 1. BaseEntity için CreatedDate/UpdatedDate güncelle
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreatedDate = now;
                            baseEntity.CreatedBy = userId;
                            break;

                        case EntityState.Modified:
                            baseEntity.UpdatedDate = now;
                            baseEntity.UpdatedBy = userId;
                            entry.Property(nameof(BaseEntity.CreatedDate)).IsModified = false;
                            entry.Property(nameof(BaseEntity.CreatedBy)).IsModified = false;
                            break;
                    }
                }

                // 2. Audit log kaydı oluştur
                if (entry.State == EntityState.Added ||
                    entry.State == EntityState.Modified ||
                    entry.State == EntityState.Deleted)
                {
                    var log = new IdtAuditLog
                    {
                        TableName = entry.Metadata.GetTableName(),
                        Operation = entry.State.ToString(),
                        CreatedDate = now,
                        CreatedBy = userId
                    };

                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            log.OldValue = SerializeProperties(entry.OriginalValues.Properties, entry.OriginalValues);
                            log.NewValue = SerializeProperties(entry.CurrentValues.Properties, entry.CurrentValues);
                            break;

                        case EntityState.Added:
                            log.NewValue = SerializeProperties(entry.CurrentValues.Properties, entry.CurrentValues);
                            break;

                        case EntityState.Deleted:
                            log.OldValue = SerializeProperties(entry.OriginalValues.Properties, entry.OriginalValues);
                            break;
                    }

                    auditLogsToAdd.Add(log);
                }
            }

            // Tüm audit logları tek seferde ekle
            if (auditLogsToAdd.Any())
            {
                context.Set<IdtAuditLog>().AddRange(auditLogsToAdd);
            }
        }

        private static string SerializeProperties(IEnumerable<Microsoft.EntityFrameworkCore.Metadata.IProperty> properties, Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues values)
        {
            var dictionary = properties.ToDictionary(
                p => p.Name,
                p => values[p]
            );
            return JsonSerializer.Serialize(dictionary);
        }
    }
}