using EPR.RegulatorService.Facade.Core.Models.Organisations;

namespace EPR.RegulatorService.Facade.API.Shared;

public interface IRegulatorUsers
{
    Task<List<OrganisationUser>> GetRegulatorUsers(Guid userId, Guid organisationId);
}