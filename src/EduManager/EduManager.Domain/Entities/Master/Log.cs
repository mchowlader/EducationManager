namespace EduManager.Domain.Entities.Master;

public class Log
{
    public long Id { get; set; }
    public DateTime TimeStamp { get; set; }    
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? MessageTemplate { get; set; } 
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public string? ErrorCode { get; set; }
}