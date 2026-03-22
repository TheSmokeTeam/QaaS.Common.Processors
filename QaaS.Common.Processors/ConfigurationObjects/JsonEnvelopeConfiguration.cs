using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record JsonEnvelopeConfiguration
{
    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 200;

    [Required, Description("Response Content-Type header")]
    public string ContentType { get; set; } = "application/json";

    [Required, Description("Name of the JSON property that will contain the request body")]
    public string BodyPropertyName { get; set; } = "data";

    [Description("Include the CLR body type as bodyType in the response")]
    public bool IncludeBodyType { get; set; } = true;

    [Description("Include the request URI as uri in the response")]
    public bool IncludeUri { get; set; }

    [Description("Include request headers as requestHeaders in the response")]
    public bool IncludeRequestHeaders { get; set; }

    [Description("Include path parameters as pathParameters in the response")]
    public bool IncludePathParameters { get; set; }

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}
