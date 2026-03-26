namespace EduManager.Domain.Exceptions;

public class TenantContextNotFoundException()
    : Exception("Tenant context could not be resolved.");
