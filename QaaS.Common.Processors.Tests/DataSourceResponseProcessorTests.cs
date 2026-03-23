using System.Collections.Immutable;
using System.Text.Json.Nodes;
using NUnit.Framework;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Generator;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.SessionDataObjects;

namespace QaaS.Common.Processors.Tests;

[TestFixture]
public class DataSourceResponseProcessorTests
{
    [Test]
    public void Process_UsesFirstDataSourceByDefault()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                StatusCode = 202,
                ContentType = "application/json",
                ResponseHeaders = new Dictionary<string, string>
                {
                    ["X-Processor"] = "data-source"
                }
            }
        };

        var result = processor.Process(
            ImmutableList.Create(CreateDataSource("first", new Data<object> { Body = JsonNode.Parse("{\"value\":1}") })),
            new Data<object>());

        var body = AssertBodyJson(result.Body);

        Assert.Multiple(() =>
        {
            Assert.That(body["value"]?.GetValue<int>(), Is.EqualTo(1));
            Assert.That(result.MetaData?.Http?.StatusCode, Is.EqualTo(202));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["Content-Type"], Is.EqualTo("application/json"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders?["X-Processor"], Is.EqualTo("data-source"));
        });
    }

    [Test]
    public void Process_CanSelectLastItemFromNamedDataSource()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                DataSourceName = "target",
                SelectionMode = DataSourceSelectionMode.Last
            }
        };

        var result = processor.Process(
            ImmutableList.Create(
                CreateDataSource("other", new Data<object> { Body = "skip" }),
                CreateDataSource(
                    "target",
                    new Data<object> { Body = "first" },
                    new Data<object> { Body = "last" })),
            new Data<object>());

        Assert.That(result.Body, Is.EqualTo("last"));
    }

    [Test]
    public void Process_CanSelectItemByIndex()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                SelectionMode = DataSourceSelectionMode.ByIndex,
                Index = 1
            }
        };

        var result = processor.Process(
            ImmutableList.Create(
                CreateDataSource(
                    "target",
                    new Data<object> { Body = "first" },
                    new Data<object> { Body = "second" },
                    new Data<object> { Body = "third" })),
            new Data<object>());

        Assert.That(result.Body, Is.EqualTo("second"));
    }

    [Test]
    public void Process_CanReturnFallbackWhenIndexIsNegative()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                SelectionMode = DataSourceSelectionMode.ByIndex,
                Index = -1,
                FallbackBody = "fallback",
                ContentType = string.Empty
            }
        };

        var result = processor.Process(
            ImmutableList.Create(CreateDataSource("target", new Data<object> { Body = "only" })),
            new Data<object>());

        Assert.Multiple(() =>
        {
            Assert.That(result.Body, Is.TypeOf<byte[]>());
            Assert.That(System.Text.Encoding.UTF8.GetString((byte[])result.Body!), Is.EqualTo("fallback"));
            Assert.That(result.MetaData?.Http?.ResponseHeaders, Is.Null);
        });
    }

    [Test]
    public void Process_CanReturnFallbackWhenIndexIsTooLarge()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                SelectionMode = DataSourceSelectionMode.ByIndex,
                Index = 5,
                FallbackBody = "fallback"
            }
        };

        var result = processor.Process(
            ImmutableList.Create(CreateDataSource("target", new Data<object> { Body = "only" })),
            new Data<object>());

        Assert.That(System.Text.Encoding.UTF8.GetString((byte[])result.Body!), Is.EqualTo("fallback"));
    }

    [Test]
    public void Process_CanReturnFallbackWhenResolvedDataSourceProducesNoData()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                FallbackBody = "empty"
            }
        };

        var result = processor.Process(
            ImmutableList.Create(CreateDataSource("target")),
            new Data<object>());

        Assert.That(System.Text.Encoding.UTF8.GetString((byte[])result.Body!), Is.EqualTo("empty"));
    }

    [Test]
    public void Process_CanReturnFallbackWhenNoDataSourcesAreAvailable()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                FallbackBody = "missing"
            }
        };

        var result = processor.Process(ImmutableList<DataSource>.Empty, new Data<object>());

        Assert.That(System.Text.Encoding.UTF8.GetString((byte[])result.Body!), Is.EqualTo("missing"));
    }

    [Test]
    public void Process_ThrowsWhenNamedDataSourceIsMissingAndNoFallbackIsConfigured()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                DataSourceName = "missing"
            }
        };

        var exception = Assert.Throws<InvalidOperationException>(() =>
            processor.Process(
                ImmutableList.Create(CreateDataSource("target", new Data<object> { Body = "only" })),
                new Data<object>()));

        Assert.That(exception?.Message, Is.EqualTo("Data source 'missing' was not found."));
    }

    [Test]
    public void Process_ThrowsWhenSelectionModeIsUnknown()
    {
        var processor = new DataSourceResponseProcessor
        {
            Context = Globals.Context,
            Configuration = new DataSourceResponseConfiguration
            {
                SelectionMode = (DataSourceSelectionMode)999
            }
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            processor.Process(
                ImmutableList.Create(CreateDataSource("target", new Data<object> { Body = "only" })),
                new Data<object>()));
    }

    private static JsonObject AssertBodyJson(object? body)
    {
        Assert.That(body, Is.TypeOf<JsonObject>());
        return (JsonObject)body!;
    }

    private static DataSource CreateDataSource(string name, params Data<object>[] generatedData)
        => new()
        {
            Name = name,
            DataSourceList = ImmutableList<DataSource>.Empty,
            Generator = new StaticGenerator(generatedData)
        };

    private sealed class StaticGenerator(params Data<object>[] generatedData) : BaseGenerator<object>
    {
        public override IEnumerable<Data<object>> Generate(
            IImmutableList<SessionData> sessionDataList,
            IImmutableList<DataSource> dataSourceList)
            => generatedData;
    }
}
