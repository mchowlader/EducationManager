namespace EduManager.Infrastructure.Jobs;

public record DbUserResult(
    bool IsSuccess, 
    string UserName, 
    string Password
);
