using System.Collections.Immutable;
using System.Text;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;
using QaaS.Framework.SDK.Session.MetaDataObjects;

namespace QaaS.Common.Processors;

public class ExampleProcessor : BaseTransactionProcessor<NoConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        return new Data<object>
        {
            Body = Encoding.UTF8.GetBytes("Hello world! This is an example :)"),
            MetaData = new MetaData { Http = new Http { StatusCode = 200 } }
        };
    }
}
