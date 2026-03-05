using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Nodes;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

public class DummyTransactionProcessor : BaseTransactionProcessor<DummyStubConfig>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        if (requestData.Body is not byte[] inputBodyByteArray)
            throw new ArgumentException("Input body object type is not byte array");

        return new Data<object>
        {
            Body = new JsonObject
            {
                [Configuration.DummyKey] = Configuration.DummyValue,
                ["EncodedResponseBody"] = Convert.ToBase64String(inputBodyByteArray),
                ["Parameters"] = JsonNode.Parse(JsonSerializer.Serialize(requestData.MetaData?.Http?.PathParameters))
            },
            MetaData = new MetaData
            {
                Http = new Http
                {
                    StatusCode = 200,
                    ResponseHeaders = new Dictionary<string, string>
                    {
                        ["Content-Type"] = "application/json"
                    }
                }
            }
        };
    }
}
