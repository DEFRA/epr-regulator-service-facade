using System;
using System.Collections.Generic;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData
{
    public static class OrganisationRegistrationDummyDataCache
    {
        private static readonly Dictionary<string, Lazy<OrganisationRegistrationDataCollection>> _cache = new();

        public static Lazy<OrganisationRegistrationDataCollection> GetOrAdd(string filePath, Func<string, OrganisationRegistrationDataCollection> valueFactory)
        {
            if (!_cache.TryGetValue(filePath, out var lazyData))
            {
                lazyData = new Lazy<OrganisationRegistrationDataCollection>(() => valueFactory(filePath));
                _cache[filePath] = lazyData;
            }
            return lazyData;
        }
    }
}
