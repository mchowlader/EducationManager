namespace EduManager.Domain.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class MasterRouteAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class TenantRouteAttribute : Attribute { }