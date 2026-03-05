using System.Collections.Immutable;
using QaaS.Common.Processors.ConfigurationObjects;
using QaaS.Framework.SDK.DataSourceObjects;
using QaaS.Framework.SDK.Hooks.Processor;
using QaaS.Framework.SDK.Session.DataObjects;

namespace QaaS.Common.Processors;

public class GrpcEchoProcessor : BaseTransactionProcessor<NoConfiguration>
{
    public override Data<object> Process(IImmutableList<DataSource> dataSourceList, Data<object> requestData)
    {
        var requestBody = requestData.Body ??
                          throw new ArgumentException("Expected non-null gRPC request body payload");

        var requestType = requestBody.GetType();
        var messageProperty = requestType.GetProperty("Message") ??
                              throw new ArgumentException(
                                  $"Expected request type '{requestType.FullName}' to expose a Message property");
        if (messageProperty.PropertyType != typeof(string))
            throw new ArgumentException("Expected request Message property to be of type string");

        var requestMessage = (string?)messageProperty.GetValue(requestBody) ?? string.Empty;
        var responseType = ResolveResponseType(requestType);
        var response = Activator.CreateInstance(responseType) ??
                       throw new InvalidOperationException($"Could not instantiate response type '{responseType.FullName}'");

        responseType.GetProperty("Message")?.SetValue(response, $"echo::{requestMessage}");
        responseType.GetProperty("Code")?.SetValue(response, 200);

        return new Data<object> { Body = SerializeIfSupported(response) };
    }

    private static Type ResolveResponseType(Type requestType)
    {
        var requestTypeName = requestType.FullName ??
                              throw new InvalidOperationException("Could not resolve request type full name");

        if (!requestTypeName.EndsWith("Request", StringComparison.Ordinal))
            throw new ArgumentException($"Request type '{requestTypeName}' is not supported by GrpcEchoProcessor");

        var responseTypeName = requestTypeName[..^"Request".Length] + "Response";
        return requestType.Assembly.GetType(responseTypeName, throwOnError: false) ??
               throw new InvalidOperationException(
                   $"Could not resolve response type '{responseTypeName}' in assembly '{requestType.Assembly.FullName}'");
    }

    private static object SerializeIfSupported(object response)
    {
        var toByteArrayMethod = response.GetType().GetMethod("ToByteArray", Type.EmptyTypes);
        if (toByteArrayMethod is null || toByteArrayMethod.ReturnType != typeof(byte[]))
            return response;

        return (byte[])toByteArrayMethod.Invoke(response, null)!;
    }
}
