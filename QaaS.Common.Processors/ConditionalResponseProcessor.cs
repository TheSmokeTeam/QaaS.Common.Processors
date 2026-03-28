using System.Collections.Immutable;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

/// <summary>
/// Returns the first configured response whose rule matches the incoming request metadata, or the configured default response when no rule matches.
/// </summary>
/// <qaas-docs group="Data-driven responses" subgroup="Conditional routing" />
public class ConditionalResponseProcessor : BaseTransactionProcessor<ConditionalResponseConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var requestHttpMetaData = requestData.MetaData?.Http;

        foreach (var rule in Configuration.Rules)
        {
            if (IsMatch(rule, requestHttpMetaData))
            {
                return ProcessorResponseFactory.CreateTextResponse(
                    rule.ResponseBody,
                    rule.StatusCode,
                    rule.ContentType,
                    rule.ResponseHeaders);
            }
        }

        return ProcessorResponseFactory.CreateTextResponse(
            Configuration.DefaultBody,
            Configuration.DefaultStatusCode,
            Configuration.DefaultContentType,
            Configuration.DefaultResponseHeaders);
    }

    private static bool IsMatch(ConditionalResponseRule rule, Http? requestHttpMetaData)
    {
        if (!string.IsNullOrWhiteSpace(rule.RequestHeaderName) &&
            requestHttpMetaData?.RequestHeaders is not null &&
            requestHttpMetaData.RequestHeaders.TryGetValue(rule.RequestHeaderName, out var headerValue))
        {
            return string.Equals(headerValue, rule.ExpectedValue, StringComparison.Ordinal);
        }

        if (!string.IsNullOrWhiteSpace(rule.PathParameterName) &&
            requestHttpMetaData?.PathParameters is not null &&
            requestHttpMetaData.PathParameters.TryGetValue(rule.PathParameterName, out var pathParameterValue))
        {
            return string.Equals(pathParameterValue, rule.ExpectedValue, StringComparison.Ordinal);
        }

        return false;
    }
}
