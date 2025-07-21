using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Helpers.Filters;

using System;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;

[ExcludeFromCodeCoverage]
public static class OrganisationRegistrationSubmissionsFilters
{
    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> Filter(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable,
        GetOrganisationRegistrationSubmissionsFilter filters) => queryable
        .FilterByOrganisationName(filters.OrganisationName)
        .FilterByOrganisationRef(filters.OrganisationReference)
        .FilterByOrganisationType(filters.OrganisationType)
        .FilterBySubmissionStatus(filters.Statuses)
        .FilterByRelevantYear(filters.RelevantYears);

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterByOrganisationName(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? organisationName)
    {
        if (string.IsNullOrWhiteSpace(organisationName)) return queryable;
        var nameParts = organisationName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var completeQueryable = from q in queryable
            where Array.TrueForAll(nameParts,
                part => q.OrganisationName.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        queryable = completeQueryable.Any()
            ? completeQueryable
            : (from q in queryable
                where nameParts.Any(part => q.OrganisationName.Contains(part, StringComparison.OrdinalIgnoreCase))
                select q);
        return queryable;
    }

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterByOrganisationRef(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? organisationRef)
    {
        if (string.IsNullOrWhiteSpace(organisationRef)) return queryable;
        var nameParts = organisationRef.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        queryable = from q in queryable
            where nameParts.Any(part => q.OrganisationReference.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        return queryable;
    }

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterByApplicationRef(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? applicationRef)
    {
        if (string.IsNullOrWhiteSpace(applicationRef)) return queryable;
        var nameParts = applicationRef.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        queryable = from q in queryable
            where nameParts.Any(part => q.ApplicationReferenceNumber.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        return queryable;
    }

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterByRegistrationRef(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? registrationRef)
    {
        if (string.IsNullOrWhiteSpace(registrationRef)) return queryable;
        var nameParts = registrationRef.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        queryable = from q in queryable
            where nameParts.Any(part => q.RegistrationReferenceNumber.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        return queryable;
    }

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterByOrganisationType(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? organisationType)
    {
        if (string.IsNullOrWhiteSpace(organisationType) || organisationType == "none") return queryable;
        queryable = from q in queryable
            where organisationType.Contains(q.OrganisationType.ToString())
            select q;

        return queryable;
    }

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterBySubmissionStatus(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? submissionStatus)
    {
        if (string.IsNullOrWhiteSpace(submissionStatus) || submissionStatus == "none") return queryable;
        queryable = from q in queryable
            where submissionStatus.Contains(q.SubmissionStatus.ToString())
            select q;

        return queryable;
    }

    public static IQueryable<OrganisationRegistrationSubmissionDetailsResponse> FilterByRelevantYear(
        this IQueryable<OrganisationRegistrationSubmissionDetailsResponse> queryable, string? relevantYear)
    {
        if (string.IsNullOrWhiteSpace(relevantYear)) return queryable;
        queryable = from q in queryable
            where relevantYear.Contains(q.RelevantYear.ToString())
            select q;

        return queryable;
    }
}