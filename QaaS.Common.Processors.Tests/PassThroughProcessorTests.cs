using System.Collections.Immutable;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class PassThroughProcessorTests
{
    [Test]
    public void Process_ReturnsOriginalBodyAndPreservesRequestMetadataByDefault()
    {
        var body = new byte[] { 1, 2, 3 };
        var processor = new PassThroughProcessor
        {
            Context = Globals.Context,
            Configuration = new PassThroughConfiguration
            {
                StatusCode = 202,
                ContentType = "application/octet-stream"
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = body,
                MetaData = new MetaData
                {
                    IoMatchIndex = 5,
                    Http = new Http
                    {
                        Uri = new Uri("https://qaas.dev/passthrough"),
                        RequestHeaders = new Dictionary<string, string>
                        {
                            ["Trace-Id"] = "abc"
                        },
                        PathParameters = new Dictionary<string, string>
                        {
                            ["id"] = "9"
                        }
                    }
                }
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.SameAs(body));
            Assert.That(result.MetaData?.IoMatchIndex, Is.EqualTo(5));
            Assert.That(result.MetaData?.Http?.Uri, Is.EqualTo(new Uri("https://qaas.dev/passthrough")));
            Assert.That(result.MetaData?.Http?.RequestHeaders?["Trace-Id"], Is.EqualTo("abc"));
            Assert.That(result.MetaData?.Http?.PathParameters?["id"], Is.EqualTo("9"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(202));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("application/octet-stream"));
        });
    }

    [Test]
    public void Process_CanReplaceMetadataInsteadOfPreservingIt()
    {
        var processor = new PassThroughProcessor
        {
            Context = Globals.Context,
            Configuration = new PassThroughConfiguration
            {
                StatusCode = 204,
                PreserveMetaData = false,
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Mode"] = "fresh"
                }
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = "payload",
                MetaData = new MetaData
                {
                    IoMatchIndex = 9,
                    Http = new Http
                    {
                        Uri = new Uri("https://qaas.dev/original")
                    }
                }
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.EqualTo("payload"));
            Assert.That(result.MetaData?.IoMatchIndex, Is.Null);
            Assert.That(result.MetaData?.Http?.Uri, Is.Null);
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(204));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Mode"], Is.EqualTo("fresh"));
        });
    }

    [Test]
    public void Process_CanReturnFreshHttpMetadataWithoutHeaders()
    {
        var processor = new PassThroughProcessor
        {
            Context = Globals.Context,
            Configuration = new PassThroughConfiguration
            {
                PreserveMetaData = false
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = "payload"
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.EqualTo("payload"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(200));
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    [Test]
    public void Process_PreservesMetadataModeCanHandleMissingRequestMetadata()
    {
        var processor = new PassThroughProcessor
        {
            Context = Globals.Context,
            Configuration = new PassThroughConfiguration
            {
                StatusCode = 206,
                PreserveMetaData = true
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object>
            {
                Body = "payload",
                MetaData = null
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.EqualTo("payload"));
            Assert.That(result.MetaData, Is.Not.Null);
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(206));
            Assert.That(result.MetaData?.IoMatchIndex, Is.Null);
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }
}
