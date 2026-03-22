using System.Collections.Immutable;
using NUnit.Framework;
using QaaS.Framework.SDK.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class StatusCodeTransactionProcessorTests
{
    [Test]
    public void Process_ReturnsConfiguredHttpStatusCode()
    {
        var processor = new StatusCodeTransactionProcessor
        {
            Configuration = new StatusCodeConfiguration
            {
                StatusCode = 418
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var httpMetaData = result.MetaData?.Http;

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.Null);
            Assert.That(httpMetaData, Is.Not.Null);
            Assert.That(httpMetaData?.StatusCode, Is.EqualTo(418));
        });
    }
}
