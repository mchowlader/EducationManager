namespace EduManager.Application.Interfaces;

public interface ICurrentUserService
{
    long UserId { get; }
    bool IsAuthenticated { get; }
}
