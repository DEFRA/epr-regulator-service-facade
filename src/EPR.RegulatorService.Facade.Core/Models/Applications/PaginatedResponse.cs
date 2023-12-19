namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get;  set; }
}
