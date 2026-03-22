using System.Collections.Immutable;
using System.Text;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class TextTransformProcessorTests
{
    [Test]
    public void Process_TransformsStringPayloadWithTrimReplacePrefixAndSuffix()
    {
        var processor = new TextTransformProcessor
        {
            Context = Globals.Context,
            Configuration = new TextTransformConfiguration
            {
                TrimWhitespace = true,
                SearchText = "hello",
                ReplacementText = "hi",
                Prefix = "[",
                Suffix = "]",
                StatusCode = 202,
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Transform"] = "string"
                }
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object> { Body = "  hello world  " });

        Assert.Multiple(() =>
        {
            Assert.That(ReadUtf8Body(result.Body), Is.EqualTo("[hi world]"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(202));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("text/plain; charset=utf-8"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Transform"], Is.EqualTo("string"));
        });
    }

    [Test]
    public void Process_CanTransformByteArrayPayloadAndRemoveMatchedText()
    {
        var processor = new TextTransformProcessor
        {
            Context = Globals.Context,
            Configuration = new TextTransformConfiguration
            {
                SearchText = "beta",
                ReplacementText = null
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object> { Body = Encoding.UTF8.GetBytes("alpha beta") });

        Assert.That(ReadUtf8Body(result.Body), Is.EqualTo("alpha "));
    }

    [Test]
    public void Process_SerializesNonTextPayloads()
    {
        var processor = new TextTransformProcessor
        {
            Context = Globals.Context,
            Configuration = new TextTransformConfiguration
            {
                Prefix = "json:"
            }
        };

        var result = processor.Process(
            ImmutableList<DataSource>.Empty,
            new Data<object> { Body = new { Name = "Ada" } });

        Assert.That(ReadUtf8Body(result.Body), Is.EqualTo("json:{\"Name\":\"Ada\"}"));
    }

    [Test]
    public void Process_CanHandleNullPayloadWithoutResponseHeaders()
    {
        var processor = new TextTransformProcessor
        {
            Context = Globals.Context,
            Configuration = new TextTransformConfiguration
            {
                ContentType = string.Empty
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());

        Assert.Multiple(() =>
        {
            Assert.That(ReadUtf8Body(result.Body), Is.EqualTo(string.Empty));
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    private static string ReadUtf8Body(object? body)
    {
        Assert.That(body, Is.TypeOf<byte[]>());
        return Encoding.UTF8.GetString((byte[])body!);
    }
}
