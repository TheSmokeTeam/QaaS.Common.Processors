using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors;

/// <summary>
/// Reads the incoming payload as text, applies the configured text transformation, and returns the transformed response.
/// </summary>
/// <qaas-docs group="Transformations" subgroup="Text transformation" />
public class TextTransformProcessor : BaseTransactionProcessor<TextTransformConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var transformedText = ReadText(requestData.Body);

        if (Configuration.TrimWhitespace)
        {
            transformedText = transformedText.Trim();
        }

        if (!string.IsNullOrEmpty(Configuration.SearchText))
        {
            transformedText = transformedText.Replace(
                Configuration.SearchText,
                Configuration.ReplacementText ?? string.Empty,
                StringComparison.Ordinal);
        }

        transformedText = $"{Configuration.Prefix}{transformedText}{Configuration.Suffix}";

        return ProcessorResponseFactory.CreateTextResponse(
            transformedText,
            Configuration.StatusCode,
            Configuration.ContentType,
            Configuration.ResponseHeaders);
    }

    private static string ReadText(object? body)
    {
        return body switch
        {
            null => string.Empty,
            byte[] bytes => Encoding.UTF8.GetString(bytes),
            string text => text,
            _ => JsonSerializer.Serialize(body)
        };
    }
}
