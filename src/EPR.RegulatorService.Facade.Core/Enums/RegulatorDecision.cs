namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegulatorDecision
{
    None = 0,

    Accepted = 1,
    
    Rejected = 2,

    /// <summary>
    /// Need to add this in submission api enum as well - in the submission API, the Cancelled value is taken
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Need to add this in submission api enum as well
    /// </summary>
    Queried = 5
}