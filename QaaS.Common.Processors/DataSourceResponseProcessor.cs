using System.Collections.Immutable;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors;

/// <summary>
/// Builds the response from generated data produced by one configured data source.
/// </summary>
/// <qaas-docs group="Data-driven responses" subgroup="Data source lookup" />
public class DataSourceResponseProcessor : BaseTransactionProcessor<DataSourceResponseConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var dataSource = ResolveDataSource(dataSourceList);
        if (dataSource is null)
        {
            return CreateFallbackOrThrow($"Data source '{Configuration.DataSourceName ?? "<first>"}' was not found.");
        }

        var generatedData = dataSource.Retrieve().ToList();
        if (generatedData.Count == 0)
        {
            return CreateFallbackOrThrow($"Data source '{dataSource.Name}' produced no data.");
        }

        var selectedData = TrySelectData(generatedData);
        if (selectedData is null)
        {
            return CreateFallbackOrThrow(
                $"Index {Configuration.Index} is out of range for data source '{dataSource.Name}' with {generatedData.Count} items.");
        }

        return ProcessorResponseFactory.CreateResponse(
            selectedData.Body,
            Configuration.StatusCode,
            Configuration.ContentType,
            Configuration.ResponseHeaders);
    }

    private DataSource? ResolveDataSource(IImmutableList<DataSource> dataSourceList)
    {
        return string.IsNullOrWhiteSpace(Configuration.DataSourceName)
            ? dataSourceList.FirstOrDefault()
            : dataSourceList.FirstOrDefault(dataSource =>
                string.Equals(dataSource.Name, Configuration.DataSourceName, StringComparison.Ordinal));
    }

    private Data<object>? TrySelectData(IReadOnlyList<Data<object>> generatedData)
    {
        return Configuration.SelectionMode switch
        {
            DataSourceSelectionMode.First => generatedData[0],
            DataSourceSelectionMode.Last => generatedData[^1],
            DataSourceSelectionMode.ByIndex when Configuration.Index >= 0 && Configuration.Index < generatedData.Count
                => generatedData[Configuration.Index],
            DataSourceSelectionMode.ByIndex => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Data<object> CreateFallbackOrThrow(string message)
    {
        if (Configuration.FallbackBody is not null)
        {
            return ProcessorResponseFactory.CreateTextResponse(
                Configuration.FallbackBody,
                Configuration.StatusCode,
                Configuration.ContentType,
                Configuration.ResponseHeaders);
        }

        throw new InvalidOperationException(message);
    }
}
