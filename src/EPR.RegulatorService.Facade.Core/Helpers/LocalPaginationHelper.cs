using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Helpers.Filters;
using EPR.RegulatorService.Facade.Core.Helpers.TestData;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
namespace EPR.RegulatorService.Facade.Core.Helpers
{
    public static class LocalPaginationHelper
    {
        public static Tuple<int, List<RegistrationSubmissionOrganisationDetails>> FilterAndOrder(
            List<RegistrationSubmissionOrganisationDetails> data,
            GetOrganisationRegistrationSubmissionsFilter filter)
        {
            var rawItems = data.AsQueryable();
            var filteredItems = rawItems.Filter(filter);
            int totalItems = filteredItems.Count();

            if (filter.PageNumber > (int)Math.Ceiling(totalItems / (double)filter.PageSize))
            {
                filter.PageNumber = (int)Math.Ceiling(totalItems / (double)filter.PageSize);
            }

            var sortedItems = filteredItems
                    .OrderBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Cancelled)
                    .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Refused)
                    .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Granted)
                    .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Queried)
                    .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Pending)
                    .ThenBy(x => x.RegistrationDateTime)
                    .Skip((filter.PageNumber.Value - 1) * filter.PageSize.Value)
                    .Take(filter.PageSize.Value)
                    .ToList();

            return Tuple.Create(totalItems, sortedItems);
        }
        public static string Paginate<T>(List<T>? items, int currentPage, int pageSize, int totalItems)
        {
            currentPage = currentPage < 1 ? 1 : currentPage;

            PaginatedResponse<T> retObj;

            if (items.Count == 0)
            {
                retObj = new PaginatedResponse<T> { Items = [], CurrentPage = currentPage, PageSize = pageSize, TotalItems = 0 };
            }
            else
            {
                if (currentPage > (int)Math.Ceiling(items.Count / (double)pageSize))
                {
                    currentPage = (int)Math.Ceiling(items.Count / (double)pageSize);
                }

                retObj = new PaginatedResponse<T>
                {
                    Items = items,
                    CurrentPage = currentPage,
                    TotalItems = totalItems,
                    PageSize = pageSize
                };
            }

            return JsonSerializer.Serialize(retObj);
        }
    }
}