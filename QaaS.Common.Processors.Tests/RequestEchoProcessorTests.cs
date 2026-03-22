using System.Collections.Immutable;
using System.Text;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class RequestEchoProcessorTests
{
    [Test]
    public void Process_EchoesByteArrayPayloadAndHttpMetadataAsJson()
    {
        var processor = new RequestEchoProcessor
        {
            Context = Globals.Context,
            Configuration = new RequestEchoConfiguration
            {
                StatusCode = 201,
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Processor"] = "echo"
                }
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = Encoding.UTF8.GetBytes("hello"),
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

        var responseJson = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(responseJson["BodyType"]?.GetValue<string>(), Is.EqualTo(typeof(byte[]).FullName));
            Assert.That(responseJson["Body"]?["base64"]?.GetValue<string>(), Is.EqualTo("aGVsbG8="));
            Assert.That(responseJson["Body"]?["text"]?.GetValue<string>(), Is.EqualTo("hello"));
            Assert.That(responseJson["Body"]?["length"]?.GetValue<int>(), Is.EqualTo(5));
            Assert.That(responseJson["Uri"]?.GetValue<string>(), Is.EqualTo("https://qaas.dev/items/42"));
            Assert.That(responseJson["RequestHeaders"]?["Accept"]?.GetValue<string>(), Is.EqualTo("application/json"));
            Assert.That(responseJson["PathParameters"]?["id"]?.GetValue<string>(), Is.EqualTo("42"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(201));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Processor"], Is.EqualTo("echo"));
        });
    }

    [Test]
    public void Process_RespectsFlagsThatDisableOptionalSections()
    {
        var processor = new RequestEchoProcessor
        {
            Context = Globals.Context,
            Configuration = new RequestEchoConfiguration
            {
                IncludeRequestHeaders = false,
                IncludePathParameters = false,
                IncludeUri = false
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = new { Name = "Ada" },
                MetaData = new MetaData
                {
                    Http = new Http
                    {
                        Uri = new Uri("https://qaas.dev/hidden"),
                        RequestHeaders = new Dictionary<string, string> { ["A"] = "B" },
                        PathParameters = new Dictionary<string, string> { ["id"] = "7" }
                    }
                }
            });

        var responseJson = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(responseJson["Body"]?["Name"]?.GetValue<string>(), Is.EqualTo("Ada"));
            Assert.That(responseJson["Uri"], Is.Null);
            Assert.That(responseJson["RequestHeaders"], Is.Null);
            Assert.That(responseJson["PathParameters"], Is.Null);
        });
    }

    [Test]
    public void Process_AllowsNullBodyAndHeaderlessResponses()
    {
        var processor = new RequestEchoProcessor
        {
            Context = Globals.Context,
            Configuration = new RequestEchoConfiguration
            {
                ContentType = string.Empty,
                IncludeRequestHeaders = false,
                IncludePathParameters = false,
                IncludeUri = false
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var responseJson = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(responseJson["BodyType"], Is.Null);
            Assert.That(responseJson["Body"], Is.Null);
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    [Test]
    public void Process_PreservesJsonNodeBodiesWithoutReserializingThem()
    {
        var processor = new RequestEchoProcessor
        {
            Context = Globals.Context,
            Configuration = new RequestEchoConfiguration()
        };

        var body = JsonNode.Parse("{\"name\":\"Grace\"}");
        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object> { Body = body });

        var responseJson = AssertBodyJson(result.Body);

        Assert.That(responseJson["Body"]?["name"]?.GetValue<string>(), Is.EqualTo("Grace"));
    }

    private static JsonObject AssertBodyJson(object? body)
    {
        Assert.That(body, Is.TypeOf<JsonObject>());
        return (JsonObject)body!;
    }
}
