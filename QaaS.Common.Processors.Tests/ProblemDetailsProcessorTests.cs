using System.Collections.Immutable;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class ProblemDetailsProcessorTests
{
    [Test]
    public void Process_ReturnsExplicitProblemDetailsPayload()
    {
        var processor = new ProblemDetailsProcessor
        {
            Context = Globals.Context,
            Configuration = new ProblemDetailsConfiguration
            {
                StatusCode = 409,
                Title = "Duplicate item",
                Type = "https://qaas.dev/problems/duplicate",
                Detail = "The item already exists.",
                Instance = "/items/42",
                Extensions = new Dictionary<string, string>
                {
                    ["traceId"] = "abc-123"
                },
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Processor"] = "problem-details"
                }
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var body = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(body["type"]?.GetValue<string>(), Is.EqualTo("https://qaas.dev/problems/duplicate"));
            Assert.That(body["title"]?.GetValue<string>(), Is.EqualTo("Duplicate item"));
            Assert.That(body["status"]?.GetValue<int>(), Is.EqualTo(409));
            Assert.That(body["detail"]?.GetValue<string>(), Is.EqualTo("The item already exists."));
            Assert.That(body["instance"]?.GetValue<string>(), Is.EqualTo("/items/42"));
            Assert.That(body["traceId"]?.GetValue<string>(), Is.EqualTo("abc-123"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(409));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("application/problem+json"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Processor"], Is.EqualTo("problem-details"));
        });
    }

    [Test]
    public void Process_CanUseRequestUriAsProblemInstance()
    {
        var processor = new ProblemDetailsProcessor
        {
            Context = Globals.Context,
            Configuration = new ProblemDetailsConfiguration
            {
                StatusCode = 404,
                Title = "Missing",
                Type = "about:blank"
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                MetaData = new MetaData
                {
                    Http = new Http
                    {
                        Uri = new Uri("https://qaas.dev/items/404")
                    }
                }
            });

        var body = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(body["instance"]?.GetValue<string>(), Is.EqualTo("https://qaas.dev/items/404"));
            Assert.That(body["detail"], Is.Null);
        });
    }

    [Test]
    public void Process_CanOmitInstanceDetailAndHeaders()
    {
        var processor = new ProblemDetailsProcessor
        {
            Context = Globals.Context,
            Configuration = new ProblemDetailsConfiguration
            {
                Title = "Validation failed",
                Type = "https://qaas.dev/problems/validation",
                UseRequestUriAsInstance = false,
                Detail = " ",
                ContentType = string.Empty
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var body = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(body["detail"], Is.Null);
            Assert.That(body["instance"], Is.Null);
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    [Test]
    public void Process_CanHandleMissingRequestMetadataWhenUsingRequestUriAsInstance()
    {
        var processor = new ProblemDetailsProcessor
        {
            Context = Globals.Context,
            Configuration = new ProblemDetailsConfiguration()
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var body = AssertBodyJson(result.Body);

        Assert.That(body["instance"], Is.Null);
    }

    [Test]
    public void Process_CanHandleMissingHttpMetadataWhenUsingRequestUriAsInstance()
    {
        var processor = new ProblemDetailsProcessor
        {
            Context = Globals.Context,
            Configuration = new ProblemDetailsConfiguration()
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                MetaData = new MetaData()
            });

        var body = AssertBodyJson(result.Body);

        Assert.That(body["instance"], Is.Null);
    }

    [Test]
    public void Process_CanHandleMissingUriWhenUsingRequestUriAsInstance()
    {
        var processor = new ProblemDetailsProcessor
        {
            Context = Globals.Context,
            Configuration = new ProblemDetailsConfiguration()
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                MetaData = new MetaData
                {
                    Http = new Http()
                }
            });

        var body = AssertBodyJson(result.Body);

        Assert.That(body["instance"], Is.Null);
    }

    private static JsonObject AssertBodyJson(object? body)
    {
        Assert.That(body, Is.TypeOf<JsonObject>());
        return (JsonObject)body!;
    }
}
