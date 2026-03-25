# Project Context
- .NET 10, Clean Architecture
- PostgreSQL, EF Core 10
- Multi-tenant SaaS

# Architecture Rules
- No direct DbContext in handlers
- Use IUnitOfWork for SaveChanges
- Soft delete only