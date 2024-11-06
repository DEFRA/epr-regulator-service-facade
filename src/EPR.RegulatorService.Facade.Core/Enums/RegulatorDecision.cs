namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegulatorDecision
{
    None = 0,

    Accepted = 1,
    
    Rejected = 2,
    
    Cancelled = 4, //ToDo:: Need to add this and below in submission api enum as well - in the submission API, the Cancelled value is taken
    
    Queried = 5
}