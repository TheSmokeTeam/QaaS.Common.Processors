using System.Collections.Immutable;
using System.Text;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class StaticResponseProcessorTests
{
    [Test]
    public void Process_ReturnsConfiguredBodyStatusCodeAndHeaders()
    {
        var processor = new StaticResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new StaticResponseConfiguration
            {
                Body = "healthy",
                StatusCode = 202,
                ContentType = "text/plain",
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Source"] = "static"
                }
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var responseBody = result.Body as byte[];

        Assert.Multiple(() =>
        {
            Assert.That(responseBody, Is.Not.Null);
            Assert.That(Encoding.UTF8.GetString(responseBody!), Is.EqualTo("healthy"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(202));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("text/plain"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Source"], Is.EqualTo("static"));
        });
    }

    [Test]
    public void Process_AllowsNullBody()
    {
        var processor = new StaticResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new StaticResponseConfiguration
            {
                Body = null,
                StatusCode = 204
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.Null);
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(204));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("text/plain; charset=utf-8"));
        });
    }

    [Test]
    public void Process_CanOmitResponseHeadersWhenNoContentTypeOrCustomHeadersAreConfigured()
    {
        var processor = new StaticResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new StaticResponseConfiguration
            {
                Body = "headerless",
                ContentType = string.Empty,
                ResponseHeaders = null
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());

        Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
    }
}
