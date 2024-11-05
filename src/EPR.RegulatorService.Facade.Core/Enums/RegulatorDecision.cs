namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegulatorDecision
{
    None = 0,

    Accepted = 1,
    
    Rejected = 2,
    
    Cancelled = 3, //ToDo:: Need to add this and below in submission api enum as well
    
    Queried = 4
}