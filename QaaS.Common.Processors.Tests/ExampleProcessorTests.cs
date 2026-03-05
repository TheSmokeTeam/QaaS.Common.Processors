using System.Collections.Immutable;
using System.Text;
using NUnit.Framework;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class ExampleProcessorTests
{
    [Test]
    public void Process_ReturnsExpectedPayloadAndHttpMetadata()
    {
        var processor = new ExampleProcessor
        {
            Context = Globals.Context
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());
        var resultBody = result.Body as byte[];

        Assert.Multiple(() =>
        {
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(200));
            Assert.That(resultBody, Is.Not.Null);
            Assert.That(Encoding.UTF8.GetString(resultBody!),
                Is.EqualTo("Hello world! This is an example :)"));
        });
    }
}
