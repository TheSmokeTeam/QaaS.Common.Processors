using System.Collections.Immutable;
using System.Text.Json.Nodes;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors;

/// <summary>
/// Returns an RFC 7807 style problem-details JSON response using the configured status and fields.
/// </summary>
/// <qaas-docs group="Error responses" subgroup="Problem details" />
public class ProblemDetailsProcessor : BaseTransactionProcessor<ProblemDetailsConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var instance = !string.IsNullOrWhiteSpace(Configuration.Instance)
            ? Configuration.Instance
            : Configuration.UseRequestUriAsInstance
                ? requestData.MetaData?.Http?.Uri?.ToString()
                : null;

        var problemDetails = new JsonObject
        {
            ["type"] = Configuration.Type,
            ["title"] = Configuration.Title,
            ["status"] = Configuration.StatusCode
        };

        if (!string.IsNullOrWhiteSpace(Configuration.Detail))
        {
            problemDetails["detail"] = Configuration.Detail;
        }

        if (!string.IsNullOrWhiteSpace(instance))
        {
            problemDetails["instance"] = instance;
        }

        if (Configuration.Extensions is not null)
        {
            foreach (var extension in Configuration.Extensions)
            {
                problemDetails[extension.Key] = extension.Value;
            }
        }

        return ProcessorResponseFactory.CreateResponse(
            problemDetails,
            Configuration.StatusCode,
            Configuration.ContentType,
            Configuration.ResponseHeaders);
    }
}
