using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

public class RequestEchoProcessor : BaseTransactionProcessor<RequestEchoConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var requestHttpMetaData = requestData.MetaData?.Http;
        var responseBody = new JsonObject
        {
            ["BodyType"] = requestData.Body?.GetType().FullName,
            ["Body"] = CreateBodyNode(requestData.Body)
        };

        if (Configuration.IncludeUri && requestHttpMetaData?.Uri is not null)
        {
            responseBody["Uri"] = requestHttpMetaData.Uri.ToString();
        }

        if (Configuration.IncludeRequestHeaders && requestHttpMetaData?.RequestHeaders is not null)
        {
            responseBody["RequestHeaders"] = JsonSerializer.SerializeToNode(requestHttpMetaData.RequestHeaders);
        }

        if (Configuration.IncludePathParameters && requestHttpMetaData?.PathParameters is not null)
        {
            responseBody["PathParameters"] = JsonSerializer.SerializeToNode(requestHttpMetaData.PathParameters);
        }

        return new Data<object>
        {
            Body = responseBody,
            MetaData = new MetaData
            {
                Http = new Http
                {
                    StatusCode = Configuration.StatusCode,
                    ResponseHeaders = BuildResponseHeaders(Configuration.ContentType, Configuration.ResponseHeaders)
                }
            }
        };
    }

    private static JsonNode? CreateBodyNode(object? body)
    {
        return body switch
        {
            null => null,
            byte[] bytes => new JsonObject
            {
                ["base64"] = Convert.ToBase64String(bytes),
                ["text"] = Encoding.UTF8.GetString(bytes),
                ["length"] = bytes.Length
            },
            JsonNode node => node.DeepClone(),
            _ => JsonSerializer.SerializeToNode(body)
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
