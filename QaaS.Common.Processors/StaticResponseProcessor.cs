using System.Collections.Immutable;
using System.Text;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

/// <summary>
/// Returns a fixed UTF-8 response body with the configured status code, content type, and headers.
/// </summary>
public class StaticResponseProcessor : BaseTransactionProcessor<StaticResponseConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
        => new()
        {
            Body = Configuration.Body is null ? null : Encoding.UTF8.GetBytes(Configuration.Body),
            MetaData = new MetaData
            {
                Http = new Http
                {
                    StatusCode = Configuration.StatusCode,
                    ResponseHeaders = BuildResponseHeaders(Configuration.ContentType, Configuration.ResponseHeaders)
                }
            }
        };

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
