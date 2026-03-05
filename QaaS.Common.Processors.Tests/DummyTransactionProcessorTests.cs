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
public class DummyTransactionProcessorTests
{
    [Test]
    public void Process_BuildsJsonResponseWithConfiguredKeyAndEncodedPayload()
    {
        var processor = new DummyTransactionProcessor
        {
            Configuration = new DummyStubConfig
            {
                DummyKey = "KeyA",
                DummyValue = "ValueA"
            }
        };

        var requestData = new Data<object>
        {
            Body = Encoding.UTF8.GetBytes("abc"),
            MetaData = new MetaData
            {
                Http = new Http
                {
                    PathParameters = new Dictionary<string, string>
                    {
                        ["id"] = "42"
                    }
                }
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, requestData);
        var resultJson = JsonNode.Parse(result.Body?.ToString() ?? "{}")!.AsObject();
        var httpMetaData = result.MetaData?.Http;
        var contentType = httpMetaData?.ResponseHeaders is { } headers &&
                          headers.TryGetValue("Content-Type", out var value)
            ? value
            : null;

        Assert.Multiple(() =>
        {
            Assert.That(httpMetaData, Is.Not.Null);
            Assert.That(httpMetaData?.StatusCode, Is.EqualTo(200));
            Assert.That(contentType, Is.EqualTo("application/json"));
            Assert.That(resultJson["KeyA"]?.ToString(), Is.EqualTo("ValueA"));
            Assert.That(resultJson["EncodedResponseBody"]?.ToString(), Is.EqualTo("YWJj"));
            Assert.That(resultJson["Parameters"]?["id"]?.ToString(), Is.EqualTo("42"));
        });
    }

    [Test]
    public void Process_Throws_WhenRequestBodyIsNotByteArray()
    {
        var processor = new DummyTransactionProcessor
        {
            Configuration = new DummyStubConfig
            {
                DummyKey = "KeyA",
                DummyValue = "ValueA"
            }
        };

        Assert.Throws<ArgumentException>(() =>
            processor.Process(ImmutableList<DataSource>.Empty, new Data<object> { Body = "not-bytes" }));
    }
}
