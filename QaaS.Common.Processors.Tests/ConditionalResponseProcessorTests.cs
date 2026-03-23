using System.Collections.Immutable;
using System.Text;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class ConditionalResponseProcessorTests
{
    [Test]
    public void Process_ReturnsFirstMatchingHeaderRule()
    {
        var processor = new ConditionalResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new ConditionalResponseConfiguration
            {
                Rules = new List<ConditionalResponseRule>
                {
                    new ConditionalResponseRule
                    {
                        RequestHeaderName = "X-Mode",
                        ExpectedValue = "advanced",
                        ResponseBody = "header-match",
                        StatusCode = 202,
                        ResponseHeaders = new Dictionary<string, string>
                        {
                            ["X-Rule"] = "header"
                        }
                    }
                }
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
                        RequestHeaders = new Dictionary<string, string>
                        {
                            ["X-Mode"] = "advanced"
                        }
                    }
                }
            });

        Assert.Multiple(() =>
        {
            Assert.That(ReadUtf8Body(result.Body), Is.EqualTo("header-match"));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(202));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Rule"], Is.EqualTo("header"));
        });
    }

    [Test]
    public void Process_CanMatchPathParameterRulesAfterEarlierFailures()
    {
        var processor = new ConditionalResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new ConditionalResponseConfiguration
            {
                Rules = new List<ConditionalResponseRule>
                {
                    new ConditionalResponseRule
                    {
                        RequestHeaderName = "X-Mode",
                        ExpectedValue = "advanced",
                        ResponseBody = "should-not-match"
                    },
                    new ConditionalResponseRule
                    {
                        RequestHeaderName = "Missing",
                        ExpectedValue = "anything",
                        ResponseBody = "missing-header"
                    },
                    new ConditionalResponseRule
                    {
                        PathParameterName = "id",
                        ExpectedValue = "42",
                        ResponseBody = "path-match"
                    }
                }
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
                        RequestHeaders = new Dictionary<string, string>
                        {
                            ["X-Mode"] = "basic"
                        },
                        PathParameters = new Dictionary<string, string>
                        {
                            ["id"] = "42"
                        }
                    }
                }
            });

        Assert.That(ReadUtf8Body(result.Body), Is.EqualTo("path-match"));
    }

    [Test]
    public void Process_CanFallBackWhenMetadataIsMissing()
    {
        var processor = new ConditionalResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new ConditionalResponseConfiguration
            {
                Rules = new List<ConditionalResponseRule>
                {
                    new ConditionalResponseRule
                    {
                        RequestHeaderName = "X-Mode",
                        ExpectedValue = "advanced"
                    },
                    new ConditionalResponseRule
                    {
                        PathParameterName = "id",
                        ExpectedValue = "42"
                    }
                },
                DefaultBody = "fallback",
                DefaultStatusCode = 404
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());

        Assert.That(ReadUtf8Body(result.Body), Is.EqualTo("fallback"));
    }

    [Test]
    public void Process_CanReturnNullBodyWithoutHeadersWhenNoRuleMatches()
    {
        var processor = new ConditionalResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new ConditionalResponseConfiguration
            {
                Rules = new List<ConditionalResponseRule>
                {
                    new ConditionalResponseRule
                    {
                        PathParameterName = "missing",
                        ExpectedValue = "42"
                    }
                },
                DefaultBody = null,
                DefaultContentType = string.Empty
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
                        PathParameters = new Dictionary<string, string>
                        {
                            ["id"] = "41"
                        }
                    }
                }
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.Null);
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(404));
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    private static string ReadUtf8Body(object? body)
    {
        Assert.That(body, Is.TypeOf<byte[]>());
        return Encoding.UTF8.GetString((byte[])body!);
    }
}
