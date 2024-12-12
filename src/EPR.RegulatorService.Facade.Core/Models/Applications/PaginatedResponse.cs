namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class PaginatedResponse<T>
{
    public List<T> items { get; set; } = new();
    public int currentPage { get; set; }
    public int totalItems { get; set; }
    public int pageSize { get;  set; }
}
