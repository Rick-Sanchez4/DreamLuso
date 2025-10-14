using DreamLuso.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace DreamLuso.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuditableEntityInterceptor(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var currentUser = GetCurrentUser();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.CreatedAt = utcNow;
                        auditableEntity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        auditableEntity.UpdatedAt = utcNow;
                        auditableEntity.UpdatedBy = currentUser;
                        
                        // Prevenir modificação dos campos de criação
                        entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                        entry.Property(nameof(IAuditableEntity.CreatedBy)).IsModified = false;
                        break;
                }
            }
        }
    }

    private string GetCurrentUser()
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            return httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? httpContext.User.FindFirst(ClaimTypes.Name)?.Value
                   ?? httpContext.User.Identity.Name
                   ?? "System";
        }
        return "System";
    }
}

