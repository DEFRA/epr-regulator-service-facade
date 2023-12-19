namespace EPR.RegulatorService.Facade.Core.Models.Responses
{
    public class CreateRegulatorOrganisationResponseModel : CheckRegulatorOrganisationExistResponseModel
    {
        public string Nation { get; set; } = null!;
    }
}
