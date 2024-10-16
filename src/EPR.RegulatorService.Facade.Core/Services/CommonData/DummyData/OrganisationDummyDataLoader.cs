using System;
using System.Text.Json;
using System.Linq;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData
{
    public class OrganisationDummyDataLoader : IDummyDataLoader<OrganisationRegistrationDataCollection>
    {
        public FileFinderHelper FileFinder { get; set; } = new FileFinderHelper();

        public virtual OrganisationRegistrationDataCollection LoadData(string filePath)
        {
            var fileName = FileFinder.LocateRequiredFile(AppContext.BaseDirectory, filePath);
            var jsonData = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<OrganisationRegistrationDataCollection>(jsonData);
        }

        public class FileFinderHelper
        {
            public string LocateRequiredFile(string baseDirectory, string filePath)
            {
                return GetPossiblePaths(baseDirectory, filePath)
                       .FirstOrDefault(path => File.Exists(path)) ?? filePath;
            }

            private static IEnumerable<string> GetPossiblePaths(string baseDirectory, string filePath)
            {
                // Default path
                yield return Path.Combine(baseDirectory, filePath);

                // If build configuration placeholder is present, return paths for Debug and Release
                if (baseDirectory.Contains("$(buildConfiguration)", StringComparison.OrdinalIgnoreCase))
                {
                    yield return ReplaceBuildConfiguration(baseDirectory, filePath, "Debug");
                    yield return ReplaceBuildConfiguration(baseDirectory, filePath, "Release");
                }
            }

            private static string ReplaceBuildConfiguration(string baseDirectory, string filePath, string buildConfiguration)
            {
                var updatedDirectory = baseDirectory.Replace("$(buildConfiguration)", buildConfiguration, StringComparison.OrdinalIgnoreCase);
                return Path.Combine(updatedDirectory, filePath);
            }
        }
    }
}
