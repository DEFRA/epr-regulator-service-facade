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
                var jsonData = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<OrganisationRegistrationDataCollection>(jsonData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error loading data from file: {ex.Message}", ex);
            }
        }
    }
}
