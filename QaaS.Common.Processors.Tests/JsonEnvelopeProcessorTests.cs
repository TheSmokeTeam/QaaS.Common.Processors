using System.Collections.Immutable;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class JsonEnvelopeProcessorTests
{
    [Test]
    public void Process_CanWrapRequestBodyAndMetadataInCustomEnvelope()
    {
        var requestBody = new Dictionary<string, string>
        {
            ["Name"] = "Ada"
        };

        var processor = new JsonEnvelopeProcessor
        {
            Context = Globals.Context,
            Configuration = new JsonEnvelopeConfiguration
            {
                BodyPropertyName = "payload",
                IncludeUri = true,
                IncludeRequestHeaders = true,
                IncludePathParameters = true,
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Processor"] = "envelope"
                }
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = requestBody,
                MetaData = new MetaData
                {
                    Http = new Http
                    {
                        Uri = new Uri("https://qaas.dev/items/42"),
                        RequestHeaders = new Dictionary<string, string>
                        {
                            ["Accept"] = "application/json"
                        },
                        PathParameters = new Dictionary<string, string>
                        {
                            ["id"] = "42"
                        }
                    }
                }
            });

        var body = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(body["payload"]?["Name"]?.GetValue<string>(), Is.EqualTo("Ada"));
            Assert.That(body["bodyType"]?.GetValue<string>(), Is.EqualTo(requestBody.GetType().FullName));
            Assert.That(body["uri"]?.GetValue<string>(), Is.EqualTo("https://qaas.dev/items/42"));
            Assert.That(body["requestHeaders"]?["Accept"]?.GetValue<string>(), Is.EqualTo("application/json"));
            Assert.That(body["pathParameters"]?["id"]?.GetValue<string>(), Is.EqualTo("42"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Processor"], Is.EqualTo("envelope"));
        });
    }

    [Test]
    public void Process_PreservesJsonNodeBodiesAsDeepClones()
    {
        var processor = new JsonEnvelopeProcessor
        {
            Context = Globals.Context,
            Configuration = new JsonEnvelopeConfiguration
            {
                IncludeBodyType = false,
                ContentType = string.Empty
            }
        };

        var body = JsonNode.Parse("{\"name\":\"Grace\"}")!;
        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object> { Body = body });

        body["name"] = "Changed";
        var responseJson = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(responseJson["data"]?["name"]?.GetValue<string>(), Is.EqualTo("Grace"));
            Assert.That(responseJson["bodyType"], Is.Null);
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    [Test]
    public void Process_CanHandleNullBodiesAndMissingMetadata()
    {
        var processor = new JsonEnvelopeProcessor
        {
            Context = Globals.Context,
            Configuration = new JsonEnvelopeConfiguration
            {
                IncludeUri = true,
                IncludeRequestHeaders = true,
                IncludePathParameters = true
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var body = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(body["data"], Is.Null);
            Assert.That(body["bodyType"], Is.Null);
            Assert.That(body["uri"], Is.Null);
            Assert.That(body["requestHeaders"], Is.Null);
            Assert.That(body["pathParameters"], Is.Null);
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("application/json"));
        });
    }

    private static JsonObject AssertBodyJson(object? body)
    {
        Assert.That(body, Is.TypeOf<JsonObject>());
        return (JsonObject)body!;
    }
}
