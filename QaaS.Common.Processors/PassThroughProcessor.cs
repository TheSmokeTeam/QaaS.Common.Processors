using System.Collections.Immutable;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

public class PassThroughProcessor : BaseTransactionProcessor<PassThroughConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var requestHttpMetaData = Configuration.PreserveMetaData ? requestData.MetaData?.Http : null;
        var responseHttpMetaData = CreateHttpMetaData(requestHttpMetaData);
        var responseMetaData = Configuration.PreserveMetaData
            ? requestData.MetaData is null
                ? new MetaData { Http = responseHttpMetaData }
                : requestData.MetaData with { Http = responseHttpMetaData }
            : new MetaData { Http = responseHttpMetaData };

        return new Data<object>
        {
            Body = requestData.Body,
            MetaData = responseMetaData
        };
    }

    private Http CreateHttpMetaData(Http? requestHttpMetaData)
    {
        var responseHeaders = BuildResponseHeaders(Configuration.ContentType, Configuration.ResponseHeaders);
        return requestHttpMetaData is null
            ? new Http
            {
                StatusCode = Configuration.StatusCode,
                ResponseHeaders = responseHeaders
            }
            : requestHttpMetaData with
            {
                StatusCode = Configuration.StatusCode,
                ResponseHeaders = responseHeaders
            };
    }

    private static IDictionary<string, string>? BuildResponseHeaders(
        string? contentType,
        IDictionary<string, string>? configuredHeaders)
    {
        var responseHeaders = configuredHeaders is null
            ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string>(configuredHeaders, StringComparer.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            responseHeaders["Content-Type"] = contentType;
        }

        return responseHeaders.Count == 0 ? null : responseHeaders;
    }
}
