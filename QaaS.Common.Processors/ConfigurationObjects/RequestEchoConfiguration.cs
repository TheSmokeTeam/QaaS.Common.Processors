using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record RequestEchoConfiguration
{
    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 200;

    [Required, Description("Response Content-Type header")]
    public string ContentType { get; set; } = "application/json";

    [Description("Include request headers in the echoed JSON response")]
    public bool IncludeRequestHeaders { get; set; } = true;

    [Description("Include request path parameters in the echoed JSON response")]
    public bool IncludePathParameters { get; set; } = true;

    [Description("Include request Uri in the echoed JSON response")]
    public bool IncludeUri { get; set; } = true;

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}
