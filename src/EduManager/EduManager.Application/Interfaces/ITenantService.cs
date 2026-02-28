using System;
using System.Collections.Generic;
using System.Text;

namespace EduManager.Application.Interfaces;

public interface ITenantService
{
    string GetCurrentTenantSlug();
    Task<string?> GetConnectionStringAsync(string slug);
}
