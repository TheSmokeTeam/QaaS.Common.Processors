using System.Text;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

internal static class ProcessorResponseFactory
{
    internal static Data<object> CreateResponse(
        object? body,
        int statusCode,
        string? contentType,
        IDictionary<string, string>? configuredHeaders = null)
        => new()
        {
            Body = body,
            MetaData = new MetaData
            {
                Http = new Http
                {
                    StatusCode = statusCode,
                    ResponseHeaders = BuildResponseHeaders(contentType, configuredHeaders)
                }
            }
        };

    internal static Data<object> CreateTextResponse(
        string? body,
        int statusCode,
        string? contentType,
        IDictionary<string, string>? configuredHeaders = null)
        => CreateResponse(body is null ? null : Encoding.UTF8.GetBytes(body), statusCode, contentType, configuredHeaders);

    internal static IDictionary<string, string>? BuildResponseHeaders(
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
