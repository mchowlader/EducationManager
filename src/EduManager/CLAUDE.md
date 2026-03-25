# EduManager — Project Context

## Tech Stack
- .NET 10, C#, Clean Architecture
- PostgreSQL, EF Core 10.0.3
- MediatR (CQRS), AutoMapper, FluentValidation
- Hangfire (background jobs), JWT Bearer auth
- Serilog logging

## Project Structure
- EduManager.Domain — Entities, Interfaces, Constants, Enums
- EduManager.Application — Commands, Queries, DTOs, Handlers
- EduManager.Infrastructure — Repositories, Services, Jobs, Persistence
- EduManager.Api — Endpoints, Middleware, Filters
- EduManager.Migrator — Console app for tenant DB migrations

## Architecture Rules
- No direct DbContext in Application layer
- Use IUnitOfWork for SaveChanges
- Use IRepository<T> for data access
- Soft delete only (IsDelete = true), never hard delete
- BaseEndpoints<TEntity, TCreate, TUpdate, TResponse> for standard CRUD
- MasterRouteAttribute skips TenantMiddleware
- ITenantEntity marker interface for tenant entities

## Multi-Tenancy
- Master DB: Tenants, AdminUsers, Logs
- Tenant DB: Per-tenant isolated PostgreSQL database
- Tenant identified by subdomain or X-Tenant-Slug header
- TenantContext stored in HttpContext.Items["TenantContext"]
- EduDbContext resolves connection string from TenantContext at runtime

## Auth
- Two flows: SuperAdmin (Master DB) and Tenant User (Tenant DB)
- JWT Access Token 30min, Refresh Token 7 days (rotation)
- Refresh tokens hashed with SHA256 before DB storage
- Permissions stored as strings in RolePermissions table
- JWT claims: "permission" (lowercase) for tenant users

## Key Conventions
- All DTOs are records
- Nullable properties in UpdateDto (PATCH behavior)
- ApiResponse<T> wrapper for all responses
- ErrorCode only on 500 errors
- Result<T> pattern in handlers
- Permissions.For(entity, action) generates permission strings
- BaseProfile<TEntity, TCreate, TUpdate, TResponse> for AutoMapper