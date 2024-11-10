using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Helpers.Filters;

using System;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public static class OrganisationRegistrationSubmissionsFilters
{
    public static IQueryable<RegistrationSubmissionOrganisationDetails> Filter(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable,
        GetOrganisationRegistrationSubmissionsFilter filters) => queryable
        .FilterByOrganisationName(filters.OrganisationName)
        .FilterByOrganisationRef(filters.OrganisationReference)
        .FilterByApplicationRef(filters.ApplicationReferenceNumber)
        .FilterByRegistrationRef(filters.RegistrationReferenceNumber)
        .FilterByOrganisationType(filters.OrganisationType)
        .FilterBySubmissionStatus(filters.Statuses)
        .FilterByRelevantYear(filters.RelevantYears);

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterByOrganisationName(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? organisationName)
    {
        if (string.IsNullOrEmpty(organisationName)) return queryable;
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

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterByOrganisationRef(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? organisationRef)
    {
        if (string.IsNullOrEmpty(organisationRef)) return queryable;
        var nameParts = organisationRef.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        queryable = from q in queryable
            where nameParts.Any(part => q.OrganisationReference.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        return queryable;
    }

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterByApplicationRef(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? applicationRef)
    {
        if (string.IsNullOrEmpty(applicationRef)) return queryable;
        var nameParts = applicationRef.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        queryable = from q in queryable
            where nameParts.Any(part => q.ApplicationReferenceNumber.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        return queryable;
    }

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterByRegistrationRef(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? registrationRef)
    {
        if (string.IsNullOrEmpty(registrationRef)) return queryable;
        var nameParts = registrationRef.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        queryable = from q in queryable
            where nameParts.Any(part => q.RegistrationReferenceNumber.Contains(part, StringComparison.OrdinalIgnoreCase))
            select q;
        return queryable;
    }

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterByOrganisationType(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? organisationType)
    {
        if (string.IsNullOrEmpty(organisationType) || organisationType == "none") return queryable;
        queryable = from q in queryable
            where organisationType.Contains(q.OrganisationType.ToString())
            select q;

        return queryable;
    }

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterBySubmissionStatus(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? submissionStatus)
    {
        if (string.IsNullOrEmpty(submissionStatus) || submissionStatus == "none") return queryable;
        queryable = from q in queryable
            where submissionStatus.Contains(q.SubmissionStatus.ToString())
            select q;

        return queryable;
    }

    public static IQueryable<RegistrationSubmissionOrganisationDetails> FilterByRelevantYear(
        this IQueryable<RegistrationSubmissionOrganisationDetails> queryable, string? relevantYear)
    {
        if (string.IsNullOrEmpty(relevantYear)) return queryable;
        queryable = from q in queryable
            where relevantYear.Contains(q.RegistrationYear)
            select q;

        return queryable;
    }
}