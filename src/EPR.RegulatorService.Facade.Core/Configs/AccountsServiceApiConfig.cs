namespace EPR.RegulatorService.Facade.Core.Configs
{
    public class AccountsServiceApiConfig
    {
        public const string SectionName = "AccountsServiceApiConfig";

        public string BaseUrl { get; set; } = null!;
        public string AccountServiceClientId { get; set; } = null!;
        public string Certificate { get; set; } = null!;
        public int Timeout { get; set; }
        public int ServicePooledConnectionLifetime { get; set; }
        public int ServiceRetryCount { get; set; }
        public AccountsServiceEndpoints Endpoints { get; set; } = null!;
    }

    public class AccountsServiceEndpoints
    {
        public string PendingApplications { get; set; } = null!;
        public string GetOrganisationsApplications { get; set; } = null!;
        public string ManageEnrolment { get; set; } = null!;
        public string TransferOrganisationNation { get; set; } = null!;
        public string UserOrganisations { get; set; } = null!;
        public string CreateRegulator { get; set; } = null!;
        public string GetRegulator { get; set; } = null!;
        public string RegulatorInvitation { get; set; } = null!;
        public string RegulatorEnrollment { get; set; } = null!;
        public string RegulatorInvitedUser { get; set; } = null!;
        public string GetRegulatorUsers { get; set; } = null!;
        public string GetOrganisationsBySearchTerm { get; set; } = null!;
        public string GetUsersByOrganisationExternalId { get; set; } = null!;
        public string GetOrganisationDetails { get; set; } = null!;
        public string RegulatorRemoveApprovedUser { get; set; } = null!;    
        public string AddRemoveApprovedUser { get; set; } = null!;
        public string PendingUserDetailChangeRequests { get; set; } = null!;
        public string GetUserDetailChangeRequest { get; set; } = null!;
        public string ApproveOrRejectChangeRequest { get; set; } = null!;

        

    }
}
