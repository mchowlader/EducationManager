using EduManager.Domain.Common;
using EduManager.Domain.Enums;

namespace EduManager.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty ;
        public string Mobile { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string EncryptionSalt { get; set; } = string.Empty;
        public TenantStatus Status { get; set; } = TenantStatus.Pending;
    }
}
