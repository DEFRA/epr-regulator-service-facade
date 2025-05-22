namespace EPR.RegulatorService.Facade.API.Helpers
{
    public static class AntiVirus
    {
        public static string GetContainerName(string submissionType, string suffix) => 
            suffix is null ? submissionType : submissionType + suffix;
    }
}
