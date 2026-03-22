using NUnit.Framework;
using QaaS.Common.Processors;

namespace QaaS.Common.Processors.Tests;

public sealed class ProcessorRetirementCatalogTests
{
    [Test]
    public void RetiredPublicTypes_ShouldMatchTheRetiredPackageSurface()
    {
        Assert.That(
            ProcessorRetirementCatalog.RetiredPublicTypes,
            Is.EquivalentTo(
            [
                "ExampleProcessor",
                "DummyTransactionProcessor",
                "GrpcEchoProcessor",
                "StatusCodeTransactionProcessor",
                "DummyStubConfig",
                "NoConfiguration"
            ]));
    }

    [Test]
    public void BacklogCandidates_ShouldContainTenNamedProcessors()
    {
        Assert.That(ProcessorRetirementCatalog.BacklogCandidates, Has.Count.EqualTo(10));
        Assert.That(
            ProcessorRetirementCatalog.BacklogCandidates.Select(candidate => candidate.Name),
            Is.EquivalentTo(
            [
                "StaticResponseProcessor",
                "TransactionFromDataSources",
                "RequestEchoProcessor",
                "PassThroughProcessor",
                "JsonTemplateProcessor",
                "ConditionalResponseProcessor",
                "SequenceProcessor",
                "LatencyInjectionProcessor",
                "ProblemDetailsProcessor",
                "TextTransformProcessor"
            ]));
        Assert.That(
            ProcessorRetirementCatalog.BacklogCandidates.All(candidate => candidate.SuggestedPackage == ProcessorRetirementCatalog.SuggestedSuccessorPackage),
            Is.True);
    }

    [TestCase("ExampleProcessor", true)]
    [TestCase("QaaS.Common.Processors.ExampleProcessor", true)]
    [TestCase("global::QaaS.Common.Processors.NoConfiguration", true)]
    [TestCase("TextTransformProcessor", false)]
    [TestCase("QaaS.Framework.SDK.Hooks.BaseHooks.StatusCodeTransactionProcessor", false)]
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("   ", false)]
    public void IsRetiredPublicType_ShouldReturnExpectedValue(string? typeName, bool expectedResult)
    {
        Assert.That(ProcessorRetirementCatalog.IsRetiredPublicType(typeName), Is.EqualTo(expectedResult));
    }

    [TestCase("StatusCodeTransactionProcessor")]
    [TestCase("QaaS.Common.Processors.StatusCodeTransactionProcessor")]
    [TestCase("global::QaaS.Common.Processors.StatusCodeTransactionProcessor")]
    [TestCase("QaaS.Framework.SDK.Hooks.BaseHooks.StatusCodeTransactionProcessor")]
    public void TryResolveCanonicalStatusHook_ShouldNormalizeKnownStatusHookNames(string processorTypeName)
    {
        var resolved = ProcessorRetirementCatalog.TryResolveCanonicalStatusHook(processorTypeName, out var canonicalStatusHook);

        Assert.That(resolved, Is.True);
        Assert.That(canonicalStatusHook, Is.EqualTo(ProcessorRetirementCatalog.CanonicalStatusHook));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("DummyTransactionProcessor")]
    public void TryResolveCanonicalStatusHook_ShouldRejectUnknownValues(string? processorTypeName)
    {
        var resolved = ProcessorRetirementCatalog.TryResolveCanonicalStatusHook(processorTypeName, out var canonicalStatusHook);

        Assert.That(resolved, Is.False);
        Assert.That(canonicalStatusHook, Is.Empty);
    }

    [Test]
    public void TryGetBacklogCandidate_ShouldReturnCandidateForKnownNames()
    {
        var resolved = ProcessorRetirementCatalog.TryGetBacklogCandidate("SequenceProcessor", out var candidate);

        Assert.That(resolved, Is.True);
        Assert.That(candidate, Is.Not.Null);
        Assert.That(candidate!.Name, Is.EqualTo("SequenceProcessor"));
        Assert.That(candidate.Summary, Does.Contain("Stateful response sequencing"));
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("QaaS.Common.Processors.ExampleProcessor")]
    public void TryGetBacklogCandidate_ShouldRejectUnknownValues(string? processorTypeName)
    {
        var resolved = ProcessorRetirementCatalog.TryGetBacklogCandidate(processorTypeName, out var candidate);

        Assert.That(resolved, Is.False);
        Assert.That(candidate, Is.Null);
    }

    [Test]
    public void GetMigrationRecommendation_ShouldReturnFrameworkStatusHookForLegacyStatusProcessor()
    {
        var recommendation = ProcessorRetirementCatalog.GetMigrationRecommendation("global::StatusCodeTransactionProcessor");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Kind, Is.EqualTo(ProcessorMigrationKind.FrameworkStatusHook));
            Assert.That(recommendation.Target, Is.EqualTo(ProcessorRetirementCatalog.CanonicalStatusHook));
            Assert.That(recommendation.Guidance, Does.Contain("fully qualified framework hook"));
        });
    }

    [Test]
    public void GetMigrationRecommendation_ShouldReturnBacklogRecommendationForCandidateNames()
    {
        var recommendation = ProcessorRetirementCatalog.GetMigrationRecommendation("LatencyInjectionProcessor");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Kind, Is.EqualTo(ProcessorMigrationKind.SuccessorBacklog));
            Assert.That(recommendation.Target, Is.EqualTo("LatencyInjectionProcessor"));
            Assert.That(recommendation.Guidance, Does.Contain(ProcessorRetirementCatalog.SuggestedSuccessorPackage));
        });
    }

    [Test]
    public void GetMigrationRecommendation_ShouldReturnLocalProcessorGuidanceForRetiredNonStatusTypes()
    {
        var recommendation = ProcessorRetirementCatalog.GetMigrationRecommendation("QaaS.Common.Processors.ExampleProcessor");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Kind, Is.EqualTo(ProcessorMigrationKind.LocalProcessor));
            Assert.That(recommendation.Target, Is.EqualTo(ProcessorRetirementCatalog.LocalProcessorBaseType));
            Assert.That(recommendation.Guidance, Does.Contain("implemented locally"));
        });
    }

    [Test]
    public void GetMigrationRecommendation_ShouldReturnDefaultLocalGuidanceForBlankNames()
    {
        var recommendation = ProcessorRetirementCatalog.GetMigrationRecommendation("   ");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Kind, Is.EqualTo(ProcessorMigrationKind.LocalProcessor));
            Assert.That(recommendation.Target, Is.EqualTo(ProcessorRetirementCatalog.LocalProcessorBaseType));
            Assert.That(recommendation.Guidance, Does.Contain("Author a local processor"));
        });
    }

    [Test]
    public void GetMigrationRecommendation_ShouldPreserveUnknownTypeNameForLocalImplementations()
    {
        var recommendation = ProcessorRetirementCatalog.GetMigrationRecommendation("global::CustomProcessor");

        Assert.Multiple(() =>
        {
            Assert.That(recommendation.Kind, Is.EqualTo(ProcessorMigrationKind.LocalProcessor));
            Assert.That(recommendation.Target, Is.EqualTo("CustomProcessor"));
            Assert.That(recommendation.Guidance, Does.Contain("No shared replacement exists"));
        });
    }
}
