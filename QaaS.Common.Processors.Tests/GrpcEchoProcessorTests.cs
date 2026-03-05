using System.Collections.Immutable;
using System.Text;
using NUnit.Framework;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class GrpcEchoProcessorTests
{
    [Test]
    public void Process_CreatesMatchingResponseTypeAndEchoesMessage()
    {
        var processor = new GrpcEchoProcessor
        {
            Context = Globals.Context
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty,
            new Data<object> { Body = new EchoRequest { Message = "hello" } });

        Assert.That(result.Body, Is.TypeOf<EchoResponse>());
        var response = (EchoResponse)result.Body!;

        Assert.Multiple(() =>
        {
            Assert.That(response.Message, Is.EqualTo("echo::hello"));
            Assert.That(response.Code, Is.EqualTo(200));
        });
    }

    [Test]
    public void Process_ThrowsForUnsupportedRequestTypeName()
    {
        var processor = new GrpcEchoProcessor
        {
            Context = Globals.Context
        };

        Assert.Throws<ArgumentException>(() =>
            processor.Process(ImmutableList<DataSource>.Empty, new Data<object> { Body = new InvalidInput() }));
    }

    [Test]
    public void Process_WhenResponseSupportsToByteArray_ReturnsByteArrayBody()
    {
        var processor = new GrpcEchoProcessor
        {
            Context = Globals.Context
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty,
            new Data<object> { Body = new ProtobufStyleRequest { Message = "hello" } });

        Assert.That(result.Body, Is.TypeOf<byte[]>());
        Assert.That(Encoding.UTF8.GetString((byte[])result.Body!), Is.EqualTo("echo::hello|200"));
    }

    public sealed class EchoRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public sealed class EchoResponse
    {
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }
    }

    public sealed class InvalidInput
    {
        public string Value { get; set; } = string.Empty;
    }

    public sealed class ProtobufStyleRequest
    {
        public string Message { get; set; } = string.Empty;
    }

    public sealed class ProtobufStyleResponse
    {
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }

        public byte[] ToByteArray() => Encoding.UTF8.GetBytes($"{Message}|{Code}");
    }
}
