using System;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData
{
    public class OrganisationDummyDataLoader : IDummyDataLoader<OrganisationRegistrationDataCollection>
    {
        public virtual OrganisationRegistrationDataCollection LoadData(string filePath)
        {
            try
            {
                var fileName = locateRequiredFile(filePath);
                var jsonData = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<OrganisationRegistrationDataCollection>(jsonData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading data from file: {ex.Message}", ex);
            }
        }

        private string locateRequiredFile(string filePath)
        {
            string currentWorkingDirectory = AppContext.BaseDirectory;
            string currentWorkingPath = Path.Combine(currentWorkingDirectory, filePath);
            string testPath = currentWorkingPath;
            
            if (currentWorkingDirectory.Contains("$(buildConfiguration", StringComparison.OrdinalIgnoreCase))
            {
                testPath = currentWorkingPath.Replace("$(buildConfiguration)", "Debug", StringComparison.OrdinalIgnoreCase);

                if (File.Exists(testPath))
                {
                    return testPath;
                }
                testPath = Path.Combine(currentWorkingDirectory.Replace("$(buildConfiguration)", "Release", StringComparison.OrdinalIgnoreCase), filePath);
                if (File.Exists(testPath))
                {
                    return testPath;
                }
            }

            if (File.Exists(testPath))
            {
                return testPath;
            }

            return filePath;
        }
    }
}
