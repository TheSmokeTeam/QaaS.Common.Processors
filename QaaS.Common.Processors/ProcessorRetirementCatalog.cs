namespace QaaS.Common.Processors;

public enum ProcessorMigrationKind
{
    FrameworkStatusHook,
    LocalProcessor,
    SuccessorBacklog
}

public sealed record ProcessorBacklogCandidate(
    string Name,
    string Summary,
    string SuggestedPackage);

public sealed record ProcessorMigrationRecommendation(
    ProcessorMigrationKind Kind,
    string Target,
    string Guidance);

public static class ProcessorRetirementCatalog
{
    public const string CanonicalStatusHook = "QaaS.Framework.SDK.Hooks.BaseHooks.StatusCodeTransactionProcessor";
    public const string LocalProcessorBaseType = "BaseTransactionProcessor<TConfig>";
    public const string SuggestedSuccessorPackage = "QaaS.Mocker.Processors";

    public static IReadOnlyList<string> RetiredPublicTypes { get; } =
    [
        "ExampleProcessor",
        "DummyTransactionProcessor",
        "GrpcEchoProcessor",
        "StatusCodeTransactionProcessor",
        "DummyStubConfig",
        "NoConfiguration"
    ];

    public static IReadOnlyList<ProcessorBacklogCandidate> BacklogCandidates { get; } =
    [
        new("StaticResponseProcessor", "Configurable body, status code, headers, and content type.", SuggestedSuccessorPackage),
        new("TransactionFromDataSources", "Fixture-backed responses with deterministic or random selection modes.", SuggestedSuccessorPackage),
        new("RequestEchoProcessor", "Structured request echo for protocol debugging and contract tests.", SuggestedSuccessorPackage),
        new("PassThroughProcessor", "Payload pass-through with optional metadata overrides.", SuggestedSuccessorPackage),
        new("JsonTemplateProcessor", "JSON templating from request context and selected data-source items.", SuggestedSuccessorPackage),
        new("ConditionalResponseProcessor", "Conditional response selection using headers, params, or body fields.", SuggestedSuccessorPackage),
        new("SequenceProcessor", "Stateful response sequencing for retry and transition scenarios.", SuggestedSuccessorPackage),
        new("LatencyInjectionProcessor", "Fixed or jittered latency injection for timeout and retry tests.", SuggestedSuccessorPackage),
        new("ProblemDetailsProcessor", "RFC-style HTTP problem responses with configurable metadata.", SuggestedSuccessorPackage),
        new("TextTransformProcessor", "Prefix, suffix, or replace transforms over text payloads.", SuggestedSuccessorPackage)
    ];

    private static readonly HashSet<string> RetiredPublicTypeNames =
    [
        ..RetiredPublicTypes,
        "QaaS.Common.Processors.ExampleProcessor",
        "QaaS.Common.Processors.DummyTransactionProcessor",
        "QaaS.Common.Processors.GrpcEchoProcessor",
        "QaaS.Common.Processors.StatusCodeTransactionProcessor",
        "QaaS.Common.Processors.DummyStubConfig",
        "QaaS.Common.Processors.NoConfiguration"
    ];

    private static readonly HashSet<string> StatusHookAliases =
    [
        "StatusCodeTransactionProcessor",
        "QaaS.Common.Processors.StatusCodeTransactionProcessor",
        CanonicalStatusHook
    ];

    private static readonly Dictionary<string, ProcessorBacklogCandidate> BacklogCandidatesByName =
        BacklogCandidates.ToDictionary(candidate => candidate.Name, StringComparer.Ordinal);

    public static bool IsRetiredPublicType(string? typeName)
    {
        var normalizedTypeName = NormalizeTypeName(typeName);
        return normalizedTypeName is not null && RetiredPublicTypeNames.Contains(normalizedTypeName);
    }

    public static bool TryResolveCanonicalStatusHook(string? processorTypeName, out string canonicalStatusHook)
    {
        var normalizedTypeName = NormalizeTypeName(processorTypeName);
        if (normalizedTypeName is null || !StatusHookAliases.Contains(normalizedTypeName))
        {
            canonicalStatusHook = string.Empty;
            return false;
        }

        canonicalStatusHook = CanonicalStatusHook;
        return true;
    }

    public static bool TryGetBacklogCandidate(string? processorTypeName, out ProcessorBacklogCandidate? candidate)
    {
        var normalizedTypeName = NormalizeTypeName(processorTypeName);
        if (normalizedTypeName is null || !BacklogCandidatesByName.TryGetValue(normalizedTypeName, out var resolvedCandidate))
        {
            candidate = null;
            return false;
        }

        candidate = resolvedCandidate;
        return true;
    }

    public static ProcessorMigrationRecommendation GetMigrationRecommendation(string? processorTypeName)
    {
        if (TryResolveCanonicalStatusHook(processorTypeName, out var canonicalStatusHook))
        {
            return new(
                ProcessorMigrationKind.FrameworkStatusHook,
                canonicalStatusHook,
                "Use the fully qualified framework hook name in mocker YAML to avoid ambiguity with QaaS.Mocker's internal fallback processor.");
        }

        if (TryGetBacklogCandidate(processorTypeName, out var backlogCandidate))
        {
            var resolvedBacklogCandidate = backlogCandidate!;
            return new(
                ProcessorMigrationKind.SuccessorBacklog,
                resolvedBacklogCandidate.Name,
                $"{resolvedBacklogCandidate.Name} remains a backlog candidate for a successor shared processor package such as {resolvedBacklogCandidate.SuggestedPackage}.");
        }

        var normalizedTypeName = NormalizeTypeName(processorTypeName);
        if (normalizedTypeName is null)
        {
            return new(
                ProcessorMigrationKind.LocalProcessor,
                LocalProcessorBaseType,
                "Author a local processor in the consuming mocker project when no shared replacement exists.");
        }

        if (IsRetiredPublicType(normalizedTypeName))
        {
            return new(
                ProcessorMigrationKind.LocalProcessor,
                LocalProcessorBaseType,
                "This retired shared type must now be implemented locally in the consuming mocker project.");
        }

        return new(
            ProcessorMigrationKind.LocalProcessor,
            normalizedTypeName,
            "No shared replacement exists. Keep or author this processor locally in the consuming mocker project.");
    }

    private static string? NormalizeTypeName(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return null;
        }

        var normalizedTypeName = typeName.Trim();
        if (normalizedTypeName.StartsWith("global::", StringComparison.Ordinal))
        {
            normalizedTypeName = normalizedTypeName["global::".Length..];
        }

        return normalizedTypeName;
    }
}
