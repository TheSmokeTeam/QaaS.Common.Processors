using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors;

/// <summary>
/// Wraps the incoming request payload and optional request metadata in a JSON envelope response.
/// </summary>
public class JsonEnvelopeProcessor : BaseTransactionProcessor<JsonEnvelopeConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var requestHttpMetaData = requestData.MetaData?.Http;
        var envelope = new JsonObject
        {
            [Configuration.BodyPropertyName] = CreateBodyNode(requestData.Body)
        };

        if (Configuration.IncludeBodyType)
        {
            envelope["bodyType"] = requestData.Body?.GetType().FullName;
        }

        if (Configuration.IncludeUri && requestHttpMetaData?.Uri is not null)
        {
            envelope["uri"] = requestHttpMetaData.Uri.ToString();
        }

        if (Configuration.IncludeRequestHeaders && requestHttpMetaData?.RequestHeaders is not null)
        {
            envelope["requestHeaders"] = JsonSerializer.SerializeToNode(requestHttpMetaData.RequestHeaders);
        }

        if (Configuration.IncludePathParameters && requestHttpMetaData?.PathParameters is not null)
        {
            envelope["pathParameters"] = JsonSerializer.SerializeToNode(requestHttpMetaData.PathParameters);
        }

        return ProcessorResponseFactory.CreateResponse(
            envelope,
            Configuration.StatusCode,
            Configuration.ContentType,
            Configuration.ResponseHeaders);
    }

    private static JsonNode? CreateBodyNode(object? body)
    {
        return body switch
        {
            null => null,
            JsonNode node => node.DeepClone(),
            _ => JsonSerializer.SerializeToNode(body)
        };
    }
}
